using System;

namespace Cardofun.Interfaces.DTOs
{
    /// <summary>
    /// Stores an info of how to access an image from third parties
    /// </summary>
    public class GlobalPhotoIdentifiersDto
    {
        public String Url { get; set; }
        public String PublicId { get; set; }
    }
}