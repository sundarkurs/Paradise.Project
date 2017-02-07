using System.Collections.Generic;
using IdentityServer3.Core.Models;

namespace IdentityServer.Web.Configuration
{
    public static class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new[]
            {
            new Client
            {
                Enabled = true,
                ClientName = "MVC Client",
                ClientId = "mvc",
                Flow = Flows.Implicit,

                RedirectUris = new List<string>
                {
                    "https://localhost:44329/"
                },
                PostLogoutRedirectUris = new List<string>
                {
                    "https://localhost:44329/"
                },
                AllowAccessToAllScopes = true
            }
        };
        }
    }
}