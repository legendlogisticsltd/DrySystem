using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DryAgentSystem.Data;
using DryAgentSystem.Models;

namespace DryAgentSystem.Controllers
{
    public class RateRequestController : Controller
    {
        // GET: RateRequest
        [Authorize]
        [Route("RateRequest")]
        public ActionResult RateRequest()
        {
            SearchParameters statusdisplay = new SearchParameters();
            ViewBag.statuslist = DataContext.GetStatus();
            ViewBag.PortList = DataContext.GetCountryPorts();
            ViewBag.CompanyList = DataContext.GetCompany();
            return View();
        }

        [HttpPost]
        [Route("RateRequest")]
        public ActionResult RateRequest(SearchParameters filter,string submit)
        {
            ViewBag.statuslist = DataContext.GetStatus();
            ViewBag.PortList = DataContext.GetCountryPorts();
            ViewBag.CompanyList = DataContext.GetCompany();

            if (submit == "Search")
            {
                TempData["filterobj"] = filter;
                return View();
            }

            else
            {
                return RedirectToAction("RateRequest", "RateRequest");
            }
        }



        public JsonResult GetRateRequestData(string sidx, string sord, int page, int rows)
        {
            SearchParameters filter = new SearchParameters();

            if (TempData["filterobj"] != null)
            {
                filter = (SearchParameters)TempData["filterobj"];
            }
            var rateRequestdata = DataContext.GetAllRateRequestData(filter);
            int totalRecords = rateRequestdata.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = (from rateRequestGrid in rateRequestdata
                        select new
                        {
                            rateRequestGrid.RateID,
                            cell = new string[]
                            {
                                rateRequestGrid.RateID,
                                rateRequestGrid.CompanyName,
                                //rateRequestGrid.ShipmentTerm,
                                rateRequestGrid.PlaceOfReceipt,
                                rateRequestGrid.LoadPort,
                                rateRequestGrid.TransshipmentPort,
                                rateRequestGrid.DischargePort,
                                rateRequestGrid.PlaceOfDelivery,
                                //rateRequestGrid.POLFreeDays.ToString(),
                                //rateRequestGrid.PODFreeDays.ToString(),
                                rateRequestGrid.EffectiveDate.ToString("MM-dd-yyyy"),
                                rateRequestGrid.ValidityDate.ToString("MM-dd-yyyy"),
                                rateRequestGrid.Status,
                                rateRequestGrid.Quantity.ToString(),
                                rateRequestGrid.EquipmentType,
                                //rateRequestGrid.CargoType,
                                rateRequestGrid.Rate.ToString(),
                                rateRequestGrid.RateCountered.ToString(),
                                rateRequestGrid.AgentName
                            }
                        }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}