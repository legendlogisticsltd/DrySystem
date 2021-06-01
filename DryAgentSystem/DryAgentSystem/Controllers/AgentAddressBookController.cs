using DryAgentSystem.Data;
using DryAgentSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DryAgentSystem.Controllers
{
    public class AgentAddressBookController : Controller
    {
        // GET: AgentAddressBook
        [Authorize]
        [Route("AgentAddressBook")]
        public ActionResult AgentAddressBook()
        {
            SearchParameters statusdisplay = new SearchParameters();
            //ViewBag.statuslist = DataContext.GetBookingStatus();
            ViewBag.PortList = DataContext.GetCountryPorts();
            ViewBag.CompanyList = DataContext.GetCompany();
            // ViewBag.CompanyId = DataContext.GetCompanyId();
            return View();
        }

        [HttpPost]
        [Route("AgentAddressBook")]
        public ActionResult AgentAddressBook(SearchParameters search, string submit)
        {
            //ViewBag.statuslist = DataContext.GetBookingStatus();
            //ViewBag.PortList = DataContext.GetCountryPorts();
            ViewBag.CompanyList = DataContext.GetCompany();
            if (submit == "Search")
            {
                TempData["SearchParameters"] = search;
                //TempData.Keep("SearchParameters");
                return View();
            }
            else
            {
                return RedirectToAction("AgentAddressBook", "AgentAddressBook");
            }
        }
        public JsonResult GetAgentAddressBookData(string sidx, string sord, int page, int rows)
        {
            SearchParameters filter = new SearchParameters();

            if (TempData["SearchParameters"] != null)
            {
                filter = (SearchParameters)TempData["SearchParameters"];
            }
            var addressBookdata = DataContext.GetAllAgentAddressBookData(filter);
            int totalRecords = addressBookdata.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = (from AgentAddressBookDataGrid in addressBookdata
                        select new
                        {
                            AgentAddressBookDataGrid.ID,
                            cell = new string[]
                            {
                                AgentAddressBookDataGrid.ID,
                                AgentAddressBookDataGrid.CompanyName,
                                //AgentAddressBookDataGrid.Owner,
                                AgentAddressBookDataGrid.Email,
                                AgentAddressBookDataGrid.PhoneNo,
                                //AgentAddressBookDataGrid.IDCompany,
                            }
                        }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteAgentAddressDetails(string ID)
        {
            ErrorLog errorLog = DataContext.DeleteAgentAddress(ID);

            if (!errorLog.IsError)
            {

                TempData["Message"] = "data id successfully deleted ";
            }
            else
            {
                TempData["Message"] = errorLog.ErrorMessage;
            }

            return RedirectToAction("AgentAddressBook", "AgentAddressBook");
        }
    }
}