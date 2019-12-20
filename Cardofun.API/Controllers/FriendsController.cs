using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Cardofun.API.Helpers.Extensions;
using Cardofun.Core.ApiParameters;
using Cardofun.Core.Enums;
using Cardofun.Domain.Models;
using Cardofun.Interfaces.DTOs;
using Cardofun.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Cardofun.API.Controllers
{
    [Route("api/users/{userId}/[controller]")]
    public class FriendsController: UsersControllerBase
    {
        #region Fields
        private readonly ICardofunRepository _cardofunRepository;
        private readonly IMapper _mapper;
        #endregion Fields

        #region Constructor
        public FriendsController(ICardofunRepository cardofunRepository, IMapper mapper)
        {
            _cardofunRepository = cardofunRepository;
            _mapper = mapper;
        }
        #endregion Constructor

        #region Controller methods
        /// <summary>
        /// Returns list of the user's friends
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserFriends(Int32 userId, [FromQuery]UserFriendParams userFriendParams)
        {
            if(userId != Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            userFriendParams.UserId = userId;

            var userPages = await _cardofunRepository.GetPageOfFriendsAsync(userFriendParams);

            var mappedCollection = _mapper.Map<IEnumerable<User>, IEnumerable<UserForListDto>>(userPages, UserAfterMap);
                
            Response.AddPagination(userPages.PageNumber, userPages.PageSize, userPages.TotalCount, userPages.TotalPages);
            return Ok(mappedCollection);
        }

        /// <summary>
        /// Request a friendship
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="recepientId">Id of a user that ought to be a friend</param>
        /// <returns></returns>
        [HttpPost("{recepientId}")]
        public async Task<IActionResult> RequestFriendship(Int32 userId, Int32 recepientId)
        {
            if(userId != Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            if(userId == recepientId)
                return BadRequest("It is not possible to invite yourself to be friends");

            var friendRequest = await _cardofunRepository.GetFriendRequestAsync(userId, recepientId);

            if(friendRequest != null)
                return BadRequest("The friend request has been already sent before");

            friendRequest = await _cardofunRepository.GetFriendRequestAsync(recepientId, userId);

            if(friendRequest != null)
                return BadRequest("There is a friend request from recepient waiting for reply");

            friendRequest = new FriendRequest
            {
                FromUserId = userId,
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
        /// <param name="userId">User Id</param>
        /// <param name="recepientId">Id of a user that requested the friendship</param>
        /// <param name="friendshipStatus">New friendship status (Requested, Accepted, Declined)</param>
        /// <returns></returns>
        [HttpPut("{recepientId}")]
        public async Task<IActionResult> ReplyOnFriendshipRequest(Int32 userId, Int32 recepientId, [FromBody]FriendshipStatusParams friendshipStatus)
        {
            if(userId != Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            if(userId == recepientId)
                return BadRequest("It's not possible to reply on own friendship request");

            var friendRequest = await _cardofunRepository.GetFriendRequestAsync(recepientId, userId);

            if(friendRequest == null)
                return BadRequest("The friend request has not been found");

            if(!Enum.TryParse(typeof(FriendshipStatus), friendshipStatus.Status, true, out object fStatus))
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
        /// <param name="userId">User Id</param>
        /// <param name="recepientId">Id of a user that will be excluded from friend list</param>
        /// <returns></returns>
        [HttpDelete("{recepientId}")]
        public async Task<IActionResult> RemoveFriendshipRequest(Int32 userId, Int32 recepientId)
        {
            if(userId != Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var request = await _cardofunRepository.GetFriendRequestAsync(userId, recepientId);

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