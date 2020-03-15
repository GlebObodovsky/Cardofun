using System;
using System.Collections.Generic;
using Cardofun.Interfaces.DTOs;
using MimeKit;
using MimeKit.Text;

namespace MailingService.Helpers
{
    public sealed class MailMessageBuilder 
    {
        private MimeMessage _mailMessage = new MimeMessage();

        /// <summary>
        /// Specifies of who is the sender of that mail message
        /// </summary>
        public MailMessageBuilder From(String name, String address)
        {
            _mailMessage.From.Clear();
            _mailMessage.From.Add(new MailboxAddress(name, address));
            return this;
        }

        /// <summary>
        /// Specifies of who is the sender of that mail message
        /// </summary>
        public MailMessageBuilder From(EmailAddressDto address)
            => From(address.Name, address.Address);

        /// <summary>
        /// Adds the address to the list of receivers
        /// </summary>
        public MailMessageBuilder To(String name, String address)
        {
            _mailMessage.To.Add(new MailboxAddress(name, address));
            return this;
        }
        
        /// <summary>
        /// Adds the address to the list of receivers
        /// </summary>
        public MailMessageBuilder To(EmailAddressDto address)
            => To(address.Name, address.Address);

        /// <summary>
        /// Adds the address to the list of receivers
        /// </summary>
        public MailMessageBuilder To(IEnumerable<EmailAddressDto> addresses)
        {
            foreach (var address in addresses)
                To(address);
            return this;
        }

        /// <summary>
        /// Adds the address to the list of receivers who are enlisted in the 'copy' 
        /// </summary>
        public MailMessageBuilder Cc(String name, String address)
        {
            _mailMessage.Cc.Add(new MailboxAddress(name, address));
            return this;
        }

        /// <summary>
        /// Adds the address to the list of receivers who are enlisted in the 'copy' 
        /// </summary>
        public MailMessageBuilder Cc(EmailAddressDto address)
            => Cc(address.Name, address.Address);

        /// <summary>
        /// Adds the address to the list of receivers who are enlisted in the 'copy' 
        /// </summary>
        public MailMessageBuilder Cc(IEnumerable<EmailAddressDto> addresses)
        {
            foreach (var address in addresses)
                Cc(address);
            return this;
        }

        /// <summary>
        /// Specifies the sublect of the mail message
        /// </summary>
        public MailMessageBuilder Subject(string subject)
        {
            _mailMessage.Subject = subject;
            return this;
        } 
        
        /// <summary>
        /// Assigns a body part of the mail message and returns back the builder
        /// </summary>
        /// <param name="body"></param>
        /// <param name="textFormat">Plain / Flowed (rfc3676) / Html / Enriched / CompressedRichText / RichText</param>
        /// <returns></returns>
        public MailMessageBuilder Body(string body, String textFormat = "Html")
        {
            TextFormat format = TextFormat.Html;

            if (Enum.TryParse(typeof(TextFormat), textFormat, true, out object objFormat))
                format = (TextFormat)objFormat;

            _mailMessage.Body = new TextPart(textFormat) { Text = body };

            return this;
        }

        /// <summary>
        /// Returns the mail message and flushes the building mail message object
        /// </summary>
        /// <returns></returns>
        public MimeMessage Build()
        {
            if (_mailMessage.From.Count == 0)
                throw new InvalidOperationException("Can't create a mail message with empty From. Please call 'From' method first");

            if (_mailMessage.To.Count == 0)
                throw new InvalidOperationException("Can't create a mail message with empty To. Please call 'To' method first");

            var message = _mailMessage;

            _mailMessage = new MimeMessage();

            return message;
        }
    }
}