using DryAgentSystem.Data;
using DryAgentSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace DryAgentSystem.Controllers
{
    public class AgentAddressBookDetailsController : Controller
    {
        // GET: AgentAddressBookDetails
        [Authorize]
        //public ActionResult AgentAddressBookDetails(string ID)
        //{
        //    AgentAddressBook agentaddressbook = new AgentAddressBook();
        //    agentaddressbook = DataContext.GetAgentAddressBookfromID(ID);
        //    Session["ID"] = ID;


        //    if (TempData["agentaddressbookobj"] != null)
        //    {
        //        agentaddressbook = (AgentAddressBook)TempData["agentaddressbookobj"];
        //    }

        //    return View(agentaddressbook);
        //}

        public ActionResult AgentAddressBookDetails()
        {
            ViewBag.PortList = DataContext.GetCountryPorts();
           // ViewBag.Comoanynames=
            return View();
        }

        public JsonResult GetCompanyNames(string agentaddressbook)
        {
            var companynames = DataContext.GetCompanyNames(agentaddressbook);
            return Json(companynames, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetCompanyNamesForUpadte(string agentaddressbook)
        //{
        //    var comapnynames = DataContext.GetCompanyNamesForUpdate(agentaddressbook);
        //    return Json(comapnynames, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetCompanyNamesForUpdate(string CompanyName, string CompanyID)
        {
            var companynames = DataContext.GetCompanyNamesForUpdate(CompanyName, CompanyID);
            return Json(companynames, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AgentAddressBookDetails(AgentAddressBook agentaddressbook, string submit)
        {
            ViewBag.PortList = DataContext.GetCountryPorts();
            if (submit == "Save")
            {
                bool flag = true;
                bool EmailFlag = true;
                var companynames = DataContext.GetCompanyNames(agentaddressbook.CompanyName);
                
                if (companynames == true)
                {
                    TempData["Message"] = "Company name already exist.";
                    // return RedirectToAction("AgentAddressBookDetails", "AgentAddressBookDetails");
                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        ErrorLog errorLog = DataContext.SaveAddressBook(agentaddressbook);
                        if (!errorLog.IsError && flag == true && EmailFlag == true)
                        {
                            agentaddressbook.ID = errorLog.ErrorMessage;
                            //TempData["Message"] = "Agent Address Book successfully created ID " + agentaddressbook.ID;
                        }

                        else
                        {
                            TempData["Message"] = errorLog.ErrorMessage;
                        }
                    }
                    ModelState.Clear();
                    return RedirectToAction("AgentAddressBook", "AgentAddressBook");
                }
            }
            if (submit == "Update")
            {
                //var comapnynames = DataContext.GetCompanyNamesForUpdate(agentaddressbook.CompanyName);
                if (ModelState.IsValid)
                {
                    ErrorLog errorLog = DataContext.UpdateAgentAddressDetails(agentaddressbook);
                    if (!errorLog.IsError)
                    {
                        //TempData["message"] = "Agent Address Book successfully updated";
                    }
                    else
                    {
                        TempData["message"] = errorLog.ErrorMessage;
                    }

                }
                ModelState.Clear();
                return RedirectToAction("AgentAddressBook", "AgentAddressBook");
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult UpdateAgentAddressDetails(string ID)
        {
            var addressbookDetails = DataContext.GetAddressBookFromID(ID);
            
            ViewBag.PortList = DataContext.GetCountryPorts();
            ViewBag.shipmentlist = DataContext.ShipmentTerm();
            ViewBag.equipmentlist = DataContext.EquipmentType();
            ViewBag.CompanyList = DataContext.GetCompany();
            ViewBag.AgencyList = DataContext.GetAgencyType();

            TempData["ID"] = ID;
            return View(addressbookDetails);
        }

        [HttpPost]
        public ActionResult UpdateAgentAddressBookDetails(AgentAddressBook agentaddressbook, string submit)
        {
            if (ModelState.IsValid)
            {
                ViewBag.PortList = DataContext.GetCountryPorts();
                ErrorLog errorLog = DataContext.UpdateAgentAddressDetails(agentaddressbook);
                if (!errorLog.IsError)
                {
                    //TempData["message"] = "Agent Address Book successfully updated";
                }
                else
                {
                    TempData["message"] = errorLog.ErrorMessage;
                }

            }
            else
            {
                TempData["Message"] = "Please check your fields, some of the fields are not in correct format";
            }
            ModelState.Clear();
            return RedirectToAction("AgentAddressBook", "AgentAddressBook");
        }
    }
}