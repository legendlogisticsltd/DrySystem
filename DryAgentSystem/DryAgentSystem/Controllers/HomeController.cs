using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DryAgentSystem.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        [Route("Home")]
        public ActionResult Home()
        {
            return View();
        }
    }
}