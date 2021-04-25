using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DryAgentSystem.Data;
using DryAgentSystem.Models;
using ClosedXML.Excel;
using System.Data;
using System.IO;

namespace DryAgentSystem.Controllers
{
    public class QuotationController : Controller
    {
        // GET: Quotation
        [Authorize]
        [Route("Quotation")]
        public ActionResult Quotation()
        {
            SearchParameters statusdisplay = new SearchParameters();
            ViewBag.statuslist = DataContext.GetQuoteStatus();
            ViewBag.PortList = DataContext.GetCountryPorts();
            ViewBag.CompanyList = DataContext.GetCompany();
            return View();
        }

        [HttpPost]
        [Route("Quotation")]
        public ActionResult Quotation(SearchParameters search,string submit)
        {
            ViewBag.statuslist = DataContext.GetQuoteStatus();
            ViewBag.PortList = DataContext.GetCountryPorts();
            ViewBag.CompanyList = DataContext.GetCompany();

            if (submit == "Search")
            {
                TempData["SearchParameters"] = search;
                //TempData.Keep("SearchParameters");
                return View();
            }
            if (submit == "ExportToExcel")
            {
                FileResult excelFile = ExportToExcel(search);
                return excelFile;
            }
            else
            {
                return RedirectToAction("Quotation", "Quotation");
            }
        }

        public JsonResult GetQuoteRefData(string sidx, string sord, int page, int rows)
        {
            SearchParameters search = new SearchParameters();
            if (TempData["SearchParameters"] != null)
            {
                search = (SearchParameters)TempData["SearchParameters"];
            }
            var quoteRefdata = DataContext.GetAllQuoteRefData(search);
            int totalRecords = quoteRefdata.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = (from quoteRefGrid in quoteRefdata
                        select new
                        {
                            quoteRefGrid.QuoteRefID,
                            cell = new string[]
                            {
                                quoteRefGrid.QuoteRefID,
                                quoteRefGrid.RateID,
                                quoteRefGrid.CompanyName,
                                //rateRequestGrid.ShipmentTerm,
                                quoteRefGrid.PlaceOfReceipt,
                                quoteRefGrid.LoadPort,
                                quoteRefGrid.TransshipmentPort,
                                quoteRefGrid.DischargePort,
                                quoteRefGrid.PlaceOfDelivery,
                                //quoteRefGrid.POLFreeDays.ToString(),
                                //quoteRefGrid.PODFreeDays.ToString(),
                                quoteRefGrid.EffectiveDate.ToString("MM-dd-yyyy"),
                                quoteRefGrid.Validity.ToString("MM-dd-yyyy"),
                                quoteRefGrid.StatusDIS,
                                quoteRefGrid.Quantity.ToString(),
                                quoteRefGrid.EquipmentType,
                                //rateRequestGrid.CargoType,
                                quoteRefGrid.Rate.ToString(),
                                quoteRefGrid.RateCountered.ToString(),
                                quoteRefGrid.AgentName
                            }
                        }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public FileResult ExportToExcel(SearchParameters searchParameters)
        {
            var quoteRefdata = DataContext.GetAllQuoteRefData(searchParameters);

            DataTable dataTable = new DataTable("Quotation");
            dataTable.Columns.AddRange(new DataColumn[20]
            { 
                new DataColumn("QuoteRefID"),
                new DataColumn("RateID"),
                new DataColumn("CompanyName"),
                new DataColumn("ShipmentTerm"),
                new DataColumn("LoadPort"),
                new DataColumn("TransshipmentPort"),
                new DataColumn("DischargePort"),
                new DataColumn("PlaceOfReceipt"),
                new DataColumn("PlaceOfDelivery"),
                new DataColumn("POLFreeDays"),
                new DataColumn("PODFreeDays"),
                new DataColumn("EffectiveDate"),
                new DataColumn("Validity"),
                new DataColumn("Status"),
                new DataColumn("Quantity"),
                new DataColumn("EquipmentType"),
                new DataColumn("CargoType"),
                new DataColumn("Rate"),
                new DataColumn("RateCountered"),
                new DataColumn("AgentName")
            });

            foreach (var quoteRef in quoteRefdata)
            {
                dataTable.Rows.Add(quoteRef.QuoteRefID, quoteRef.RateID, quoteRef.CompanyName, quoteRef.ShipmentTerm, quoteRef.LoadPort, quoteRef.TransshipmentPort, quoteRef.DischargePort,
                    quoteRef.PlaceOfReceipt, quoteRef.PlaceOfDelivery, quoteRef.POLFreeDays, quoteRef.PODFreeDays, quoteRef.EffectiveDate.ToString("MM-dd-yyyy"), 
                    quoteRef.Validity.ToString("MM-dd-yyyy"), quoteRef.StatusDIS, quoteRef.Quantity, quoteRef.EquipmentType, quoteRef.CargoType, quoteRef.Rate, 
                    quoteRef.RateCountered, quoteRef.AgentName);
            }

            using (XLWorkbook xLWorkbook = new XLWorkbook())
            {
                xLWorkbook.Worksheets.Add(dataTable, "Quotation");
                using (MemoryStream stream = new MemoryStream()) //using System.IO;  
                {
                    xLWorkbook.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "QuotationDetails.xlsx");
                }
            }
        }

    }
}