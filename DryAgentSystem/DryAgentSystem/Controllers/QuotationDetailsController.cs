using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DryAgentSystem.Data;
using DryAgentSystem.Models;

namespace DryAgentSystem.Controllers
{
    public class QuotationDetailsController : Controller
    {
        // GET: Quotation
        [HttpGet]
        [Authorize]

        public ActionResult QuotationDetails(string quoteRefID)
        {
            var quoterefdetails = DataContext.GetQuoteRequestFromQuoteID(quoteRefID);
            
            TempData["rateID"] = quoterefdetails.RateID;
            return View(quoterefdetails);
        }

        [HttpPost]
        public ActionResult QuotationDetails(QuoteRef quote)
        {
            ViewBag.PortList = DataContext.GetCountryPorts();
            ViewBag.shipmentlist = DataContext.ShipmentTerm();
            ViewBag.equipmentlist = DataContext.EquipmentType();


            Booking bookingnew = new Booking();
            if (ModelState.IsValid)
            {
                {
                    bookingnew.CargoType = quote.CargoType;
                    bookingnew.CompanyName = quote.CompanyName;
                    bookingnew.DischargePort = quote.DischargePort;
                    //bookingnew.EffectiveDate = quote.EffectiveDate;
                    bookingnew.EquipmentType = quote.EquipmentType;
                    //bookingnew.ExportLocalCharges = quote.ExportLocalCharges;
                    bookingnew.IDCompany = quote.IDCompany;
                    bookingnew.IMO = quote.IMO;
                    bookingnew.UNNo = quote.UNNo;
                    bookingnew.LoadPort = quote.LoadPort;
                    bookingnew.TransshipmentPort = quote.TransshipmentPort;
                    bookingnew.TransshipmentType = quote.TransshipmentType;
                    bookingnew.RateType = quote.RateType;
                    bookingnew.Temperature = quote.Temperature;
                    bookingnew.Humidity = quote.Humidity;
                    bookingnew.Ventilation = quote.Ventilation;
                    bookingnew.PlaceOfDelivery = quote.PlaceOfDelivery;
                    bookingnew.PlaceOfReceipt = quote.PlaceOfReceipt;
                    bookingnew.PODFreeDays = quote.PODFreeDays;
                    bookingnew.POLFreeDays = quote.POLFreeDays;
                    bookingnew.Quantity = quote.Quantity;
                    bookingnew.GTotalSalesCal = quote.GTotalSalesCal;
                    bookingnew.Rate = quote.Rate;
                    bookingnew.RateCountered = quote.RateCountered;
                    bookingnew.Grossweight = quote.GrossWt;
                    bookingnew.RateID = quote.RateID;
                    bookingnew.QuoteRefID = quote.QuoteRefID;
                    bookingnew.Shipper = quote.ShipperName3;

                    bookingnew.Remark = quote.Remark;

                    bookingnew.ShipmentTerm = quote.ShipmentTerm;
                    //bookingnew.Validity = quote.Validity;

                }
                TempData["bookingobj"] = bookingnew;
                ModelState.Clear();
                return RedirectToAction("BookingDetails", "BookingDetails");

            }
            else
            {
                TempData["Message"] = "Please check the fields, some of the fields are not in correct format";
                
                ModelState.Clear();
                return View();
            }
        }

        public JsonResult GetQuoteChargesList(string sidx, string sord, int page, int rows)//, SearchParameters statusdisplay)
        {
            string rateID = (string)TempData["rateID"];
            var quoteChargesdata = DataContext.GetQuoteChargesList(rateID);
            int totalRecords = quoteChargesdata.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = (from rateRequestGrid in quoteChargesdata
                        select new
                        {
                            rateRequestGrid.ID,
                            cell = new string[]
                            {
                                rateRequestGrid.ID,
                                rateRequestGrid.Description
                            }
                        }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}