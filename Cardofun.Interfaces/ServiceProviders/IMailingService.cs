using System.Collections.Generic;
using System.Threading.Tasks;
using Cardofun.Interfaces.DTOs;

namespace Cardofun.Interfaces.ServiceProviders
{
    public interface IMailingService
    {
        Task SendAsync(EmailMessageDto emailMessage);
	    // IEnumerable<EmailMessageDto> ReceiveEmail(int maxCount = 10);
    }
}