using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DryAgentSystem.Controllers
{
    public class InvoiceDetailsController : Controller
    {
        // GET: InvoiceDetails
        [Authorize]
        public ActionResult InvoiceDetails()
        {
            return View();
        }
    }
}