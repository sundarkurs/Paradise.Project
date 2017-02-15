using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using IdentityServer3.Core;
using Microsoft.Owin;

[assembly: OwinStartup(typeof(SecuredClient.Web.IdentityConfigurations.Startup))]
namespace SecuredClient.Web.IdentityConfigurations
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            AntiForgeryConfig.UniqueClaimTypeIdentifier = Constants.ClaimTypes.Subject;
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            // Middleware settings
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies",
                ExpireTimeSpan = TimeSpan.FromMinutes(10),
                SlidingExpiration = true
            });

            JwtSecurityTokenHandler.InboundClaimTypeMap.Clear();

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = ConfigurationManager.AppSettings["IdentityProviderHostedUrl"],
                ClientId = "mvc",
                Scope = "openid profile", // roles
                RedirectUri = ConfigurationManager.AppSettings["MvcClientUrl"],
                ResponseType = "id_token",

                SignInAsAuthenticationType = "Cookies",

                UseTokenLifetime = false,

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = n =>
                    {
                        var id = n.AuthenticationTicket.Identity;

                        // create new identity and set name and role claim type
                        var nid = new ClaimsIdentity(
                            id.AuthenticationType,
                            Constants.ClaimTypes.GivenName,
                            Constants.ClaimTypes.Role);

                        foreach (var i in id.Claims)
                        {
                            if (i.Type != "iss" && i.Type != "aud" && i.Type != "exp" && i.Type != "nbf" && i.Type != "nonce"
                            && i.Type != "iat" && i.Type != "sid" && i.Type != "auth_time" && i.Type != "idp" && i.Type != "amr"
                            && i.Type != "id_token")
                            {
                                nid.AddClaim(i);
                            }

                        }

                        // add some other app specific claim
                        nid.AddClaim(new Claim("app_specific", "some data"));

                        // Logout redirect
                        nid.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));

                        n.AuthenticationTicket = new AuthenticationTicket(
                            nid,
                            n.AuthenticationTicket.Properties);

                        return Task.FromResult(0);
                    },

                    // Logout redirect
                    RedirectToIdentityProvider = n =>
                    {
                        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.LogoutRequest)
                        {
                            var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token");

                            if (idTokenHint != null)
                            {
                                n.ProtocolMessage.IdTokenHint = idTokenHint.Value;
                            }
                        }

                        return Task.FromResult(0);
                    }
                }

            });

        }


    }
}