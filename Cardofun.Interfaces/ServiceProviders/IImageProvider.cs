using Cardofun.Interfaces.DTOs;
using Microsoft.AspNetCore.Http;

namespace Cardofun.Interfaces.ServiceProviders
{
    /// <summary>
    /// Abstract image storage
    /// </summary>
    public interface IImageProvider
    {
        /// <summary>
        /// Saves a given picture to a storage, cropping it and resizing along the process
        /// </summary>
        /// <param name="file">The picture to save</param>
        /// <returns>Identifiers of the picture after it has been saved</returns>
        GlobalPhotoIdentifiersDto SavePicture(IFormFile picture); 
    }
}