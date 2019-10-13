using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Cardofun.Domain.Models;
using Cardofun.Interfaces.DTOs;
using Cardofun.Interfaces.Repositories;
using Cardofun.Interfaces.ServiceProviders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cardofun.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        #region Fields
        private readonly ICardofunRepository _cardofunRepository;
        private readonly IMapper _mapper;
        private readonly IImageProvider _imageProvider;
        #endregion Fields
        
        #region Constructor
        public PhotosController(ICardofunRepository cardofunRepository, IMapper mapper, IImageProvider imageProvider)
        {
            _mapper = mapper;
            _cardofunRepository = cardofunRepository;
            _imageProvider = imageProvider;
        }
        #endregion Constructor

        #region Controller methods
        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(Guid id)
        {
            var photo = await _cardofunRepository.GetPhoto(id);

            if(photo == null)
                return BadRequest("The requested photo hasn't been found");

            return Ok(_mapper.Map<PhotoForReturnDto>(photo));
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(Int32 userId, 
            [FromForm]PhotoForCreationDto photoForCreation)
        {
            if(userId != Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await _cardofunRepository.GetUserAsync(userId);

            var photoIdentifiers = _imageProvider.SavePicture(photoForCreation.File);
            photoForCreation.Url = photoIdentifiers.Url;
            photoForCreation.PublicId = photoIdentifiers.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreation);
            // If there's no main photo in user's profile - set this one as main
            photo.IsMain = !user.Photos.Any(p => p.IsMain);

            user.Photos.Add(photo);

            if(await _cardofunRepository.SaveChangesAsync())
            {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute(nameof(GetPhoto), new { id = photo.Id }, photoToReturn);
            }

            return BadRequest("Could not add the photo");
        }
        #endregion Controller methods
    }
}