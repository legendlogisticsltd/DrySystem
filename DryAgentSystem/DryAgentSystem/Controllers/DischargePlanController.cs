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
            //ViewBag.CompanyList = DataContext.GetCompany();
            return View();
        }
        [HttpPost]
        [Route("DischargePlan")]
        public ActionResult DischargePlan(SearchParameters search, string submit)
        {
            ViewBag.statuslist = DataContext.GetDischargePlanStatus();
            ViewBag.PortList = DataContext.GetCountryPorts();
            //ViewBag.CompanyList = DataContext.GetCompany();

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
            var shipmentData = DataContext.GetDischargePlanList(filter);
            int totalRecords = shipmentData.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = (from shipmentDataGrid in shipmentData
                        select new
                        {
                            //rateRequestGrid.RateID,
                            cell = new string[]
                            {
                                shipmentDataGrid.JobRef,
                                shipmentDataGrid.IDNo.ToString(),
                                shipmentDataGrid.LoadPort,
                                shipmentDataGrid.DischPort,
                                shipmentDataGrid.Country,
                                shipmentDataGrid.Quantity.ToString(),
                                shipmentDataGrid.DischargePlanStatus,
                                shipmentDataGrid.ATA.ToString("MM-dd-yyyy"),
                                shipmentDataGrid.CreateDate.ToString("MM-dd-yyyy"),
                                shipmentDataGrid.ClosingDate.ToString("MM-dd-yyyy"),
                                shipmentDataGrid.LoadAgent,
                                shipmentDataGrid.ETA.ToString("MM-dd-yyyy"),
                                shipmentDataGrid.ETD.ToString("MM-dd-yyyy"),
                                shipmentDataGrid.QuoteType,
                                shipmentDataGrid.QuantityLifting.ToString(),
                                shipmentDataGrid.DischAgentAddress,
                                shipmentDataGrid.DischFT.ToString(),
                                shipmentDataGrid.IDAgentDisch,
                                shipmentDataGrid.IDAgentLoad,
                                shipmentDataGrid.LoadAgentAddress,
                                shipmentDataGrid.LoadFT.ToString(),
                                shipmentDataGrid.PortPair,
                                shipmentDataGrid.ModifyDate.ToString("MM-dd-yyyy"),
                                shipmentDataGrid.PlaceOfDelivery,
                                shipmentDataGrid.PlaceOfReceipt,
                                shipmentDataGrid.DischAgentNameBL,
                                shipmentDataGrid.ShipperNameBL,
                                shipmentDataGrid.UniversalSerialNr,
                                shipmentDataGrid.User
                            }
                        }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}