namespace Nop.Web.Themes.SaljiDalje.ExternalAuth.Google
{
    /// <summary>
    /// Represents settings of the Facebook authentication method
    /// </summary>
    public static class FacebookExternalAuthSettings
    {
        /// <summary>
        /// Gets or sets OAuth2 client identifier
        /// </summary>
        public static string ClientKeyIdentifier { get; set; } = "";

        /// <summary>
        /// Gets or sets OAuth2 client secret
        /// </summary>
        public static string ClientSecret { get; set; } = "";
    }
}