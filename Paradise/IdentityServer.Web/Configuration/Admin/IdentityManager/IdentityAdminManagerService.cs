using IdentityAdmin.Configuration;
using IdentityAdmin.Core;
using IdentityServer.Web.Configuration.Helper;
using IdentityServer3.Admin.EntityFramework;
using IdentityServer3.Admin.EntityFramework.Entities;

namespace IdentityServer.Web.Configuration.Admin.IdentityManager
{
    public class IdentityAdminManagerService : IdentityAdminCoreManager<IdentityClient, int, IdentityScope, int>
    {
        public IdentityAdminManagerService()
            : base(MyConstants.AdminConfig)
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