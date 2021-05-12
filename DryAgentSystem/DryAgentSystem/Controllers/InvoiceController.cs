using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DryAgentSystem.Data;
using DryAgentSystem.Models;

namespace DryAgentSystem.Controllers
{
    public class InvoiceController : Controller
    {
        // GET: Invoice
        [Authorize]
        [Route("Invoice")]
        public ActionResult Invoice()
        {
            SearchParameters statusdisplay = new SearchParameters();
            ViewBag.statuslist = DataContext.GetInvoiceStatus();
            ViewBag.PortList = DataContext.GetCountryPorts();
            return View();
        }

        [HttpPost]
        [Route("Invoice")]
        public ActionResult Invoice(SearchParameters search, string submit)
        {
            ViewBag.statuslist = DataContext.GetInvoiceStatus();
            ViewBag.PortList = DataContext.GetCountryPorts();
            if (submit == "Search")
            {
                TempData["SearchParameters"] = search;
                //TempData.Keep("SearchParameters");
                return View();
            }
            else
            {
                return RedirectToAction("Invoice", "Invoice");
            }
        }

        public JsonResult GetInvoiceData(string sidx, string sort, int page, int rows)
        {
            SearchParameters search = new SearchParameters();
            if (TempData["SearchParameters"] != null)
            {
                search = (SearchParameters)TempData["SearchParameters"];
            }
            var invoiceData = DataContext.GetInvoiceDetails(search);
            int totalRecords = invoiceData.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = (from InvoiceGrid in invoiceData
                        select new
                        {
                              InvoiceGrid.InvoiceNo,
                            cell = new string[]
                            {
                                InvoiceGrid.InvoiceNo,
                                InvoiceGrid.JobRefNo,
                                InvoiceGrid.BillingParty,
                                InvoiceGrid.LoadPort,
                                InvoiceGrid.DischargePort,
                                InvoiceGrid.Status
                            }
                        }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}