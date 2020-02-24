using System;

namespace Cardofun.Interfaces.Configurations
{
  /// <summary>
  /// Settings that configure the mailing service
  /// </summary>
  public class MailingServiceConfigurations
  {
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
    }
}