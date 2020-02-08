using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Cardofun.API.Helpers.Extensions;
using Cardofun.API.Hubs;
using Cardofun.Core.ApiParameters;
using Cardofun.Domain.Models;
using Cardofun.Interfaces.DTOs;
using Cardofun.Interfaces.Repositories;
using Cardofun.Interfaces.ServiceProviders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Cardofun.API.Controllers
{
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        #region Fields
        private readonly ICardofunRepository _cardofunRepository;
        private readonly IMapper _mapper;
        private readonly IImageProvider _imageProvider;
        private readonly IHubContext<ChatHub, IChatHubClient> _messageHub;
        #endregion Fields
        
        #region Constructor
        public MessagesController(ICardofunRepository cardofunRepository, IMapper mapper, 
            IImageProvider imageProvider, IHubContext<ChatHub, IChatHubClient> messageHub)
        {
            _mapper = mapper;
            _cardofunRepository = cardofunRepository;
            _imageProvider = imageProvider;
            _messageHub = messageHub;
        }
        #endregion Constructor

        #region MessagesController methods
        /// <summary>
        /// Gets a page of lastly sent messages to/from a user 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="messagePrams"></param>
        /// <returns></returns>
        [HttpGet("dialogues")]
        public async Task<IActionResult> GetDialoguesForUser(Int32 userId, [FromQuery]MessagePrams messagePrams)
        {
            if (userId != Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messagePrams.UserId = userId;

            var messagesFromRepo = await _cardofunRepository.GetDialoguesForUser(messagePrams);
            var mappedCollection = _mapper.Map<IEnumerable<MessageExtendedDto>>(messagesFromRepo);

            Response.AddPagination(messagesFromRepo.PageNumber, messagesFromRepo.PageSize, messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);
            return Ok(mappedCollection);
        }

        [HttpGet("thread/{secondUserId}")]
        public async Task<IActionResult> GetMessageThreadForUsers(Int32 userId, Int32 secondUserId, [FromQuery]MessageThreadPrams messagePrams)
        {
            if (userId != Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messagePrams.UserId = userId;
            messagePrams.SecondUserId = secondUserId;

            var messagesFromRepo = await _cardofunRepository.GetPaginatedMessageThread(messagePrams);

            // Mark unread messages as read            
            var currentDate = DateTime.Now;
            var unreadMessages = messagesFromRepo.Where(m => m.RecipientId == userId).Where(m => !m.ReadAt.HasValue).ToArray();
            if(unreadMessages != null && unreadMessages.Length > 0)
            {
                for (int i = 0; i < unreadMessages.Length; i++)
                    unreadMessages[i].ReadAt = currentDate;
                await _cardofunRepository.SaveChangesAsync();

                NotifyUsersOfReadMessages(unreadMessages);
            }

            var mappedCollection = _mapper.Map<MessageListDto>(messagesFromRepo);
            
            Response.AddPagination(messagesFromRepo.PageNumber, messagesFromRepo.PageSize, messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);
            return Ok(mappedCollection);
        }

        /// <summary>
        /// Gets a message by it's Id
        /// </summary>
        /// <param name="userId">Id of a user that is trying to get the message</param>
        /// <param name="id">Id of the message</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = nameof(GetMessage))]
        public async Task<IActionResult> GetMessage(Int32 userId, Guid id)
        {
            if (userId != Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepo = await _cardofunRepository.GetMessageAsync(id);

            if (messageFromRepo == null)
                return NotFound();
                
            // Mark this message as read if it's for the user to read
            if (messageFromRepo.RecipientId == userId && !messageFromRepo.ReadAt.HasValue)
            {
                messageFromRepo.ReadAt = DateTime.Now;
                await _cardofunRepository.SaveChangesAsync();
            
                NotifyUsersOfReadMessages(messageFromRepo);
            }
           
            if (messageFromRepo.SenderId == userId || messageFromRepo.RecipientId == userId)
                return Ok(_mapper.Map<MessageExtendedDto>(messageFromRepo));
            else
                return Unauthorized();
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(Int32 userId, 
            [FromBody]MessageForCreationDto messageForCreation)
        {
            if (userId != Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageForCreation.SenderId = userId;

            var recepient = await _cardofunRepository.GetUserAsync(messageForCreation.RecipientId);

            if (recepient == null)
                return BadRequest("Recepient hasn't been found");  

            if (messageForCreation.File != null)
                messageForCreation.Photo = await _imageProvider.SavePictureAsync(messageForCreation.File);

            var message = _mapper.Map<Message>(messageForCreation);
            _cardofunRepository.Add(message);

            if (!await _cardofunRepository.SaveChangesAsync())
                return BadRequest("Could not send the message");

            message = await _cardofunRepository.GetMessageAsync(message.Id);
            var messageToReturn = _mapper.Map<MessageExtendedDto>(message);
            
            _ = _messageHub.Clients.Users(message.SenderId.ToString(), message.RecipientId.ToString()).ReceiveMessage(messageToReturn);

            return CreatedAtRoute(nameof(GetMessage), new { userId, id = messageToReturn.Id }, messageToReturn);
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(Int32 userId, Guid id)
        {
            if (userId != Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var message = await _cardofunRepository.GetMessageAsync(id);
            
            if (message == null)
                return NotFound();

            if (message.RecipientId != userId)
                return Unauthorized();

            if (message.ReadAt.HasValue)
                return BadRequest("The message had been marked as read before");

            message.ReadAt = DateTime.Now;
            var saved = await _cardofunRepository.SaveChangesAsync();

            NotifyUsersOfReadMessages(message);

            if(saved)
                return NoContent();
            
            return BadRequest("Cannot mark this message as read");
        }
        #endregion MessagesController methods

        #region SignalR
        private void NotifyUsersOfReadMessages(params Message[] readMessages)
        {
            var chats = readMessages
                .GroupBy(m => m.SenderId >= m.RecipientId ? (m.SenderId, m.RecipientId) : (m.RecipientId, m.SenderId));

            foreach (var chat in chats)
            {
                var readMessage = new ReadMessagesListDto 
                { 
                    UserOneId = chat.Key.Item1,
                    UserTwoId = chat.Key.Item2,
                    MessageIds = chat.Select(m => m.Id)
                };

                _ = _messageHub.Clients.Users(chat.Key.Item1.ToString(), chat.Key.Item2.ToString())
                    .MarkMessageAsRead(readMessage);
            }
            
        }
        #endregion SignalR
    }
}