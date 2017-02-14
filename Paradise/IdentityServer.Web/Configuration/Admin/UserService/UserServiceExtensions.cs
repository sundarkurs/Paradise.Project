using IdentityServer.Web.Configuration.AspNetIdentity;
using IdentityServer3.Core.Configuration;
using IdentityServer3.Core.Services;

namespace IdentityServer.Web.Configuration.Admin.UserService
{
    public static class UserServiceExtensions
    {
        public static void ConfigureUserService(this IdentityServerServiceFactory factory, string connString)
        {
            factory.UserService = new Registration<IUserService, UserService>();
            factory.Register(new Registration<AspNetIdentity.UserManager>());
            factory.Register(new Registration<UserStore>());
            factory.Register(new Registration<Context>(resolver => new Context(connString)));
        }
    }
}