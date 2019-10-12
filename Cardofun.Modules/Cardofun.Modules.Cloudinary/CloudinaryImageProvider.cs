using Cardofun.Interfaces.ServiceProviders;
using Microsoft.Extensions.Options;

namespace Cardofun.Modules.Cloudinary
{
    public class CloudinaryImageProvider: IImageProvider
    {
        private readonly CloudinaryProviderSettings _providerSettings;

        public CloudinaryImageProvider(IOptions<CloudinaryProviderSettings> providerSettings)
        {
            _providerSettings = providerSettings.Value;
        }
    }
}
