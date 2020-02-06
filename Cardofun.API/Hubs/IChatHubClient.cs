using System.Threading.Tasks;

namespace Cardofun.API.Hubs
{
    public interface IChatHubClient
    {
        Task ReceiveMessage(string message);
    }
}