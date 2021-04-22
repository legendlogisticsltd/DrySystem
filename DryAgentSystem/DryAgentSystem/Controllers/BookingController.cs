using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DryAgentSystem.Data;
using DryAgentSystem.Models;

namespace DryAgentSystem.Controllers
{
    public class BookingController : Controller
    {
        // GET: Booking
        [Authorize]
        [Route("Booking")]
        public ActionResult Booking()
        {
            SearchParameters statusdisplay = new SearchParameters();
            ViewBag.statuslist = DataContext.GetBookingStatus();
            ViewBag.PortList = DataContext.GetCountryPorts();
            ViewBag.CompanyList = DataContext.GetCompany();
            return View();
        }

        [HttpPost]
        [Route("Booking")]
        public ActionResult Booking(SearchParameters search,string submit)
        {
            ViewBag.statuslist = DataContext.GetBookingStatus();
            ViewBag.PortList = DataContext.GetCountryPorts();
            ViewBag.CompanyList = DataContext.GetCompany();
            if (submit == "Search")
            {
                TempData["SearchParameters"] = search;
                //TempData.Keep("SearchParameters");
                return View();
            }
            else
            {
                return RedirectToAction("Booking", "Booking");
            }
        }

        public JsonResult GetBookingData(string sidx, string sort, int page, int rows)
        {
            SearchParameters search = new SearchParameters();
            if (TempData["SearchParameters"] != null)
            {
                search = (SearchParameters)TempData["SearchParameters"];
            }
            var bookingData = DataContext.GetBookingDetails(search);
            int totalRecords = bookingData.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = (from bookingGrid in bookingData
                        select new
                        {
                            bookingGrid.BookingID,
                            cell = new string[]
                            {
                                bookingGrid.BookingID,
                                bookingGrid.BookingNo,
                                bookingGrid.QuoteRefID,
                                bookingGrid.CompanyName,
                                bookingGrid.DischargePort,
                                bookingGrid.LoadPort,
                                bookingGrid.BookingStatus
                            }
                        }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
    }
}