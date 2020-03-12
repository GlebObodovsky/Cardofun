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
using Cardofun.Core.NameConstants;
using Microsoft.AspNetCore.Identity;

namespace Cardofun.API.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : UsersControllerBase
    {
        #region Fields
        private readonly ICardofunRepository _cardofunRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        #endregion Fields

        #region Constructor
        public UsersController(ICardofunRepository cardofunRepository, UserManager<User> userManager, IMapper mapper)
        {
            _cardofunRepository = cardofunRepository;
            _userManager = userManager;
            _mapper = mapper;
        }
        #endregion Constructor

        #region Controller methods
        /// <summary>
        /// Checks if user with the given login already exists
        /// </summary>
        /// <returns></returns>
        [HttpHead("userNames/{login}")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckIfUserNameExists(String login)
        {
            if (await _cardofunRepository.CheckIfUserNameExists(login))
                return Ok();

            return NotFound();
        }

        /// <summary>
        /// Checks if user with the given email already exists
        /// </summary>
        /// <returns></returns>
        [HttpHead("emails/{email}")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckIfEmailExists(String email)
        {
            if (await _cardofunRepository.CheckIfEmailExists(email))
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
            var mappedCollection = _mapper.Map<IEnumerable<User>, IEnumerable<UserForListDto>>(userPages, UserAfterMap);

            Response.AddPagination(userPages.PageNumber, userPages.PageSize, userPages.TotalCount, userPages.TotalPages);
            return Ok(mappedCollection);
        }

        /// <summary>
        /// Gets a user by the given id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}", Name = nameof(GetUser))]
        public async Task<IActionResult> GetUser(Int32 id)
        {
            var user = await _cardofunRepository.GetUserAsync(id);

            if (user != null)
                return Ok(_mapper.Map<UserForDetailedDto>(user));

            return NotFound();
        }

        public class Ololol 
        {
            public String Token { get; set; }
        }

        /// <summary>
        /// Allows a user to confirm his email
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="Token"></param>
        /// <returns></returns>
        [HttpPost("{userId}/verify")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyUser(Int32 userId, [FromBody]Ololol token)
        {
            var user = await _cardofunRepository.GetUserAsync(userId);

            if (user == null)
                return BadRequest("The user hasn't been found");

            var result = await _userManager.ConfirmEmailAsync(user, token.Token);

            if (result.Succeeded)
                return Ok();

            return BadRequest("Confirmation token is invalid");
        }

        /// <summary>
        /// Updates a user by the given id
        /// </summary>
        /// <returns></returns>
        [HttpPut("{userId}")]
        [Authorize(Policy = PolicyConstants.UserMatchRequired)]
        public async Task<IActionResult> UpdateUser(Int32 userId, UserForUpdateDto newUserInfo)
        {
            // If there is any learning language that is the same as speaking
            if (newUserInfo.LanguagesTheUserLearns.Any(ll => newUserInfo.LanguagesTheUserSpeaks.Any(sl => sl.Code.Equals(ll.Code))))
                return BadRequest("One cannot speak and learn same language");

            _cardofunRepository.StartTransaction();

            var user = await _cardofunRepository.GetUserAsync(userId);

            // checking if there's a language that the user was learning before but is speaking now
            var anyUpgraded = user.LanguagesTheUserLearns.Any(oldLearningLang => newUserInfo.LanguagesTheUserSpeaks.Any(newSpeakingLang => oldLearningLang.LanguageCode.Equals(newSpeakingLang.Code)));
            // checking if there's a language that the user was speaking before but is learning now
            var anyDowngraded = user.LanguagesTheUserSpeaks.Any(oldSpeakingLang => newUserInfo.LanguagesTheUserLearns.Any(newLearningLang => oldSpeakingLang.LanguageCode.Equals(newLearningLang.Code)));
            // if any of above is true - clearing up old values in order not to get 
            // unique constraint error while saving after new values are applied
            if (anyUpgraded || anyDowngraded)
            {
                user.LanguagesTheUserLearns = null;
                user.LanguagesTheUserSpeaks = null;
                await _cardofunRepository.SaveChangesAsync();
            }

            _mapper.Map<UserForUpdateDto, User>(newUserInfo, user);

            if (await _cardofunRepository.SaveChangesAsync())
            {
                _cardofunRepository.CommitTransaction();
                return NoContent();
            }

            return BadRequest("Updating user failed on save");
        }
        #endregion Controller methods
    }
}