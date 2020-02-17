using System;
using System.Threading.Tasks;
using Cardofun.Interfaces.DTOs;

namespace Cardofun.API.Hubs
{
    public interface IChatHubClient
    {
        Task ReceiveUnreadMessagesCount(Int32 count);
        Task ReceiveMessage(MessageExtendedDto message);
        Task MarkMessageAsRead(ReadMessagesListDto readMessages);
    }
}