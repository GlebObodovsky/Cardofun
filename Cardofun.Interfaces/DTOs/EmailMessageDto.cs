using System;
using System.Collections.Generic;

namespace Cardofun.Interfaces.DTOs
{
    public class EmailMessageDto
    {
        public String Subject { get; set; }
        public String Content { get; set; }
        public IEnumerable<EmailAddressDto> ToAddresses { get; set; }
        public IEnumerable<EmailAddressDto> FromAddresses { get; set; }
    }
}