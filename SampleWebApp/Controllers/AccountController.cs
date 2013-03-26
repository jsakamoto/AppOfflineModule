using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SampleWebApp.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string loginName, string password)
        {
            if (Membership.ValidateUser(loginName, password) == true)
            {
                FormsAuthentication.SetAuthCookie(loginName, false);
                return RedirectToAction("Index", "Home");
            }
            return View();
        }


    }
}
