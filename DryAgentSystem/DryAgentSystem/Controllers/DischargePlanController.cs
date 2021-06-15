using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DryAgentSystem.Data;
using DryAgentSystem.Models;

namespace DryAgentSystem.Controllers
{
    public class DischargePlanController : Controller
    {
        // GET: DischargePlan
        [Authorize]
        [Route("DischargePlan")]
        public ActionResult DischargePlan()
        {
            SearchParameters statusdisplay = new SearchParameters();
            ViewBag.statuslist = DataContext.GetDischargePlanStatus();
            ViewBag.PortList = DataContext.GetCountryPorts();
            ViewBag.CompanyList = DataContext.GetCompany();
            return View();
        }
        [HttpPost]
        [Route("DischargePlan")]
        public ActionResult DischargePlan(SearchParameters search, string submit)
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
                return RedirectToAction("DischargePlan", "DischargePlan");
            }
        }
        public JsonResult GetDischargePlan(string sidx, string sord, int page, int rows)
        {
            SearchParameters filter = new SearchParameters();

            if (TempData["filterobj"] != null)
            {
                filter = (SearchParameters)TempData["filterobj"];
            }
            var dischargePlanData = DataContext.GetDischargePlanList(filter);
            int totalRecords = dischargePlanData.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = (from dischargePlanGrid in dischargePlanData
                        select new
                        {
                            //rateRequestGrid.RateID,
                            cell = new string[]
                            {
                                dischargePlanGrid.JobRef,
                                dischargePlanGrid.ChargeParty,
                                dischargePlanGrid.IDNo.ToString(),
                                dischargePlanGrid.LoadPort,
                                dischargePlanGrid.DischPort,
                                dischargePlanGrid.Quantity.ToString(),
                                dischargePlanGrid.DischargePlanStatus,
                                dischargePlanGrid.ATA.ToString("MM-dd-yyyy"),
                                dischargePlanGrid.ETA.ToString("MM-dd-yyyy"),
                                dischargePlanGrid.ETD.ToString("MM-dd-yyyy"),
                                dischargePlanGrid.User
                            }
                        }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}