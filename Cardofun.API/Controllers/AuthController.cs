using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cardofun.Domain.Models;
using Cardofun.Interfaces.Repositories;
using Cardofun.Interfaces.DTOs;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using Cardofun.API.Helpers.Constants;

namespace Cardofun.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        #region Fields
        private readonly ICardofunRepository _cardofunRepoitory;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        #endregion Fields

        #region  Constructor
        public AuthController(ICardofunRepository cardofunRepoitory, UserManager<User> userManager,
            SignInManager<User> signInManager, IConfiguration config, IMapper mapper)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _mapper = mapper;
            _cardofunRepoitory = cardofunRepoitory;
            _config = config;
        }
        #endregion  Constructor

        #region Controller methods
        /// <summary>
        /// Allows a new user to register in the service
        /// </summary>
        /// <returns></returns>
        [HttpPost(nameof(Register))]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegister)
        {
            var newUser = _mapper.Map<User>(userForRegister);

            var result = await _userManager.CreateAsync(newUser, userForRegister.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return CreatedAtRoute("GetUser", new { Controller = "Users", Id = newUser.Id }, _mapper.Map<UserShortInfoDto>(newUser));
        }

        /// <summary>
        /// Allows a user to login to the service
        /// </summary>
        /// <returns></returns>
        [HttpPost(nameof(Login))]
        public async Task<IActionResult> Login(UserForLoginDto userForLogin)
        {
            var user = await LoginAsync(userForLogin.UserName, userForLogin.Password);

            if (user == null)
                return Unauthorized();

            return Ok(new
            {
                token = await GenerateJwtToken(user),
                user = _mapper.Map<UserShortInfoDto>(user)
            });
        }
        #endregion Controller methods

        #region Functions
        /// <summary>
        /// Allows user to login with his/her credentials
        /// </summary>
        /// <param name="userName">User's login</param>
        /// <param name="password">User's password</param>
        /// <returns>Authenticated user</returns>        
        private async Task<User> LoginAsync(string userName, string password)
        {
            var user = await _cardofunRepoitory.GetUserByNameAsync(userName);

            if (user == null)
                return null;

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

            if (!result.Succeeded)
                return null;

            return user;
        }

        private async Task<String> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
            };

            var userRoles = await _userManager.GetRolesAsync(user);
    
            claims.AddRange(userRoles.Select(ur => new Claim(ClaimTypes.Role, ur)));

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection(AppSettingsConstants.Token).Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenExpirationTime = _config.GetSection(AppSettingsConstants.TokenExpiresInHours).Value;

            if (!int.TryParse(tokenExpirationTime, out int addHours))
                addHours = 24;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(addHours),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
        }
        #endregion Functions
    }
}