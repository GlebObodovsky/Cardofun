using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cardofun.Interfaces.DTOs;

namespace Cardofun.Interfaces.ServiceProviders
{
    public interface IMailingService
    {
        /// <summary>
        /// Sends an email to users
        /// </summary>
        Task SendAsync(EmailMessageDto emailMessage);

        /// <summary>
        /// Sends a confirmation email to the specified user.
        /// It uses a special template which path is specified under EmailConfirmationMessageTemplatePath
        /// in the config section of MailingServiceSettings
        /// </summary>
        /// <param name="user">The user who has to be verified</param>
        /// <param name="token">The token that the user has to deliver to the server in order to get his email confirmed</param>
        /// <returns></returns>
        Task SendConfirmationEmailAsync(EmailAddressDto user, String token);
	    // IEnumerable<EmailMessageDto> ReceiveEmail(int maxCount = 10);
    }
}