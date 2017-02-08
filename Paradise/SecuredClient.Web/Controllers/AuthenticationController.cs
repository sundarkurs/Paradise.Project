using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SecuredClient.Web.Controllers
{
    public class AuthenticationController : Controller
    {
        [Authorize]
        public ActionResult Login()
        {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            Request.GetOwinContext().Authentication.SignOut();
            return Redirect("/");
        }
    }
}