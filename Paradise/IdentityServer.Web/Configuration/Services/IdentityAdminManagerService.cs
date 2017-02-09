using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IdentityAdmin.Configuration;
using IdentityAdmin.Core;
using IdentityServer.Web.IdentityManage;
using IdentityServer3.Admin.EntityFramework;
using IdentityServer3.Admin.EntityFramework.Entities;

namespace IdentityServer.Web.Configuration.Services
{
    public class IdentityAdminManagerService : IdentityAdminCoreManager<IdentityClient, int, IdentityScope, int>
    {
        public IdentityAdminManagerService()
            : base(LocalConstants.AdminConfig)
        {
        }
    }

    public static class IdentityAdminManagerServiceExtensions
    {
        public static void Configure(this IdentityAdminServiceFactory factory)
        {
            factory.IdentityAdminService = new Registration<IIdentityAdminService, IdentityAdminManagerService>();
        }
    }
}