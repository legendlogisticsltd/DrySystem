using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DryAgentSystem.Data;
using DryAgentSystem.Models;

namespace DryAgentSystem.Controllers
{
    public class ShipmentDetailsController : Controller
    {
        // GET: ShipmentDetails
        [HttpGet]
        [Authorize]
        public ActionResult ShipmentDetails(string JobRef)
        {
            var shiprefdetails = DataContext.GetShipmentFromJobRef(JobRef);
            TempData["UniversalSerialNr"] = shiprefdetails.ShipmentDetailsModel.UniversalSerialNr;
            return View(shiprefdetails);
        }

        public JsonResult GetVesselDetails(string sidx, string sort, int page, int rows)
        {
            string universalSerialNr = string.Empty;
            if (TempData["UniversalSerialNr"] != null)
            {
                universalSerialNr = TempData["UniversalSerialNr"].ToString();

                var vesselData = DataContext.GetVesselsDetails(universalSerialNr);
                int totalRecords = vesselData.Count();
                var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

                var jsonData = new
                {
                    total = totalPages,
                    page,
                    records = totalRecords,
                    rows = (from vesselGrid in vesselData
                            select new
                            {
                                vesselGrid.ID,
                                cell = new string[]
                                {
                                    vesselGrid.ID,
                                    vesselGrid.VesselName,
                                    vesselGrid.VoyNo,
                                    vesselGrid.LoadPort,
                                    vesselGrid.DischPort,
                                    vesselGrid.ETD.ToString("MM-dd-yyyy"),
                                    vesselGrid.ETA.ToString("MM-dd-yyyy"),
                                    vesselGrid.Carrier,
                                    //vesselGrid.DateATA?.ToString("MM-dd-yyyy"),
                                    //vesselGrid.DateSOB?.ToString("MM-dd-yyyy"),
                                    vesselGrid.UniversalSerialNr,
                                    vesselGrid.CarrierBookingRefNo
                                }
                            }).ToArray()
                };

                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new object[] { new object() }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}