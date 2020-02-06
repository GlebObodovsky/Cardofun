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
        /// Settings that define an image provider for the server
        /// </summary>
        /// <returns></returns>
        public const string ImageProviderSettings = nameof(ImageProviderSettings);
        /// <summary>
        /// Set of origins that are allowed to get the data from the server (CORS)
        /// </summary>
        /// <returns></returns>
        public const string CorsOrigins = nameof(CorsOrigins);
    }
}