using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DryAgentSystem.Models;
using DryAgentSystem.Data;
using System.Data.SqlClient;
using System.Configuration;
using Dapper;
using System.Data;
using System.Web.Security;

namespace DryAgentSystem.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserDetails userDetails, string returnURL)
        {
            userDetails.Name = userDetails.Name.ToUpperInvariant();
            if (ModelState.IsValid)
            {
                if (UserIsValid(userDetails))
                {
                    FormsAuthentication.SetAuthCookie(userDetails.Name, false);
                    Session["Name"] = userDetails.Name.ToString();
                    
                    if (returnURL != null)
                    {
                        return Redirect(returnURL);

                    }
                    else
                    {
                        return RedirectToAction("Home", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email ID or Password is incorrect.");
                }
            }
            return View();
        }

        private bool UserIsValid(UserDetails userDetails)
        {
             return DataContext.GetValidUser(userDetails);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session["Name"] = null;
            return Redirect("Login");
        }
    }
}
    
