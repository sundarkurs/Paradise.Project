using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IdentityManager;
using IdentityManager.Configuration;

namespace IdentityServer.Web.Service
{
    //public static class IdentityUserManagerService : AspNetIdentityManagerService<User, string, Role, string>
    //{
    //    /// <summary>
    //    ///     Initializes a new instance of the <see cref="SimpleIdentityManagerService" /> class.
    //    /// </summary>
    //    /// <param name="userManager">The user manager.</param>
    //    /// <param name="roleManager">The role manager.</param>
    //    public SimpleIdentityManagerService(UserManager userManager, RoleManager roleManager)
    //        : base(userManager, roleManager)
    //    {
    //    }
    //}

    //public static class SimpleIdentityManagerServiceExtensions
    //{
    //    /// <summary>
    //    ///     Configures the simple identity manager service for Identity User Administration
    //    /// </summary>
    //    /// <param name="factory">The factory.</param>
    //    /// <param name="connectionString">The connection string.</param>
    //    public static void ConfigureSimpleIdentityManagerService(this IdentityManagerServiceFactory factory,
    //        string connectionString)
    //    {
    //        factory.Register(new Registration<Context>(resolver => new Context(connectionString)));
    //        factory.Register(new Registration<UserStore>());
    //        factory.Register(new Registration<RoleStore>());
    //        factory.Register(new Registration<UserManager>());
    //        factory.Register(new Registration<RoleManager>());
    //        factory.IdentityManagerService = new Registration<IIdentityManagerService, SimpleIdentityManagerService>();
    //    }
    //}
}