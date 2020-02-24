namespace Cardofun.API.Helpers.Constants
{
    public static class AppSettingsConstants
    {
        /// <summary>
        /// Security token
        /// </summary>
        public const string Token = "AppSettings:Token";
        /// <summary>
        /// Time before the issued token gets expired in hours
        /// </summary>
        public const string TokenExpiresInHours = "AppSettings:TokenExpiresInHours";
        /// <summary>
        /// Settings that are defined for the image service using on the server
        /// </summary>
        /// <returns></returns>
        public const string ImageServiceSettings = nameof(ImageServiceSettings);
        /// <summary>
        /// Settings that are defined for the mailing service using on the server
        /// </summary>
        /// <returns></returns>
        public const string MailingServiceSettings = nameof(MailingServiceSettings);
        /// <summary>
        /// Set of origins that are allowed to get the data from the server (CORS)
        /// </summary>
        /// <returns></returns>
        public const string CorsOrigins = nameof(CorsOrigins);
        /// <summary>
        /// Set of SignalR endpoints
        /// </summary>
        /// <returns></returns>
        public const string SignalrEndpoints = nameof(SignalrEndpoints);
    }
}