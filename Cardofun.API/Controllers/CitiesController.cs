using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Cardofun.Interfaces.DTOs;
using Cardofun.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cardofun.API.Controllers
{
    // [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController: ControllerBase
    {
        #region Fields
        private readonly ICardofunRepository _cardofunRepository;
        private readonly IMapper _mapper;
        #endregion Fields

        #region Constructor
        public CitiesController(ICardofunRepository cardofunRepository, IMapper mapper)
        {
            _cardofunRepository = cardofunRepository;
            _mapper = mapper;
        }
        #endregion Constructor

        #region Controller methods
        /// <summary>
        /// Gets languages by specified search pattern
        /// </summary>
        /// <returns></returns>
        [HttpGet("{searchBy}")]
        public async Task<IActionResult> GetCountries(String searchBy)
            => Ok(_mapper.Map<IEnumerable<CityDto>>(await _cardofunRepository.GetCitiesAsync(searchBy)));
        #endregion Controller methods
    }
}