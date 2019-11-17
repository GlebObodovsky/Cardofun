using System.Threading.Tasks;
using Cardofun.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using AutoMapper;
using Cardofun.Interfaces.DTOs;
using System.Collections.Generic;
using System.Security.Claims;
using Cardofun.Domain.Models;
using Cardofun.Core.ApiParameters;
using Cardofun.API.Helpers.Extensions;
using System.Linq;
using Cardofun.Core.Enums;

namespace Cardofun.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        #region Fields
        private readonly ICardofunRepository _cardofunRepository;
        private readonly IMapper _mapper;
        #endregion Fields

        #region Constructor
        public UsersController(ICardofunRepository cardofunRepository, IMapper mapper)
        {
            _cardofunRepository = cardofunRepository;
            _mapper = mapper;
        }
        #endregion Constructor

        #region Controller methods
        /// <summary>
        /// Checks if user with the given login already exists
        /// </summary>
        /// <returns></returns>
        [HttpHead("{login}")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckIfUserExists(String login)
        {
            if(await _cardofunRepository.CheckIfUserExists(login))
                return Ok();
            
            return NotFound();
        }

        /// <summary>
        /// Gets a page of users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            userParams.UserId = Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userPages = await _cardofunRepository.GetPageOfUsersAsync(userParams);
            Response.AddPagination(userPages.PageNumber, userPages.PageSize, userPages.TotalCount, userPages.TotalPages);
            return Ok(_mapper.Map<IEnumerable<UserForListDto>>(userPages));
        }

        /// <summary>
        /// Gets a user by the given id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}", Name = nameof(GetUser))]
        public async Task<IActionResult> GetUser(Int32 id)
            => Ok(_mapper.Map<UserForDetailedDto>(await _cardofunRepository.GetUserAsync(id)));

        /// <summary>
        /// Updates a user by the given id
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Int32 id, UserForUpdateDto newUserInfo)
        {
            if(id != Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            // If there is any learning language that is the same as speaking
            if(newUserInfo.LanguagesTheUserLearns.Any(ll => newUserInfo.LanguagesTheUserSpeaks.Any(sl => sl.Code.Equals(ll.Code))))
                return BadRequest("One cannot speak and learn same language");

            _cardofunRepository.StartTransaction();

            var user = await _cardofunRepository.GetUserAsync(id);

            // checking if there's a language that the user was learning before but is speaking now
            var anyUpgraded = user.LanguagesTheUserLearns.Any(oldLearningLang => newUserInfo.LanguagesTheUserSpeaks.Any(newSpeakingLang => oldLearningLang.LanguageCode.Equals(newSpeakingLang.Code)));
            // checking if there's a language that the user was speaking before but is learning now
            var anyDowngraded = user.LanguagesTheUserSpeaks.Any(oldSpeakingLang => newUserInfo.LanguagesTheUserLearns.Any(newLearningLang => oldSpeakingLang.LanguageCode.Equals(newLearningLang.Code)));
            // if any of above is true - clearing up old values in order not to get 
            // unique constraint error while saving after new values are applied
            if(anyUpgraded || anyDowngraded)
            {
                user.LanguagesTheUserLearns = null;
                user.LanguagesTheUserSpeaks = null;
                await _cardofunRepository.SaveChangesAsync();
            }

            _mapper.Map<UserForUpdateDto, User>(newUserInfo, user);
            
            if(await _cardofunRepository.SaveChangesAsync())
            {
                _cardofunRepository.CommitTransaction();
                return NoContent();
            }

            return BadRequest("Updating user failed on save");
        }

        
        /// <summary>
        /// Returns list of the user's friends
        /// </summary>
        /// <param name="id">User Id</param>
        /// <returns></returns>
        [HttpGet("{id}/friends")]
        public async Task<IActionResult> GetUserFriends(Int32 id, [FromQuery]UserFriendParams userFriendParams)
        {
            if(id != Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            userFriendParams.UserId = id;

            var userPages = await _cardofunRepository.GetPageOfFriendsAsync(userFriendParams);
            Response.AddPagination(userPages.PageNumber, userPages.PageSize, userPages.TotalCount, userPages.TotalPages);
            return Ok(_mapper.Map<IEnumerable<UserForListDto>>(userPages));
        }

        /// <summary>
        /// Request a friendship
        /// </summary>
        /// <param name="id">User Id</param>
        /// <param name="recepientId">Id of a user that ought to be a friend</param>
        /// <returns></returns>
        [HttpPost("{id}/friends/{recepientId}")]
        public async Task<IActionResult> RequestFriendship(Int32 id, Int32 recepientId)
        {
            if(id != Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            if(id == recepientId)
                return BadRequest("It is not possible to invite yourself to be friends");

            var friendRequest = await _cardofunRepository.GetFriendRequestAsync(id, recepientId);

            if(friendRequest != null)
                return BadRequest("The friend request has been already sent before");

            friendRequest = await _cardofunRepository.GetFriendRequestAsync(recepientId, id);

            if(friendRequest != null)
                return BadRequest("There is a friend request from recepient waiting for reply");

            friendRequest = new FriendRequest
            {
                FromUserId = id,
                ToUserId = recepientId,
            };
            _cardofunRepository.Add(friendRequest);
            if(await _cardofunRepository.SaveChangesAsync())
                return Ok();

            return BadRequest("Befriending user failed on save");
        }

        /// <summary>
        /// Changing a requested friendship status
        /// </summary>
        /// <param name="id">User Id</param>
        /// <param name="recepientId">Id of a user that requested the friendship</param>
        /// <param name="status">New friendship status (Requested, Accepted, Declined)</param>
        /// <returns></returns>
        [HttpPut("{id}/friends/{recepientId}")]
        public async Task<IActionResult> ReplyOnFriendshipRequest(Int32 id, Int32 recepientId, [FromBody]String status)
        {
            if(id != Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            if(id == recepientId)
                return BadRequest("It's not possible to reply on own friendship request");

            var friendRequest = await _cardofunRepository.GetFriendRequestAsync(recepientId, id);

            if(friendRequest == null)
                return BadRequest("The friend request has not been found");

            if(!Enum.TryParse(typeof(FriendshipStatus), status, true, out object fStatus))
            {
                var statuses = String.Join(", ", 
                    Enum.GetValues(typeof(FriendshipStatus))
                        .Cast<FriendshipStatus>()
                        .Select(x => x.ToString()));

                return BadRequest($"Wrong status. Must be either of these: {statuses}");
            }
            friendRequest.Status = (FriendshipStatus)fStatus;
            friendRequest.RepliedAt = DateTime.Now;

            if(await _cardofunRepository.SaveChangesAsync())
                return Ok();

            return BadRequest("Changing friendship status failed on save");
        }

        /// <summary>
        /// Removing a friendship request
        /// </summary>
        /// <param name="id">User Id</param>
        /// <param name="recepientId">Id of a user that will be excluded from friend list</param>
        /// <returns></returns>
        [HttpDelete("{id}/friends/{recepientId}")]
        public async Task<IActionResult> RemoveFriendshipRequest(Int32 id, Int32 recepientId)
        {
            if(id != Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var request = await _cardofunRepository.GetFriendRequestAsync(id, recepientId);

            if(request == null)
                return BadRequest("The friend request hasn't been found");

            _cardofunRepository.Delete(request);

            if(await _cardofunRepository.SaveChangesAsync())
                return Ok();

            return BadRequest("Friendship request deletion failed on save");
        }
        #endregion Controller methods
    }
}