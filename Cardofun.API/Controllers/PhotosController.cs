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
    [Route("api/users/{userId}/[controller]")]
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
        [HttpGet("{id}", Name = nameof(GetPhoto))]
        public async Task<IActionResult> GetPhoto(Guid id)
        {
            var photo = await _cardofunRepository.GetUserPhotoAsync(id);

            if(photo == null)
                return NotFound();

            return Ok(_mapper.Map<UserPhotoForReturnDto>(photo));
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(Int32 userId, 
            [FromForm]UserPhotoForCreationDto photoForCreation)
        {
            if(userId != Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await _cardofunRepository.GetUserAsync(userId);

            var photoIdentifiers = await _imageProvider.SavePictureAsync(photoForCreation.File);
            photoForCreation.Url = photoIdentifiers.Url;
            photoForCreation.PublicId = photoIdentifiers.PublicId;

            var photo = _mapper.Map<UserPhoto>(photoForCreation);
            // If there's no main photo in user's profile - set this one as main
            photo.IsMain = !user.Photos.Any(p => p.IsMain);

            user.Photos.Add(photo);

            if(!await _cardofunRepository.SaveChangesAsync())
                return BadRequest("Could not add the photo");

            var photoToReturn = _mapper.Map<UserPhotoForReturnDto>(photo);
            return CreatedAtRoute(nameof(GetPhoto), new { userId = userId, id = photo.Id }, photoToReturn);
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(Int32 userId, Guid id)
        {
            if(userId != Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var mainPhoto = await _cardofunRepository.GetMainPhotoForUserAsync(userId);
            var newMainPhoto = await _cardofunRepository.GetUserPhotoAsync(id);

            if(newMainPhoto == null)
                return BadRequest("Could not find the photo");

            if(newMainPhoto.UserId != userId)
                return Unauthorized();

            if(newMainPhoto.IsMain)
                return BadRequest("The photo is already main");

            if(mainPhoto != null)
                mainPhoto.IsMain = false;
            
            // Wrapping it with transaction because of circular dependency of IsMain property.
            // As the property has Unique constraint for there might me only one main pic for each user
            // It'll crush if saving both at a time
            _cardofunRepository.StartTransaction();

            await _cardofunRepository.SaveChangesAsync();

            newMainPhoto.IsMain = true;

            if(await _cardofunRepository.SaveChangesAsync())
            {
                _cardofunRepository.CommitTransaction();
                return NoContent();
            }
            
            return BadRequest("Could not set the photo as main");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(Int32 userId, Guid id)
        {
            if(userId != Int32.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var photoToRemove = await _cardofunRepository.GetUserPhotoAsync(id);
                        
            if(photoToRemove == null)
                return BadRequest("The photo does not exist");

            if(photoToRemove.UserId != userId)
                return Unauthorized();

            if(photoToRemove.IsMain)
                return BadRequest("You cannot delete your main photo");

            // In case if the photo doesn't have PublicId - we are proceeding with
            // removing it just out of the repository
            var canProceed = String.IsNullOrWhiteSpace(photoToRemove?.Photo.PublicId)
                // othervise - we're removing it from the image provider storage
                || await _imageProvider.DeletePictureAsync(photoToRemove?.Photo.PublicId);

            if(canProceed)
            {
                _cardofunRepository.Delete(photoToRemove);
                canProceed = await _cardofunRepository.SaveChangesAsync();
            }
            
            if(canProceed)
                return Ok();
            
            return BadRequest("Failed to delete the photo");
        }
        #endregion Controller methods
    }
}