using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using IdentityServer.Web.Configuration.AspNetIdentity;
using IdentityServer3.Core.Logging;
using IdentityServer3.Core.Models;

namespace IdentityServer.Web.Configuration.Admin.UserService
{
    public class UserService : AspNetIdentityUserService<User, string>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UserService" /> class.
        /// </summary>
        /// <param name="userMgr">The user MGR.</param>
        public UserService(AspNetIdentity.UserManager userMgr)
            : base(userMgr)
        {
        }
    }
}