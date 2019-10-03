using System.Threading.Tasks;
using Cardofun.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System;
using AutoMapper;
using Cardofun.Interfaces.DTOs;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Query;

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
        /// Gets all awailable users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUsers()
            => Ok(_mapper.Map<IEnumerable<UserForListDto>>(await _cardofunRepository.GetUsersAsync()));

        /// <summary>
        /// Gets a user by the given id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Int32 id)
            => Ok(_mapper.Map<UserForDetailedDto>(await _cardofunRepository.GetUserAsync(id)));
        #endregion Controller methods
    }
}