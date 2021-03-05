
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TaskOrganizer.database;
using TaskOrganizer.Models;

namespace TaskOrganizer.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User user)
        {
            using (ETOEntities eto = new ETOEntities())
            {

                bool isValid = eto.UserDetails.Any(x => x.UserName == user.UserName && x.Password == user.Password);
                if (isValid)
                {
                    FormsAuthentication.SetAuthCookie(user.UserName, false);
                    if(user.UserName == "Admin")
                    {
                        return RedirectToAction("AdminDashboard", "UserMvc");
                    }
                    return RedirectToAction("UserDashboard", "UserMvc",User.Identity.Name);
                }

                ViewBag.ErrorMessage = "Invalid Username or Password";
                return View();
            }

        }

        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(User user)
        {
            using (var client = new HttpClient())
            {
                UserDetail ud = new UserDetail();
                ud.UserName = user.UserName;
                ud.Password = user.Password;
                ud.Email = user.Email;
                ud.Mobile = user.Mobile;
                using (ETOEntities eto = new ETOEntities())
                {
                    eto.UserDetails.Add(ud);
                    eto.SaveChanges();
                }
                
                    return RedirectToAction("AdminDashboard", "UserMvc");
                
            }

        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}