using System.Threading.Tasks;
using Cardofun.Interfaces.DTOs;

namespace Cardofun.API.Hubs
{
    public interface IChatHubClient
    {
        Task ReceiveMessage(MessageExtendedDto message);
        Task MarkMessageAsRead(ReadMessagesListDto readMessages);
    }
}