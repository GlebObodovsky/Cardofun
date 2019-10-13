using System;

namespace Cardofun.Modules.Cloudinary
{
    /// <summary>
    /// Settings that needed to establish Cloudinary session 
    /// </summary>
    public class CloudinaryProviderSettings
    {
      public String CloudName { get; set; }
      public String ApiKey { get; set; }
      public String ApiSecret { get; set; }
    }
}