using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cardofun.Domain.Models;
using Cardofun.Interfaces.Repositories;
using Cardofun.Interfaces.DTOs;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Cardofun.Core.NameConstants;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace Cardofun.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController: ControllerBase
    {
        #region Fields
        private readonly IAuthRepository _authRepoitory;
        private readonly IConfiguration _config;
        #endregion Fields

        #region  Constructor
        public AuthController(IAuthRepository authRepoitory, IConfiguration config)
        {
            _authRepoitory = authRepoitory;
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
            if(await _authRepoitory.IsExistAsync(userForRegister.Login))
                return BadRequest("Login already exists");
            
            var newUser = new User
            {
                Login = userForRegister.Login
            };

            var registeredUser = await _authRepoitory.RegisterAsync(newUser, userForRegister.Password);

            return StatusCode(201);
        }
        /// <summary>
        /// Allows a user to login to the service
        /// </summary>
        /// <returns></returns>
        [HttpPost(nameof(Login))]
        public async Task<IActionResult> Login(UserForLoginDto userForLogin)
        {
            var userFromRepo = await _authRepoitory.LoginAsync(userForLogin.Login, userForLogin.Password);
        
            if(userFromRepo == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Login)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection(AppSettingsConstants.Token).Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenExpirationTime = _config.GetSection(AppSettingsConstants.TokenExpiresInHours).Value;

            if(!int.TryParse(tokenExpirationTime, out int addHours))
                addHours = 24;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(addHours),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }
        #endregion Controller methods
    }
}