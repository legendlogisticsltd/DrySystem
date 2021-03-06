using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DryAgentSystem.Data;
using DryAgentSystem.Models;

namespace DryAgentSystem.Controllers
{
    public class ShipmentController : Controller
    {
        // GET: Shipment
        [Authorize]
        [Route("Shipment")]
        public ActionResult Shipment()
        {
            SearchParameters statusdisplay = new SearchParameters();
            ViewBag.statuslist = DataContext.GetShipmentStatus();
            ViewBag.PortList = DataContext.GetCountryPorts();
            return View();
        }

        [HttpPost]
        [Route("Shipment")]
        public ActionResult Shipment(SearchParameters search, string submit)
        {
            ViewBag.statuslist = DataContext.GetBookingStatus();
            ViewBag.PortList = DataContext.GetCountryPorts();
            if (submit == "Search")
            {
                TempData["SearchParameters"] = search;
                //TempData.Keep("SearchParameters");
                return View();
            }
            else
            {
                return RedirectToAction("Shipment", "Shipment");
            }
        }

        public JsonResult GetShipmentData(string sidx, string sort, int page, int rows)
        {
            SearchParameters search = new SearchParameters();
            if (TempData["SearchParameters"] != null)
            {
                search = (SearchParameters)TempData["SearchParameters"];
            }
            var shipmentData = DataContext.GetShipmentDetails(search);
            int totalRecords = shipmentData.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = (from shipmentGrid in shipmentData
                        select new
                        {
                            shipmentGrid.JobRef,
                            cell = new string[]
                            {
                                shipmentGrid.JobRef,
                                shipmentGrid.pzChargeParty,
                                shipmentGrid.BookingNo,
                                shipmentGrid.IDQuoteRef,
                                shipmentGrid.DischPort,
                                shipmentGrid.LoadPort,
                                shipmentGrid.StatusShipment
                            }
                        }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}