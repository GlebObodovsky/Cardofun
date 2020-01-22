using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Cardofun.Core.NameConstants;
using Cardofun.Domain.Models;
using Cardofun.Interfaces.DTOs;
using Cardofun.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cardofun.API.Controllers
{
    [Authorize(Policy = PolicyConstants.ModeratorRoleRequired)]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController: ControllerBase
    {
        #region Fields
        private readonly ICardofunRepository _cardofunRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        #endregion Fields

        #region Constructor
        public AdminController(ICardofunRepository cardofunRepository, UserManager<User> userManager, IMapper mapper)
        {
            _cardofunRepository = cardofunRepository;
            _userManager = userManager;
            _mapper = mapper;
        }
        #endregion Constructor

        [HttpGet("usersWithRoles/{userName}")]
        public async Task<IActionResult> GetUsersWithRoles(String userName)
        {
            var user = await _cardofunRepository.GetUserWithRolesByNameAsync(userName);

            if (user != null)
                return Ok(_mapper.Map<User, UserForAdminPanelDto>(user));

            return NotFound();
        }

        [HttpPost("editRoles/{userName}")]
        public async Task<IActionResult> EditRoles(String userName, RoleEditDto roleEdit)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
                return BadRequest($"The user with name {userName} doesn't exist");

            var userRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = roleEdit.RoleNames ?? new String[] {};
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded)
                return BadRequest("Failed to add new roles!");

            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded)
                return BadRequest("Failed to remove existing roles!");
                
            return Ok(await _userManager.GetRolesAsync(user));
        }

        [HttpGet("messagesForModeration")]
        public IActionResult GetMessagesForModeration()
        {
            return Ok("Admin and moderators can see this");
        }

        [HttpGet("photosForModeration")]
        public IActionResult GetPhotosForModeration()
        {
            return Ok("Admin and moderators can see this");
        }
    }
}