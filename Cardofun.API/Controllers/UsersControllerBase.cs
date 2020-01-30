using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using AutoMapper;
using Cardofun.Domain.Models;
using Cardofun.Interfaces.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Cardofun.API.Controllers
{
    [ApiController]
    public abstract class UsersControllerBase: ControllerBase
    {
        /// <summary>
        /// Sets aftermap otions for collection of UserForListDto's. 
        /// Main purpose of which is to add friendship status information
        /// </summary>
        /// <param name="opt"></param>
        protected internal void UserAfterMap(IMappingOperationOptions<IEnumerable<User>, IEnumerable<UserForListDto>> opt)
        {
            if (!Int32.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Int32 id))
                return;

            opt.AfterMap((src, dest) =>
            {
                for (int i = 0; i < dest.Count(); i++)
                {
                    var elementFrom = src.ElementAt(i);
                    var elementTo = dest.ElementAt(i);
                    var outcomming = elementFrom.OutcomingFriendRequests.FirstOrDefault(ofr => ofr.ToUserId == id);
                    var incomming = elementFrom.IncomingFriendRequests.FirstOrDefault(ofr => ofr.FromUserId == id);
                    
                    if (outcomming == null && incomming == null)
                        continue;
                        
                    elementTo.Friendship = new FriendshipRequestDto 
                    {
                        IsOwner = outcomming != null,
                        Status = outcomming != null ? outcomming.Status : incomming.Status
                    };
                }
            });
        }
    }
}