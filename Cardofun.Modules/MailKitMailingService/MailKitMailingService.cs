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
using MailingService.Helpers;
using System.IO;
using Cardofun.Core.NameConstants;

namespace Cardofun.Modules.MailingService
{
    public class MailKitMailingService: IMailingService
    {
        private readonly MailingServiceConfigurations _emailConfiguration;
        private readonly IPhysicalFileService _physicalFileService;

        public MailKitMailingService(IOptions<MailingServiceConfigurations> mailServiceConfigurations, 
            IPhysicalFileService physicalFileService)
        {
            _emailConfiguration = mailServiceConfigurations.Value;
            _physicalFileService = physicalFileService;
        }

        // public IEnumerable<EmailMessageDto> ReceiveEmail(Int32 maxCount = 10)
        // {
        //     throw new NotImplementedException();
        // }

        public async Task SendAsync(EmailMessageDto emailMessage)
        {
            var message = new MailMessageBuilder()
                .From(_emailConfiguration.Sender.Name, _emailConfiguration.Sender.Address)
                .To(emailMessage.ToAddresses)
                .Subject(emailMessage.Subject)
                .Body(emailMessage.Content)
                .Build();

            await SendMessageAsync(message);
        }

        public async Task SendConfirmationEmailAsync(EmailAddressDto user, String token)
        {
            var templatePath = Path.Join(SystemConstants.DocumentTemplates, _emailConfiguration.EmailConfirmationMessageTemplatePath ?? "EmailConfirmationMessageTemplate.html");
            var template = _physicalFileService.ReadAllFile(templatePath);

            // ToDo: specify the default template in case if template == null

            token = token.Insert(token.Length / 3, @"<br/>");
            token = token.Insert((token.Length / 3) * 2, @"<br/>");

            template = template.Replace("[UserName]", user.Name).Replace("[ConfirmationToken]", token);

            var message = new MailMessageBuilder()
                .From(_emailConfiguration.Sender.Name, _emailConfiguration.Sender.Address)
                .To(user)
                .Subject("Confirm your email address")
                .Body(template)
                .Build();

            await SendMessageAsync(message);
        }

        private async Task SendMessageAsync(MimeMessage message)
        {
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