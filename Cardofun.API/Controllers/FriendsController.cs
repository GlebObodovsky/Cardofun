using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Cardofun.API.Helpers.Extensions;
using Cardofun.API.Hubs;
using Cardofun.Core.ApiParameters;
using Cardofun.Core.Enums;
using Cardofun.Core.NameConstants;
using Cardofun.Domain.Models;
using Cardofun.Interfaces.DTOs;
using Cardofun.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Cardofun.API.Controllers
{
    [Route("api/users/{userId}/[controller]")]
    [Authorize(Policy = PolicyConstants.UserMatchRequired)]
    public class FriendsController: UsersControllerBase
    {
        #region Fields
        private readonly ICardofunRepository _cardofunRepository;
        private readonly IMapper _mapper;
        private readonly IHubContext<FriendsHub, IFriendsHubClient> _frinedHub;
        private readonly IHubContext<NotificationsHub, INotificationsHubClient> _notificationsHub;
        #endregion Fields

        #region Constructor
        public FriendsController(ICardofunRepository cardofunRepository, IMapper mapper,
            IHubContext<FriendsHub, IFriendsHubClient> frinedHub, IHubContext<NotificationsHub, INotificationsHubClient> notificationsHub)
        {
            _cardofunRepository = cardofunRepository;
            _mapper = mapper;
            _frinedHub = frinedHub;
            _notificationsHub = notificationsHub;
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
            userFriendParams.UserId = userId;

            var userPages = await _cardofunRepository.GetPageOfFriendsAsync(userFriendParams);

            var mappedCollection = _mapper.Map<IEnumerable<User>, IEnumerable<UserForListDto>>(userPages, UserAfterMap);
                
            Response.AddPagination(userPages.PageNumber, userPages.PageSize, userPages.TotalCount, userPages.TotalPages);
            return Ok(mappedCollection);
        }

        /// <summary>
        /// Gets a friendship request
        /// </summary>
        /// <param name="userId">Id of a user that is trying to get the friendship request</param>
        /// <param name="secondUserId">Id of the friendship request</param>
        /// <returns></returns>
        [HttpGet("{secondUserId}", Name = nameof(GetFriendshipRequest))]
        public async Task<IActionResult> GetFriendshipRequest(Int32 userId, Int32 secondUserId)
        {
            var friendshipFromRepo = await _cardofunRepository.GetFriendRequestAsync(userId, secondUserId);

            if (friendshipFromRepo == null)
                friendshipFromRepo = await _cardofunRepository.GetFriendRequestAsync(secondUserId, userId);

            if (friendshipFromRepo == null)
                return NotFound();
                
            return Ok(_mapper.Map<FriendshipRequestStatusDto>(friendshipFromRepo));
        }

        [HttpGet("countOfFollowers")]
        public async Task<IActionResult> GetCountOfFollowers(Int32 userId)
            => Ok(await _cardofunRepository.GetCountOfFollowersAsync(userId));

        /// <summary>
        /// Request a friendship
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="recepientId">Id of a user that ought to be a friend</param>
        /// <returns></returns>
        [HttpPost("{recepientId}")]
        public async Task<IActionResult> RequestFriendship(Int32 userId, Int32 recepientId)
        {
            if(userId == recepientId)
                return BadRequest("It is not possible to request yourself for friendship");

            var friendRequest = await _cardofunRepository.GetFriendRequestAsync(userId, recepientId);

            if(friendRequest != null)
                return BadRequest("The friend request has been already sent before");

            friendRequest = await _cardofunRepository.GetFriendRequestAsync(recepientId, userId);

            if(friendRequest != null)
                return BadRequest("There is a friend request from recepient waiting for reply");

            if(await CreateFriendshipRequestAsync(userId, recepientId))
            {
                friendRequest = await _cardofunRepository.GetFriendRequestAsync(userId, recepientId);
                return CreatedAtRoute(nameof(GetFriendshipRequest),
                    new { userId, secondUserId = recepientId }, _mapper.Map<FriendshipRequestStatusDto>(friendRequest));
            }
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

            if(!await _cardofunRepository.SaveChangesAsync())
                return BadRequest("Changing friendship status failed on save");
                
            await NotifyUsersAboutFrinedshipStatusAsync(friendRequest);

            return Ok();
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
            var request = await _cardofunRepository.GetFriendRequestAsync(userId, recepientId);

            if(request == null)
                return BadRequest("The friend request hasn't been found");

            var oldStatus = request.Status;

            _cardofunRepository.Delete(request);

            if(!await _cardofunRepository.SaveChangesAsync())
                return BadRequest("Friendship request deletion failed on save");
            
            if (oldStatus == FriendshipStatus.Accepted)
                // If the owner of the friendship request has changed
                await CreateFriendshipRequestAsync(recepientId, userId);    
            else
                // If the friendship was deleted completly
                await NotifyUsersAboutFrinedshipStatusAsync(request, true);
            
            return Ok();
        }
        #endregion Controller methods

        #region Functions
        /// <summary>
        /// Creates a new friendship request
        /// </summary>
        /// <param name="fromUserId">The user who requests the friendship</param>
        /// <param name="toUserId">The user who'll get the request</param>
        /// <returns>True - If the request was created successfully</returns>
        private async Task<Boolean> CreateFriendshipRequestAsync(Int32 fromUserId, Int32 toUserId)
        {
            var friendRequest = new FriendRequest
            {
                FromUserId = fromUserId,
                ToUserId = toUserId,
            };
            _cardofunRepository.Add(friendRequest);
            if (!await _cardofunRepository.SaveChangesAsync())
                return false;
            
            await NotifyUsersAboutFrinedshipStatusAsync(friendRequest);

            return true;
        }
        #endregion Functions

        #region SignalR
        /// <summary>
        /// Notifies the given user that the amount of users following him has changed
        /// </summary>
        // /// <param name="recepientId"></param>
        private async Task NotifyUserAboutFollowersCountAsync(Int32 recepientId)
        {
            var countOfFollowers = await _cardofunRepository.GetCountOfFollowersAsync(recepientId);
            await _notificationsHub.Clients.User(recepientId.ToString()).ReceiveFollowersCount(countOfFollowers);
        }

        private async Task NotifyUsersAboutFrinedshipStatusAsync(FriendRequest request, Boolean isDeleted = false)
        {
            if (request == null)
                return;
            
            await NotifyUserAboutFollowersCountAsync(request.ToUserId);

            var requestToReturn = _mapper.Map<FriendshipRequestStatusDto>(request);
            requestToReturn.IsDeleted = isDeleted;

            await _frinedHub.Clients.Users(request.FromUserId.ToString(), request.ToUserId.ToString())
                .ReceiveFriendshipStatus(requestToReturn);

            if (!isDeleted)
                await NotifyUsersAboutNewOrAcceptedFriendship(request);
        }
        
        /// <summary>
        /// Notifies both, requesting and receiving the invitation users that the new friendship request has been created
        /// </summary>
        /// <param name="friendRequest"></param>
        private async Task NotifyUsersAboutNewOrAcceptedFriendship(FriendRequest friendRequest)
        {
            if (friendRequest == null)
                return;

            // Notify requesting user
            var toUser = friendRequest.ToUser != null
                ? _mapper.Map<UserForListDto>(friendRequest.ToUser)
                : _mapper.Map<UserForListDto>(await _cardofunRepository.GetUserAsync(friendRequest.ToUserId));
            toUser.Friendship = new FriendshipRequestDto 
            {
                IsOwner = false,
                Status = friendRequest.Status
            };

            if (friendRequest.Status != FriendshipStatus.Accepted)
                await _frinedHub.Clients.User(friendRequest.FromUserId.ToString()).ReceiveOutgoingFriendshipRequest(toUser);
            else
                await _frinedHub.Clients.User(friendRequest.FromUserId.ToString()).ReceiveAcceptedFriendship(toUser);
                
            // Notify receiving user
            var fromUser = friendRequest.FromUser != null
                ? _mapper.Map<UserForListDto>(friendRequest.FromUser)
                : _mapper.Map<UserForListDto>(await _cardofunRepository.GetUserAsync(friendRequest.FromUserId));
            fromUser.Friendship = new FriendshipRequestDto 
            {
                IsOwner = true,
                Status = friendRequest.Status
            };

            if (friendRequest.Status != FriendshipStatus.Accepted)
                await _frinedHub.Clients.User(friendRequest.ToUserId.ToString()).ReceiveIncommingFriendshipRequest(fromUser);
            else
                await _frinedHub.Clients.User(friendRequest.ToUserId.ToString()).ReceiveAcceptedFriendship(fromUser);
        }
        #endregion SignalR
    }
}