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
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace Cardofun.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        #region Fields
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        #endregion Fields

        #region  Constructor
        public AuthController(IAuthRepository authRepoitory, IConfiguration config, IMapper mapper)
        {
            _mapper = mapper;
            _authRepository = authRepoitory;
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
            if (await _authRepository.IsExistAsync(userForRegister.UserName))
                return BadRequest("Login already exists");

            var newUser = _mapper.Map<User>(userForRegister);

            var registeredUser = await _authRepository.RegisterAsync(newUser, userForRegister.Password);

            var userForReturn = _mapper.Map<UserShortInfoDto>(newUser);

            return CreatedAtRoute("GetUser", new { Controller = "Users", Id = newUser.Id }, userForReturn);
        }
        /// <summary>
        /// Allows a user to login to the service
        /// </summary>
        /// <returns></returns>
        [HttpPost(nameof(Login))]
        public async Task<IActionResult> Login(UserForLoginDto userForLogin)
        {
            var userFromRepo = await _authRepository.LoginAsync(userForLogin.UserName, userForLogin.Password);

            if (userFromRepo == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.UserName)
            };

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

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user = _mapper.Map<UserShortInfoDto>(userFromRepo)
            });
        }
        #endregion Controller methods
    }
}