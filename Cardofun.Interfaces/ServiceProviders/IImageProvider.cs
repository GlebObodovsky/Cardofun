using System;
using System.Threading.Tasks;
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
        Task<GlobalPhotoIdentifiersDto> SavePictureAsync(IFormFile picture);

        /// <summary>
        /// Removes the picture from a storage
        /// </summary>
        /// <param name="publicId">Picture's public Id. Depends on the underlying image provider,
        /// but in general it can be anything. Starting from url or token, 
        /// ending with physical path to the picture on a certain file system server</param>
        /// <returns></returns>
        Task<Boolean> DeletePictureAsync(string publicId);
    }
}