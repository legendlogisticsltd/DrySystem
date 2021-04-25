using DryAgentSystem.Data;
using DryAgentSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;

namespace DryAgentSystem.Controllers
{
    public class RateRequestDetailsController : Controller
    {
        // GET: RateRequestDetails

        [Authorize]
        //[ValidateAntiForgeryToken]
        //[Route("RateRequest/RateRequestDetails")]
        public ActionResult RateRequestDetails()
        {
            RateRequest ratereq = new RateRequest { ExportLocalCharges = new List<SelectListItem>(), ImportLocalCharges = new List<SelectListItem>() };

            //RateRequest ratereq = new RateRequest();
            if (TempData["raterequestobj"] != null)
            {
                ratereq = (RateRequest)TempData["raterequestobj"];
            }

            ViewBag.PortList = DataContext.GetCountryPorts();
            ViewBag.shipmentlist = DataContext.ShipmentTerm();
            ViewBag.equipmentlist = DataContext.EquipmentType();
            ViewBag.CompanyList = DataContext.GetCompany();
            ViewBag.AgencyList = DataContext.GetAgencyType();
            SelectList selectExportListItems = new SelectList("", "Text", "Value");

            ViewBag.ExportCharges = selectExportListItems;
            SelectList selectImportListItems = new SelectList("", "Text", "Value");
            ViewBag.ImportCharges = selectImportListItems;

            return View(ratereq);
        }


        [HttpPost]
        public ActionResult RateRequestDetails(RateRequest rateRequest, string submit)
        {

            ViewBag.shipmentlist = DataContext.ShipmentTerm();
            ViewBag.equipmentlist = DataContext.EquipmentType();
            ViewBag.PortList = DataContext.GetCountryPorts();
            ViewBag.CompanyList = DataContext.GetCompany();
            ViewBag.AgencyList = DataContext.GetAgencyType();

            if (submit == "Save")
            {
                if (ModelState.IsValid)
                {
                    ErrorLog errorLog = DataContext.SaveRateRequest(rateRequest);
                    if (!errorLog.IsError)
                    {
                        rateRequest.RateID = errorLog.ErrorMessage;
                        TempData["Message"] = "Rate Request successfully created RateID " + rateRequest.RateID;
                    }
                    else
                    {
                        TempData["Message"] = errorLog.ErrorMessage;
                    }
                }
                else
                {
                    TempData["Message"] = "Please check your fields, some of the fields are not in correct format";
                    return View(rateRequest);
                }
                ModelState.Clear();
                if (rateRequest.ExportLocalCharges == null)
                {
                    rateRequest.ExportLocalCharges = new List<SelectListItem>();
                }
                if (rateRequest.ImportLocalCharges == null)
                {
                    rateRequest.ImportLocalCharges = new List<SelectListItem>();
                }
                return RedirectToAction("UpdateRateRequestDetails", "RateRequestDetails", new { rateID = rateRequest.RateID });
            }
            if (submit == "Get Agency Charges")
            {
                FetchCharges(rateRequest);
            }

            if (rateRequest.ExportLocalCharges == null)
            {
                rateRequest.ExportLocalCharges = new List<SelectListItem>();
            }
            if (rateRequest.ImportLocalCharges == null)
            {
                rateRequest.ImportLocalCharges = new List<SelectListItem>();
            }

            return View(rateRequest);
        }

        [HttpGet]
        [Authorize]
        //[Route("RateRequest/UpdateRateRequestDetails/{rateID}")]
        public ActionResult UpdateRateRequestDetails(string rateID)
        {
            //UpdateRateRequest ratereq = new UpdateRateRequest { Ids = new List<int>(), LocalCharges = new List<SelectList>() };
            var rateRequestDetails = DataContext.GetRateRequestFromRateID(rateID);
            ViewBag.PortList = DataContext.GetCountryPorts();
            ViewBag.shipmentlist = DataContext.ShipmentTerm();
            ViewBag.equipmentlist = DataContext.EquipmentType();
            ViewBag.CompanyList = DataContext.GetCompany();
            ViewBag.AgencyList = DataContext.GetAgencyType();

            if (rateRequestDetails.ExportLocalCharges == null)
            {
                rateRequestDetails.ExportLocalCharges = new List<SelectListItem>();
            }
            if (rateRequestDetails.ImportLocalCharges == null)
            {
                rateRequestDetails.ImportLocalCharges = new List<SelectListItem>();
            }
            TempData["rateID"] = rateID;
            return View(rateRequestDetails);
        }

        [HttpPost]
        public ActionResult UpdateRateRequestDetails(UpdateRateRequest rateRequest, string submit)
        {
            ViewBag.PortList = DataContext.GetCountryPorts();
            ViewBag.shipmentlist = DataContext.ShipmentTerm();
            ViewBag.equipmentlist = DataContext.EquipmentType();
            ViewBag.CompanyList = DataContext.GetCompany();
            ViewBag.AgencyList = DataContext.GetAgencyType();

            if (submit == "Save")
            {
                if (ModelState.IsValid)
                {
                    ErrorLog errorLog = DataContext.UpdateRateRequest(rateRequest);
                    if (!errorLog.IsError)
                    {
                        TempData["message"] = "Rate Request successfully updated";
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
                return RedirectToAction("UpdateRateRequestDetails", "RateRequestDetails", new { rateID = rateRequest.RateID });
            }
            if (submit == "Get Agency Charges")
            {
                FetchCharges(rateRequest);
            }
            if (rateRequest.ExportLocalCharges == null)
            {
                rateRequest.ExportLocalCharges = new List<SelectListItem>();
            }
            if (rateRequest.ImportLocalCharges == null)
            {
                rateRequest.ImportLocalCharges = new List<SelectListItem>();
            }

            return View(rateRequest);
        }

        //[HttpGet]
        //[ValidateAntiForgeryToken]
        public void FetchCharges(RateRequest RateRequest)
        {
            RateRequest.ExportLocalCharges = DataContext.GetExportChargesList(RateRequest).Select(x => new SelectListItem { Text = x.ChargeDescription, Value = x.ChargeDescription });

            RateRequest.ImportLocalCharges = DataContext.GetImportChargesList(RateRequest).Select(x => new SelectListItem { Text = x.ChargeDescription, Value = x.ChargeDescription });
        }

        public void FetchCharges(UpdateRateRequest RateRequest)
        {
            RateRequest.ExportLocalCharges = DataContext.GetExportChargesList(RateRequest).Select(x => new SelectListItem { Text = x.ChargeDescription, Value = x.ChargeDescription });

            RateRequest.ImportLocalCharges = DataContext.GetImportChargesList(RateRequest).Select(x => new SelectListItem { Text = x.ChargeDescription, Value = x.ChargeDescription });
        }

        [HttpGet]
        public ActionResult Approval(string rateID)
        {
            //if (ModelState.IsValid)
            ViewBag.PortList = DataContext.GetCountryPorts();
            ViewBag.shipmentlist = DataContext.ShipmentTerm();
            ViewBag.equipmentlist = DataContext.EquipmentType();
            ViewBag.AgencyList = DataContext.GetAgencyType();
            ViewBag.CompanyList = DataContext.GetCompany();
            ErrorLog errorLog = DataContext.ApprovalRequest(rateID);
            UpdateRateRequest update = DataContext.GetRateRequestFromRateID(rateID);
            if (!errorLog.IsError)
            {
                TempData["message"] = "Rate Request successfully submitted for Approval";
                //MailSend(rateID);
            }
            else
            {
                TempData["message"] = errorLog.ErrorMessage;
            }

            return RedirectToAction("UpdateRateRequestDetails", "RateRequestDetails", new { rateID = update.RateID });
        }

        [HttpGet]
        public ActionResult Duplicate(string rateID)
        {
            //if (ModelState.IsValid)
            ViewBag.PortList = DataContext.GetCountryPorts();
            ViewBag.shipmentlist = DataContext.ShipmentTerm();
            ViewBag.equipmentlist = DataContext.EquipmentType();
            ViewBag.CompanyList = DataContext.GetCompany();
            ViewBag.AgencyList = DataContext.GetAgencyType();

            UpdateRateRequest rateRequest = DataContext.GetRateRequestFromRateID(rateID);

            RateRequest raterequestcopy = new RateRequest();
            if (ModelState.IsValid)
            {
                {
                    raterequestcopy.CargoType = rateRequest.CargoType;
                    raterequestcopy.CompanyName = rateRequest.CompanyName;
                    raterequestcopy.AgencyType = rateRequest.AgencyType;
                    raterequestcopy.TransshipmentPort = rateRequest.TransshipmentPort;
                    raterequestcopy.TransshipmentType = rateRequest.TransshipmentType;
                    raterequestcopy.RateType = rateRequest.RateType;
                    raterequestcopy.Temperature = rateRequest.Temperature;
                    raterequestcopy.Ventilation = rateRequest.Ventilation;
                    raterequestcopy.Humidity = rateRequest.Humidity;
                    raterequestcopy.ShipperName = rateRequest.ShipperName;
                    raterequestcopy.GrossWt = rateRequest.GrossWt;
                    raterequestcopy.GrossWtUnit = rateRequest.GrossWtUnit;
                    raterequestcopy.DischargePort = rateRequest.DischargePort;
                    raterequestcopy.EffectiveDate = rateRequest.EffectiveDate;
                    raterequestcopy.EquipmentType = rateRequest.EquipmentType;
                    raterequestcopy.ExportLocalCharges = rateRequest.ExportLocalCharges;
                    raterequestcopy.IDCompany = rateRequest.IDCompany;
                    raterequestcopy.ImportLocalCharges = rateRequest.ImportLocalCharges;
                    raterequestcopy.LoadPort = rateRequest.LoadPort;
                    raterequestcopy.PlaceOfDelivery = rateRequest.PlaceOfDelivery;
                    raterequestcopy.PlaceOfReceipt = rateRequest.PlaceOfReceipt;
                    raterequestcopy.PODFreeDays = rateRequest.PODFreeDays;
                    raterequestcopy.POLFreeDays = rateRequest.POLFreeDays;
                    raterequestcopy.Quantity = rateRequest.Quantity;
                    raterequestcopy.Rate = rateRequest.Rate;
                    raterequestcopy.RateCountered = rateRequest.RateCountered;
                    raterequestcopy.Remark = rateRequest.Remark;
                    raterequestcopy.UNNo = rateRequest.UNNo;
                    raterequestcopy.IMO = rateRequest.IMO;
                    raterequestcopy.SelectedExportLocalCharges = rateRequest.SelectedExportLocalCharges;
                    raterequestcopy.SelectedImportLocalCharges = rateRequest.SelectedImportLocalCharges;
                    raterequestcopy.ShipmentTerm = rateRequest.ShipmentTerm;
                    raterequestcopy.ValidityDate = rateRequest.ValidityDate;
                    FetchCharges(raterequestcopy);
                }
                ModelState.Clear();
                TempData["raterequestobj"] = raterequestcopy;
                return RedirectToAction("RateRequestDetails", "RateRequestDetails");

            }

            else
            {
                TempData["Message"] = "Please check your fields, some of the fields are not in correct format";
                return RedirectToAction("UpdateRateRequestDetails", "RateRequestDetails", new { rateID = rateRequest.RateID });
            }
        }

        public JsonResult GetQuoteChargesList(string sidx, string sord, int page, int rows)//, SearchParameters statusdisplay)
        {
            string rateID = (string)TempData["rateID"];
            var quoteChargesdata = DataContext.GetRateChargesList(rateID);
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
                                //rateRequestGrid.ID,
                                rateRequestGrid.Description
                            }
                        }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        
        public void MailSend(string rateID)
        {
            if (ModelState.IsValid)
            {
                string username = HttpContext.User.Identity.Name;
                MailMessage mail = new MailMessage();
                mail.To.Add("anket.nemlekar@legendlogisticsltd.com");
                mail.From = new MailAddress("lcms@legendlogisticsltd.com");
                mail.Subject = "RateID "+ rateID +" sent for Approval";
                string Body =string.Format("{0} has sent Rate Request with RateID {1} for Approval", username,rateID);
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.office365.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("lcms@legendlogisticsltd.com", "CtrInd@legend"); // Enter senders User name and password  
                smtp.EnableSsl = true;
                smtp.Send(mail);
                
            }
            else
            {
                
            }
        }
    }
}
