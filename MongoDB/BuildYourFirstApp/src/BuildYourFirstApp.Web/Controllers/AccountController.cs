using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BuildYourFirstApp.Web.Controllers
{
    public class AccountController : Controller
    {
        [AllowAnonymous, HttpGet]
        public ActionResult Login()
        {
            var knownKeys = new[] { "blahg" };

            return View();
        }

        [AllowAnonymous, HttpPost]
        public ActionResult Login(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                ModelState.AddModelError("username", "Please supply a username.");
                return View();
            }

            // we take any username here because we aren't implementing authentication and will simply trust who they say they are.
            // NOTE: do not do this in production.
            FormsAuthentication.RedirectFromLoginPage(
                userName: username,
                createPersistentCookie: false // so we can use different users by closing down the browser
            );

            // we won't get here...
            return null;
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
    }
}