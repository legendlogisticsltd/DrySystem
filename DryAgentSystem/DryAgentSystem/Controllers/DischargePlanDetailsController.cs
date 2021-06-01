using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DryAgentSystem.Data;
using DryAgentSystem.Models;

namespace DryAgentSystem.Controllers
{
    public class DischargePlanDetailsController : Controller
    {
        // GET: DischargePlanDetails
        [HttpGet]
        [Authorize]
        public ActionResult DischargePlanDetails(string JobRef)
        {
            var shiprefdetails = DataContext.GetShipmentDetailsFromJobRef(JobRef);
            ViewBag.PortList = DataContext.GetCountryPorts();
            TempData["UniversalSerialNr"] = shiprefdetails.UniversalSerialNr;
            return View(shiprefdetails);
        }

        [HttpPost]
        public ActionResult DischargePlanDetails(ShipmentDetails shipmentDetails, string submit)
        {
            ViewBag.PortList = DataContext.GetCountryPorts();
            if (submit == "Save")
            {
                //if (ModelState.IsValid)
                //{
                    ErrorLog errorLog = DataContext.SaveDischargePlan(shipmentDetails);
                    if (!errorLog.IsError)
                    {
                        shipmentDetails.JobRef = errorLog.ErrorMessage;
                        TempData["Message"] = "Discharge Plan is successfully created";
                    }
                    else
                    {
                        TempData["Message"] = errorLog.ErrorMessage;
                    }
                //}
                //else
                //{
                //    TempData["Message"] = "Please check your fields, some of the fields are not in correct format";
                //    return View(shipmentDetails);
                //}
                //ModelState.Clear();
                return RedirectToAction("UpdateDischargePlanDetails", "DischargePlanDetails", new { JobRef = shipmentDetails.JobRef });
            }

            return View(shipmentDetails);
        }
        [HttpGet]
        [Authorize]
        public ActionResult UpdateDischargePlanDetails(string JobRef)
        {
            var dischargePlanDetails = DataContext.GetDischargePlanFromIDNo(JobRef);
            ViewBag.PortList = DataContext.GetCountryPorts();            

            TempData["JobRef"] = JobRef;
            return View(dischargePlanDetails);
        }
        public ActionResult UpdateDischargePlanDetails(DischargePlan dischargePlan, string submit)
        {
            ViewBag.PortList = DataContext.GetCountryPorts();

            if (submit == "Save")
            {
                //if (ModelState.IsValid)
                //{
                    ErrorLog errorLog = DataContext.UpdateDischargePlan(dischargePlan);
                    if (!errorLog.IsError)
                    {
                        TempData["message"] = "Discharge Plan successfully updated";
                    }
                    else
                    {
                        TempData["message"] = errorLog.ErrorMessage;
                    }
                //}
                //else
                //{
                //    TempData["Message"] = "Please check your fields, some of the fields are not in correct format";
                //}

                ModelState.Clear();
                return RedirectToAction("UpdateDischargePlanDetails", "DischargePlanDetails", new { JobRef = dischargePlan.JobRef });
            }

            return View(dischargePlan);
        }
        [HttpGet]
        public ActionResult Confirmation(int idNo)
        {
            string jobRef = TempData["JobRef"].ToString();
            ErrorLog errorLog = DataContext.ConfirmDischargePlan(idNo);
            //var dischargePlanDetails = DataContext.GetDischargePlanFromIDNo(jobRef);
            ViewBag.PortList = DataContext.GetCountryPorts();
            if (!errorLog.IsError)
            {
                TempData["message"] = "Discharge Plan successfully confirmed";
            }
            else
            {
                TempData["message"] = errorLog.ErrorMessage;
            }

            return RedirectToAction("UpdateDischargePlanDetails", "DischargePlanDetails", new { JobRef = jobRef });
        }
    }
}