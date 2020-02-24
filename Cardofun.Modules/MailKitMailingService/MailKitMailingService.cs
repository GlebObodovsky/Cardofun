using Cardofun.Interfaces.Configurations;
using Microsoft.Extensions.Options;
using Cardofun.Interfaces.ServiceProviders;
using Cardofun.Interfaces.DTOs;
using System;
using MimeKit;
using MailKit.Net.Smtp;
using MimeKit.Text;
using System.Linq;
using System.Threading.Tasks;

namespace Cardofun.Modules.MailKitMailingService
{
    public class MailKitMailingService: IMailingService
    {
        private readonly MailingServiceConfigurations _emailConfiguration;
        public MailKitMailingService(IOptions<MailingServiceConfigurations> mailServiceConfigurations)
        {
            _emailConfiguration = mailServiceConfigurations.Value;
        }

        // public IEnumerable<EmailMessageDto> ReceiveEmail(Int32 maxCount = 10)
        // {
        //     throw new NotImplementedException();
        // }

        public async Task SendAsync(EmailMessageDto emailMessage)
        {
            var message = new MimeMessage();
            message.To.AddRange(emailMessage.ToAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));
            message.From.AddRange(emailMessage.FromAddresses.Select(x => new MailboxAddress(x.Name, x.Address)));

            message.Subject = emailMessage.Subject;

            var textFormat = TextFormat.Html;
            if (Enum.TryParse(typeof(TextFormat), _emailConfiguration.TextFormat, true, out object format))
                textFormat = (TextFormat)format;

            message.Body = new TextPart(textFormat) { Text = emailMessage.Content };

            using (var emailClient = new SmtpClient())
            {
                try
                {
                    // The last parameter here is to use SSL (Which you should when in production!)
                    await emailClient.ConnectAsync(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, true);
                
                    // Remove any OAuth functionality as we won't be using it. 
                    emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                    await emailClient.AuthenticateAsync(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);
                    
                    await emailClient.SendAsync(message);
                }
                finally
                {
                    if (emailClient.IsConnected)
                        emailClient.Disconnect(true);
                }
            }
                    
        }
    }
}