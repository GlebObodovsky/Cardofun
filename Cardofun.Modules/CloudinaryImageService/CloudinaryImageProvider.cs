using Cardofun.Interfaces.ServiceProviders;
using CloudinaryDotNet;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Cardofun.Interfaces.DTOs;
using CloudinaryDotNet.Actions;
using System;
using System.Threading.Tasks;
using Cardofun.Interfaces.Configurations;

namespace Cardofun.Modules.CloudinaryImageService
{
    /// <summary>
    /// An image manager based on Cloudinary
    /// </summary>
    public class CloudinaryImageService: IImageService
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;
        public CloudinaryImageService(IOptions<ImageServiceConfigurations> providerSettings)
        {
            // Creating an account to access the service
            Account acc = new Account(
                providerSettings.Value.CloudName,
                providerSettings.Value.ApiKey,
                providerSettings.Value.ApiSecret
            );

            // Access the service
            _cloudinary = new CloudinaryDotNet.Cloudinary(acc);
        }

        /// <summary>
        /// Saves a given picture to a storage, cropping it and resizing along the process
        /// </summary>
        /// <param name="file">The picture to save</param>
        /// <returns>Identifiers of the picture after it has been saved</returns>
        public async Task<GlobalPhotoIdentifiersDto> SavePictureAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file != null && file.Length > 0)
            {
                await Task.Run(() => 
                {
                    using (var stream = file.OpenReadStream())
                    {
                        var param = new ImageUploadParams 
                        {
                            File = new FileDescription(file.Name, stream),
                            Transformation = new Transformation()
                                .Width(500).Height(500).Crop("fill").Gravity("face")                  
                        };

                        uploadResult = _cloudinary.Upload(param);
                    }
                });
            }
            else
                throw new ArgumentException("The file hasn't been found in request");

            return new GlobalPhotoIdentifiersDto { Url = uploadResult.Uri.ToString(), PublicId = uploadResult.PublicId };
        }

        /// <summary>
        /// Removes the picture from a storage
        /// </summary>
        /// <param name="publicId">PublicId on cloudinary service</param>
        /// <returns></returns>
        public async Task<Boolean> DeletePictureAsync(string publicId)
        {
            if(String.IsNullOrWhiteSpace(publicId))
                throw new ArgumentNullException("PublicId hasn't been provided for the Cloudinary service destroy method");

            var isDeleted = false;

            await Task.Run(() => 
            {
                isDeleted = _cloudinary.Destroy(new DeletionParams(publicId)).Result == "ok";
            });

            return isDeleted;
        }
    }
}
