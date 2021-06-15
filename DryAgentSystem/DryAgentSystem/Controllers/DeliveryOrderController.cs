using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DryAgentSystem.Data;
using DryAgentSystem.Models;

namespace DryAgentSystem.Controllers
{
    public class DeliveryOrderController : Controller
    {
        // GET: DeliveryOrder
        [Authorize]
        [Route("DeliveryOrder")]
        public ActionResult DeliveryOrder()
        {
            ViewBag.statuslist = DataContext.GetDischargePlanStatus();
            ViewBag.PortList = DataContext.GetCountryPorts();
            ViewBag.CompanyList = DataContext.GetCompany();
            return View();
        }

        [HttpPost]
        [Route("DeliveryOrder")]
        public ActionResult DeliveryOrder(SearchParameters search, string submit)
        {
            ViewBag.statuslist = DataContext.GetDischargePlanStatus();
            ViewBag.PortList = DataContext.GetCountryPorts();
            ViewBag.CompanyList = DataContext.GetCompany();

            if (submit == "Search")
            {
                TempData["filterobj"] = search;
                return View();
            }

            else
            {
                return RedirectToAction("DeliveryOrder", "DeliveryOrder");
            }
        }
        public JsonResult GetDeliveryOrder(string sidx, string sord, int page, int rows)
        {
            SearchParameters filter = new SearchParameters();

            if (TempData["filterobj"] != null)
            {
                filter = (SearchParameters)TempData["filterobj"];
            }
            var dischargePlanData = DataContext.GetConfirmedDischargePlanList(filter);
            int totalRecords = dischargePlanData.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = (from dischargePlanDataGrid in dischargePlanData
                        select new
                        {
                            cell = new string[]
                            {
                                dischargePlanDataGrid.JobRef,
                                dischargePlanDataGrid.ChargeParty,
                                dischargePlanDataGrid.ATA.ToString("MM-dd-yyyy") == "01-01-0001" ? null : dischargePlanDataGrid.ATA.ToString("MM-dd-yyyy"),
                                dischargePlanDataGrid.ETA.ToString("MM-dd-yyyy") == "01-01-0001" ? null : dischargePlanDataGrid.ETA.ToString("MM-dd-yyyy"),
                                dischargePlanDataGrid.ETD.ToString("MM-dd-yyyy") == "01-01-0001" ? null : dischargePlanDataGrid.ETD.ToString("MM-dd-yyyy"),
                                dischargePlanDataGrid.LoadPort,
                                dischargePlanDataGrid.DischPort,
                                dischargePlanDataGrid.DischargePlanStatus,
                                dischargePlanDataGrid.IDNo.ToString(),
                                dischargePlanDataGrid.UniversalSerialNr
                            }
                        }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}