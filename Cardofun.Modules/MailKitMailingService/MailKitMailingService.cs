using Cardofun.Interfaces.Configurations;
using Microsoft.Extensions.Options;
using Cardofun.Interfaces.ServiceProviders;
using Cardofun.Interfaces.DTOs;
using System;
using MimeKit;
using MailKit.Net.Smtp;
using System.Threading.Tasks;
using MailingService.Helpers;
using System.IO;
using Cardofun.Core.NameConstants;
using Microsoft.Extensions.Configuration;

namespace Cardofun.Modules.MailingService
{
    public class MailKitMailingService : IMailingService
    {
        private readonly MailingServiceConfigurations _emailConfiguration;
        private readonly IPhysicalFileService _physicalFileService;
        private readonly IConfiguration _configuration;

        public MailKitMailingService(IOptions<MailingServiceConfigurations> mailServiceConfigurations,
            IConfiguration configuration,
            IPhysicalFileService physicalFileService)
        {
            _configuration = configuration;
            _emailConfiguration = mailServiceConfigurations.Value;
            _physicalFileService = physicalFileService;
        }

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

        public async Task SendConfirmationEmailAsync(UserWithEmailDto user, String token)
        {
            var templatePath = Path.Join(SystemConstants.DocumentTemplates, _emailConfiguration.EmailConfirmationMessageTemplatePath ?? "EmailConfirmationMessageTemplate.html");
            var template = _physicalFileService.ReadAllFile(templatePath);

            // ToDo: specify the default template in case if template == null

            var mainSpaAddress = _configuration.GetSection(AppSettingsConstants.MainSpaApplicationAddress).Get<string>();

            template = template
                .Replace("[UserId]", user.Id.ToString())
                .Replace("[UserName]", user.Name)
                .Replace("[MainSpaAddress]", mainSpaAddress)
                .Replace("[ConfirmationToken]", token);

            var message = new MailMessageBuilder()
                .From(_emailConfiguration.Sender.Name, _emailConfiguration.Sender.Address)
                .To(user.Name, user.Email)
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