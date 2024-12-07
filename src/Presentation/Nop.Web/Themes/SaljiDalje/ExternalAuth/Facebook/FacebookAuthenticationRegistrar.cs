using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Nop.Services.Authentication.External;
using Nop.Web.Themes.SaljiDalje.ExternalAuth.Google;

namespace SaljiDalje.Core
{
    /// <summary>
    /// Represents registrar of Facebook authentication service
    /// </summary>
    public class FacebookAuthenticationRegistrar : IExternalAuthenticationRegistrar
    {
        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="builder">Authentication builder</param>
        public void Configure(AuthenticationBuilder builder)
        {
            builder.AddFacebook(FacebookDefaults.AuthenticationScheme, options =>
            {
                //set credentials
                //var settings = EngineContext.Current.Resolve<GoogleExternalAuthSettings>();
                options.ClientId = FacebookExternalAuthSettings.ClientKeyIdentifier;
                options.ClientSecret = FacebookExternalAuthSettings.ClientSecret;
                
                options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.CorrelationCookie.SameSite = SameSiteMode.Unspecified;

                //store access and refresh tokens for the further usage
                options.SaveTokens = true;

                //set custom events handlers
                options.Events = new OAuthEvents
                {
                    //in case of error, redirect the user to the specified URL
                    OnRemoteFailure = context =>
                    {
                        context.HandleResponse();

                        var errorUrl = context.Properties.GetString(FacebookAuthenticationDefaults.ErrorCallback);
                        context.Response.Redirect(errorUrl);

                        return Task.FromResult(0);
                    }
                };
            });
        }
    }
}