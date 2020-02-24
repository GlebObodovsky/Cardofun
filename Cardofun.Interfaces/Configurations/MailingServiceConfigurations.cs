using System;
using Cardofun.Interfaces.DTOs;

namespace Cardofun.Interfaces.Configurations
{
  /// <summary>
  /// Settings that configure the mailing service
  /// </summary>
  public class MailingServiceConfigurations
  {
      public EmailAddressDto Sender { get; set; }
      
      public String SmtpServer { get; set; }
      public Int32 SmtpPort  { get; set; }
      public String SmtpUsername { get; set; }
      public String SmtpPassword { get; set; }

      public String PopServer { get; set; }
      public Int32 PopPort { get; set; }
      public String PopUsername { get; set; }
      public String PopPassword { get; set; }
      
      /// <summary>
      /// Plain / Flowed (rfc3676) / Html / Enriched / CompressedRichText / RichText
      /// Html is default
      /// </summary>
      /// <value></value>
      public String TextFormat { get; set; }

      /// <summary>
      /// Path to the file that is used for composing Emails for user Email addresses confirmation
      /// </summary>
      /// <value></value>
      public String EmailConfirmationMessageTemplatePath { get; set; }
    }
}