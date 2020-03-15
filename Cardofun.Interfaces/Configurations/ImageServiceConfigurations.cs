using System;

namespace Cardofun.Interfaces.Configurations
{
    /// <summary>
    /// Settings that needed to establish the session 
    /// </summary>
    public class ImageServiceConfigurations
    {
      public String CloudName { get; set; }
      public String ApiKey { get; set; }
      public String ApiSecret { get; set; }
    }
}