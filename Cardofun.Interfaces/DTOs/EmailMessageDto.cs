using System;
using System.Collections.Generic;

namespace Cardofun.Interfaces.DTOs
{
    public class EmailMessageDto
    {
        public IEnumerable<EmailAddressDto> ToAddresses { get; set; }
        public String Subject { get; set; }
        public String Content { get; set; }
    }
}