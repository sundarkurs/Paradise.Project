﻿using IdentityManager;
using IdentityManager.AspNetIdentity;
using IdentityManager.Configuration;
using IdentityServer.Web.AspNetIdentity;

namespace IdentityServer.Web.Service
{
    /// <summary>
    ///     SimpleIdentity ManagerServiceExtensions
    /// </summary>
    public static class SimpleIdentityManagerServiceExtensions
    {
        /// <summary>
        ///     Configures the simple identity manager service for Identity User Administration
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="connectionString">The connection string.</param>
        public static void ConfigureSimpleIdentityManagerService(this IdentityManagerServiceFactory factory,
            string connectionString)
        {
            factory.Register(new Registration<Context>(resolver => new Context(connectionString)));
            factory.Register(new Registration<UserStore>());
            factory.Register(new Registration<RoleStore>());
            factory.Register(new Registration<UserManager>());
            factory.Register(new Registration<RoleManager>());
            factory.IdentityManagerService = new Registration<IIdentityManagerService, SimpleIdentityManagerService>();
        }
    }

    /// <summary>
    ///     Simple IdentityManager Service
    /// </summary>
    /// <seealso cref="IdentityManager.AspNetIdentity.AspNetIdentityManagerService{User, String, Role, String}" />
    public class SimpleIdentityManagerService : AspNetIdentityManagerService<User, string, Role, string>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SimpleIdentityManagerService" /> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="roleManager">The role manager.</param>
        public SimpleIdentityManagerService(UserManager userManager, RoleManager roleManager)
            : base(userManager, roleManager)
        {
        }
    }
}