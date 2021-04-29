using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DryAgentSystem.Controllers
{
    public class InvoiceController : Controller
    {
        // GET: Invoice
        [Authorize]
        public ActionResult Invoice()
        {
            return View();
        }
    }
}