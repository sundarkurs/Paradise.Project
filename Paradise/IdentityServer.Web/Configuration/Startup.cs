﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Net;
using System.Net.Security;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Helpers;
using IdentityAdmin.Configuration;
using IdentityManager.Configuration;
using IdentityServer.Web.Configuration.Admin.IdentityManager;
using IdentityServer.Web.Configuration.Admin.UserManager;
using IdentityServer.Web.Configuration.Admin.UserService;
using IdentityServer.Web.Configuration.Certificate;
using IdentityServer.Web.Configuration.Helper;
using IdentityServer.Web.Configuration.InMemory;
using IdentityServer3.Core;
using IdentityServer3.Core.Configuration;
using IdentityServer3.EntityFramework;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;

[assembly: OwinStartup(typeof(IdentityServer.Web.Configuration.Startup))]
namespace IdentityServer.Web.Configuration
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            InitializeUserAdmin(app);
            InitializeConfigAdmin(app);
            //InitializeInMemoryIdentityServer(app);
            InitializeDataStoreIdentityServer(app);
            //SelfClient(app);
        }

        void InitializeUserAdmin(IAppBuilder app)
        {
            // Users and Roles manager
            app.Map("/useradmin", adminApp =>
            {
                var factory = new IdentityManagerServiceFactory();
                factory.ConfigureSimpleIdentityManagerService(MyConstants.UserAdminConfig);

                adminApp.UseIdentityManager(new IdentityManagerOptions
                {
                    Factory = factory
                });
            });
        }

        void InitializeConfigAdmin(IAppBuilder app)
        {
            // Clients and Scopes manager
            app.Map("/configadmin", adminApp =>
            {
                var factory = new IdentityAdminServiceFactory();
                factory.Configure();
                adminApp.UseIdentityAdmin(new IdentityAdminOptions
                {
                    Factory = factory
                });
            });
        }

        void InitializeInMemoryIdentityServer(IAppBuilder app)
        {
            app.Map("/identity", identityServerApp =>
            {
                var factory = new IdentityServerServiceFactory();

                factory.UseInMemoryUsers(Users.Get());
                factory.UseInMemoryClients(Clients.Get());
                factory.UseInMemoryScopes(Scopes.Get());

                factory.ConfigureClientStoreCache();
                factory.ConfigureScopeStoreCache();
                factory.ConfigureUserServiceCache();

                var identityServerOptions = new IdentityServerOptions
                {
                    SiteName = "Paradise Security Provider",
                    Factory = factory,
                    SigningCertificate = Cert.Load(),

                    AuthenticationOptions = new IdentityServer3.Core.Configuration.AuthenticationOptions
                    {
                        IdentityProviders = ConfigureIdentityProviders,
                        EnableAutoCallbackForFederatedSignout = false
                    },
                };

                identityServerApp.UseIdentityServer(identityServerOptions);

            });
        }

        void InitializeDataStoreIdentityServer(IAppBuilder app)
        {
            app.Map("/identity", identityServerApp =>
            {
                var factory = new IdentityServerServiceFactory();

                var adminConfiguration = new EntityFrameworkServiceOptions
                {
                    ConnectionString = MyConstants.AdminConfig
                };

                factory.RegisterConfigurationServices(adminConfiguration);
                factory.RegisterOperationalServices(adminConfiguration);

                factory.ConfigureUserService(MyConstants.UserAdminConfig);

                factory.ConfigureClientStoreCache();
                factory.ConfigureScopeStoreCache();
                factory.ConfigureUserServiceCache();

                var identityServerOptions = new IdentityServerOptions
                {
                    SiteName = "Paradise Security Provider",
                    Factory = factory,
                    SigningCertificate = Cert.Load(),

                    AuthenticationOptions = new IdentityServer3.Core.Configuration.AuthenticationOptions
                    {
                        IdentityProviders = ConfigureIdentityProviders,
                        EnableAutoCallbackForFederatedSignout = false
                    },
                };

                identityServerApp.UseIdentityServer(identityServerOptions);

            });
        }

        void SelfClient(IAppBuilder app)
        {

            AntiForgeryConfig.UniqueClaimTypeIdentifier = Constants.ClaimTypes.Subject;
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();


            // Middleware settings
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = ConfigurationManager.AppSettings["IdentityProviderHostedUrl"],
                ClientId = "mvc",
                Scope = "openid profile roles",
                RedirectUri = ConfigurationManager.AppSettings["MvcClientUrl"],
                ResponseType = "id_token",

                SignInAsAuthenticationType = "Cookies",

                UseTokenLifetime = false,

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = n =>
                    {
                        var id = n.AuthenticationTicket.Identity;

                        // we want to keep first name, last name, subject and roles
                        var givenName = id.FindFirst(Constants.ClaimTypes.GivenName);
                        var familyName = id.FindFirst(Constants.ClaimTypes.FamilyName);
                        var sub = id.FindFirst(Constants.ClaimTypes.Subject);
                        var roles = id.FindAll(Constants.ClaimTypes.Role);

                        // create new identity and set name and role claim type
                        var nid = new ClaimsIdentity(
                            id.AuthenticationType,
                            Constants.ClaimTypes.GivenName,
                            Constants.ClaimTypes.Role);

                        nid.AddClaim(givenName);
                        nid.AddClaim(familyName);
                        nid.AddClaim(sub);
                        nid.AddClaims(roles);

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

        X509Certificate2 LoadCertificate()
        {
            // It's a hack
            ServicePointManager.ServerCertificateValidationCallback =
                delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                {
                    return true;
                };

            return new X509Certificate2(
                string.Format(@"{0}\bin\identityServer\idsrv3test.pfx", AppDomain.CurrentDomain.BaseDirectory), "idsrv3test");
        }

        private void ConfigureIdentityProviders(IAppBuilder app, string signInAsType)
        {
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                AuthenticationType = "Google",
                Caption = "Sign-in with Google",
                SignInAsAuthenticationType = signInAsType,

                ClientId = "1076925854289-vstdac8ln4pbr3pj3snm0osam83snp46.apps.googleusercontent.com",
                ClientSecret = "RoRLUHqwpBPBiywEp_VDc73u"
            });
        }
    }
}