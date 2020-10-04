using Cardofun.Infrastructure.ServiceDefinitions.Repositories.LocationRepository;
using Cardofun.Infrastructure.ServiceDefinitions.Repositories.LocationRepository.Messages;
using Grpc.Core;

namespace Cardofun.Repository.Remote.Location
{
    public class LocationRepository // ToDo: Implement ILocationRepository interface
    {
        private readonly Channel _channel;
        private readonly LocationRepo.LocationRepoClient _client;
        public LocationRepository()
        {
            _channel = new Channel("127.0.0.1", 5000, ChannelCredentials.Insecure);
            _client = new LocationRepo.LocationRepoClient(_channel);
        }

        // ToDo: Implement a public method that would return a list of countries by a string input
    }
}