
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using IdentityServer.Web.Configuration.Authorization;
using Thinktecture.IdentityModel.Mvc;

namespace IdentityServer.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult About()
        {
            return View((User as ClaimsPrincipal).Claims);
        }

        [ResourceAuthorize("Read", "ContactDetails")]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [ResourceAuthorize("Write", "ContactDetails")]
        [HandleForbidden]
        public ActionResult UpdateContact()
        {
            if (!HttpContext.CheckAccess("Write", "ContactDetails", "some more data"))
            {
                // either 401 or 403 based on authentication state
                return this.AccessDenied();
            }

            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Logout()
        {
            Request.GetOwinContext().Authentication.SignOut();
            return Redirect("/");
        }
    }
}