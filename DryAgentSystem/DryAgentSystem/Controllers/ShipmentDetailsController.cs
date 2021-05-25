using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DryAgentSystem.Data;
using DryAgentSystem.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace DryAgentSystem.Controllers
{
    public class ShipmentDetailsController : Controller
    {
        // GET: ShipmentDetails
        [HttpGet]
        [Authorize]
        public ActionResult ShipmentDetails(string JobRef)
        {
            ShipmentBL shiprefdetails = DataContext.GetShipmentFromJobRef(JobRef);
            ViewBag.CompanyList = DataContext.GetCompany();
            ViewBag.pktypeList = DataContext.GetPackageType();
            ViewBag.wtunitlist = DataContext.GetWtUnit();
            ViewBag.munitlist = DataContext.GetMUnit();
            if (TempData["shipmentobj"] != null)
            {
                shiprefdetails = (ShipmentBL)TempData["shipmentobj"];
            }
            TempData["UniversalSerialNr"] = shiprefdetails.ShipmentDetailsModel.UniversalSerialNr;
            Session["UniversalSerialNr"] = shiprefdetails.ShipmentDetailsModel.UniversalSerialNr;
            if (shiprefdetails.ShipmentDetailsModel.ContainerList == null)
            {
                shiprefdetails.ShipmentDetailsModel.ContainerList = DataContext.GetContainerList(shiprefdetails.ShipmentDetailsModel.LoadPort).Select(x => new SelectListItem { Text = x.ContainerNo, Value = x.ContainerNo });
            }
            if (shiprefdetails.ShipmentDetailsModel.ClosingDate.ToString() == "1/1/0001 12:00:00 AM")
            {
                shiprefdetails.ShipmentDetailsModel.ClosingDate = DateTime.Today;
            }
            //if(TempData["invoicesave"] != null)
            //{
            //    shiprefdetails.ShipmentDetailsModel.InvoiceSave = (bool)TempData["invoicesave"];
            //}
            //else
            //{
            //    shiprefdetails.ShipmentDetailsModel.InvoiceSave = false;
            //}

            return View(shiprefdetails);
        }
        
        [HttpPost]
        public ActionResult ShipmentDetails(ShipmentBL shipment, string submit)
        {
            //ViewBag.PortList = DataContext.GetCountryPorts();
            ViewBag.pktypeList = DataContext.GetPackageType();
            ViewBag.wtunitlist = DataContext.GetWtUnit();
            ViewBag.munitlist = DataContext.GetMUnit();
            //ViewBag.bltypeList = DataContext.GetBLType();

            if (submit == "Save")
            {
                if (ModelState.IsValid)
                {
                    ErrorLog errorLog = DataContext.SaveShipment(shipment);
                    if (!errorLog.IsError)
                    {
                        shipment.BLDetailsModel.JobRef = errorLog.ErrorMessage;
                        TempData["Message"] = "Shipment Request successfully created Job Ref " + shipment.BLDetailsModel.JobRef;
                    }
                    else
                    {
                        TempData["Message"] = errorLog.ErrorMessage;
                    }
                }

                ModelState.Clear();
                return RedirectToAction("ShipmentDetails", "ShipmentDetails", new { JobRef = shipment.BLDetailsModel.JobRef});
            }
            if (submit == "Update")
            {
                if (ModelState.IsValid)
                {
                    ErrorLog errorLog = DataContext.UpdateShipment(shipment);
                    if (!errorLog.IsError)
                    {
                        TempData["message"] = "Shipment Request successfully updated";
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
                return RedirectToAction("ShipmentDetails", "ShipmentDetails", new { JobRef = shipment.ShipmentDetailsModel.JobRef});
            }
            if (submit == "Allocate Containers")
            {
                var tankData = DataContext.GetTanksDetails(shipment.ShipmentDetailsModel.UniversalSerialNr);
                int totalRecords = tankData.Count() + shipment.ShipmentDetailsModel.SelectedContainerList.Count;

                if (totalRecords<=shipment.ShipmentDetailsModel.Quantity)
                {
                    ErrorLog errorLog = DataContext.CreateTank(shipment.ShipmentDetailsModel);
                    if (!errorLog.IsError)
                    {
                        TempData["message"] = "Containers are Allocated successfully";
                    }
                    else
                    {
                        TempData["message"] = errorLog.ErrorMessage;
                    }
                }
                else
                {
                    TempData["Message"] = "No. of Allocated containers " + totalRecords + " cannot be more than quantity " + shipment.ShipmentDetailsModel.Quantity + ".\n Please reduce the no. of containers and try again";
                }
                
                if (shipment.ShipmentDetailsModel.ContainerList == null)
                {
                    shipment.ShipmentDetailsModel.ContainerList = DataContext.GetContainerList(shipment.ShipmentDetailsModel.LoadPort).Select(x => new SelectListItem { Text = x.ContainerNo, Value = x.ContainerNo });
                }
                TempData["shipmentobj"] = shipment;
                return ShipmentDetails(shipment.ShipmentDetailsModel.JobRef);
            }
            if(submit == "Print Shipping Instruction")
            {
                shipment.BLDetailsModel.UniversalSerialNr = shipment.ShipmentDetailsModel.UniversalSerialNr;
                ErrorLog errorLog = DataContext.SaveShippingInstruction(shipment.BLDetailsModel);
                PrintBL("ShippingInstruction", errorLog.ErrorMessage);
                return RedirectToAction("ShipmentDetails", "ShipmentDetails", new { JobRef = shipment.ShipmentDetailsModel.JobRef });
            }
            
            else
            {
                return View(shipment);
            }

        }

        public JsonResult GetVesselDetails(string sidx, string sort, int page, int rows)
        {
            string universalSerialNr = string.Empty;
            if (TempData["UniversalSerialNr"] != null)
            {
                universalSerialNr = TempData["UniversalSerialNr"].ToString();
                TempData.Keep("UniversalSerialNr");

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

        public JsonResult selectAdd(string name)
        {
            List<SelectListItem> companies = DataContext.GetCompany();
            return Json(companies.Where(x => x.Text == name), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ShippingInstruction(string ShipperNameSI, string ShipperAddressSI, string ConsigneeNameSI, string ConsigneeAddressSI, string UniversalSerialNr)
        {
            BLDetails bLDetails = new BLDetails();
            bLDetails.ShipperNameSI = ShipperNameSI;
            bLDetails.ShipperAddressSI = ShipperAddressSI;
            bLDetails.ConsigneeNameSI = ConsigneeNameSI;
            bLDetails.ConsigneeAddressSI = ConsigneeAddressSI;
            bLDetails.UniversalSerialNr = UniversalSerialNr;
            ErrorLog errorLog = DataContext.SaveShippingInstruction(bLDetails);
            //PrintBL("ShippingInstruction", "");

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);

        }


        public void PrintBL(string PrintName, String jobref)
        {
            DataContext.ChangeBLType(PrintName, jobref);
            ShipmentBL shipment = DataContext.GetShipmentFromJobRef(jobref);
            List<AllocateEquipment> allocateEquipment = DataContext.GetTanksDetails(shipment.ShipmentDetailsModel.UniversalSerialNr);


            Document pdfDoc = new Document(PageSize.A4, 40, 40, 20, 20);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.AddTitle("Bill Of Lading "+PrintName);
            pdfDoc.Open();
            //Chunk chunk;
            Paragraph para, para1;
            PdfPTable table;
            PdfPCell cell;

            para = new Paragraph("FOR COMBINED TRANSPORT OR OCEAN TRANSPORT OR MULTIMODAL TRANSPORT", FontFactory.GetFont("Arial", 11, Font.BOLD, new BaseColor(0, 0, 128)));
            pdfDoc.Add(para);

            //BL table starts here
            table = new PdfPTable(2);
            table.WidthPercentage = 100f;
            table.SpacingBefore = 10f;
            table.SpacingAfter = 0f;

            if(PrintName=="ShippingInstruction")
            {
                para = new Paragraph("Shipper", FontFactory.GetFont("Arial", 7, Font.BOLD, new BaseColor(0, 0, 128)));
                para1 = new Paragraph(shipment.BLDetailsModel.ShipperNameSI +
                    "\n" + shipment.BLDetailsModel.ShipperAddressSI +
                    "\n\n\n\n", FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
            }
            else
            {
                para = new Paragraph("Shipper", FontFactory.GetFont("Arial", 7, Font.BOLD, new BaseColor(0, 0, 128)));
                para1 = new Paragraph(shipment.BLDetailsModel.ShipperNameBL +
                    "\n" + shipment.BLDetailsModel.ShipperAddressBL +
                    "\n\n\n\n", FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
            }
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 3f;
            cell.PaddingTop = 0f;
            cell.Rowspan = 2;
            cell.Border = 0;
            cell.BorderWidthBottom = 0.5f;
            cell.BorderColor = new BaseColor(0, 32, 96);
            cell.AddElement(para);
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph("Bill of Lading No.:          " + jobref, FontFactory.GetFont("Arail", 8, Font.ITALIC, new BaseColor(0, 0, 128)));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.PaddingTop = 0f;
            cell.BorderWidth = 0.8f;
            cell.BorderColor = new BaseColor(0, 32, 96);
            cell.AddElement(para1);
            table.AddCell(cell);

            Image image = Image.GetInstance(Server.MapPath("~/Content/Img/BLLegendLogo.png"));
            image.ScaleAbsolute(250, 250);
            para1 = new Paragraph(PrintName, FontFactory.GetFont("Arial", 15, Font.BOLD, new BaseColor(0, 0, 128)));
            para1.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Rowspan = 2;
            cell.Padding = 5f;
            cell.BorderWidthRight = 0;
            cell.BorderWidthBottom = 0.5f;
            cell.BorderColor = new BaseColor(0, 32, 96);
            cell.AddElement(image);
            cell.AddElement(para1);
            table.AddCell(cell);

            if(PrintName=="ShippingInstruction")
            {
                para = new Paragraph("Consignee (if 'To Order' so indicate)", FontFactory.GetFont("Arial", 7, Font.BOLD, new BaseColor(0, 0, 128)));
                para1 = new Paragraph(shipment.BLDetailsModel.ConsigneeNameSI +
                    "\n" + shipment.BLDetailsModel.ConsigneeAddressSI +
                    "\n\n\n", FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
            }
            else
            {
                para = new Paragraph("Consignee (if 'To Order' so indicate)", FontFactory.GetFont("Arial", 7, Font.BOLD, new BaseColor(0, 0, 128)));
                para1 = new Paragraph(shipment.BLDetailsModel.ConsigneeNameBL +
                    "\n" + shipment.BLDetailsModel.ConsigneeAddressBL +
                    "\n\n\n", FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
            }
            
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.PaddingTop = 0f;
            cell.Border = 0;
            cell.BorderWidthBottom = 0.5f;
            cell.BorderColor = new BaseColor(0, 32, 96);
            cell.AddElement(para);
            cell.AddElement(para1);
            table.AddCell(cell);

            para = new Paragraph("Notify Party (No claim shall attached for failure to notify)", FontFactory.GetFont("Arial", 7, Font.BOLD, new BaseColor(0, 0, 128)));
            para1 = new Paragraph(shipment.BLDetailsModel.NotifyPartyName +
                "\n" + shipment.BLDetailsModel.NotifyPartyAddress +
                "\n\n\n\n", FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.PaddingTop = 0f;
            cell.Border = 0;
            cell.BorderWidthBottom = 1f;
            cell.BorderColor = new BaseColor(0, 32, 96);
            cell.AddElement(para);
            cell.AddElement(para1);
            table.AddCell(cell);

            para = new Paragraph("SHIPPING AGENT REFERENCES (COMPLETE NAME AND ADDRESS)", FontFactory.GetFont("Arial", 7, Font.BOLD, new BaseColor(0, 0, 128)));
            para1 = new Paragraph(shipment.BLDetailsModel.DischAgentNameBL +
                "\n" + shipment.BLDetailsModel.DischAgentAddress +
                "\n\n\n\n", FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.PaddingTop = 0f;
            cell.Border = 0;
            cell.BorderWidthTop = 0f;
            cell.BorderWidthLeft = 0.5f;
            cell.BorderWidthBottom = 1f;
            cell.BorderWidthRight = 0f;
            cell.BorderColor = new BaseColor(0, 32, 96);
            cell.AddElement(para);
            cell.AddElement(para1);
            table.AddCell(cell);

            para = new Paragraph("VESSEL/VOYAGE NO.", FontFactory.GetFont("Arial", 7, Font.BOLD, new BaseColor(0, 0, 128)));
            para1 = new Paragraph(shipment.VesselModel.VesselName + " \t " + shipment.VesselModel.VoyNo +
               "\n\n", FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.PaddingTop = 0f;
            cell.Border = 0;
            cell.BorderWidthLeft = 0f;
            cell.BorderWidthRight = 0f;
            cell.BorderWidthBottom = 1f;
            cell.BorderColor = new BaseColor(0, 32, 96);
            cell.AddElement(para);
            cell.AddElement(para1);
            table.AddCell(cell);

            para = new Paragraph("PRE-CARRIAGE BY", FontFactory.GetFont("Arial", 7, Font.BOLD, new BaseColor(0, 0, 128)));
            para1 = new Paragraph(shipment.BLDetailsModel.PreCarriageBy +
                "\n\n", FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.Border = 0;
            cell.PaddingTop = 0f;
            cell.BorderWidthLeft = 0f;
            cell.BorderWidthRight = 0f;
            cell.BorderWidthBottom = 1f;
            cell.BorderColor = new BaseColor(0, 32, 96);
            cell.AddElement(para);
            cell.AddElement(para1);
            table.AddCell(cell);

            para = new Paragraph("PORT OF LOADING ", FontFactory.GetFont("Arial", 7, Font.BOLD, new BaseColor(0, 0, 128)));
            para1 = new Paragraph(shipment.BLDetailsModel.LoadPort +
               "\n\n", FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.PaddingTop = 0f;
            cell.Border = 0;
            cell.BorderWidthLeft = 0f;
            cell.BorderWidthRight = 0f;
            cell.BorderWidthBottom = 1f;
            cell.BorderColor = new BaseColor(0, 32, 96);
            cell.AddElement(para);
            cell.AddElement(para1);
            table.AddCell(cell);

            para = new Paragraph("PLACE OF RECEIPT **", FontFactory.GetFont("Arial", 7, Font.BOLD, new BaseColor(0, 0, 128)));
            para1 = new Paragraph(shipment.BLDetailsModel.PlaceofReceipt +
                "\n\n", FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.Border = 0;
            cell.PaddingTop = 0f;
            cell.BorderWidthLeft = 0f;
            cell.BorderWidthRight = 0f;
            cell.BorderWidthBottom = 1f;
            cell.BorderColor = new BaseColor(0, 32, 96);
            cell.AddElement(para);
            cell.AddElement(para1);
            table.AddCell(cell);

            para = new Paragraph("PORT OF DISCHARGE", FontFactory.GetFont("Arial", 7, Font.BOLD, new BaseColor(0, 0, 128)));
            para1 = new Paragraph(shipment.BLDetailsModel.DischPort +
                "\n\n", FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.PaddingTop = 0f;
            cell.Border = 0;
            cell.AddElement(para);
            cell.AddElement(para1);
            table.AddCell(cell);

            para = new Paragraph("PLACE OF DELIVERY **", FontFactory.GetFont("Arial", 7, Font.BOLD, new BaseColor(0, 0, 128)));
            para1 = new Paragraph(shipment.BLDetailsModel.PlaceofDelivery +
                "\n\n", FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.PaddingTop = 0f;
            cell.Border = 0;
            cell.AddElement(para);
            cell.AddElement(para1);
            table.AddCell(cell);
            pdfDoc.Add(table);

            table = new PdfPTable(4);
            table.SetTotalWidth(new float[] { 140, 230, 70, 80 });
            table.LockedWidth = true;
            table.SpacingBefore = 0f;
            table.SpacingAfter = 3f;
            //table.SetExtendLastRow(true, false);
            //table.DefaultCell.FixedHeight = 500f;

            para1 = new Paragraph("PARTICULARS FURNISHED BY SHIPPER - NOT CHECKED BY CARRIER- CARRIER NOT RESPONSIBLE", FontFactory.GetFont("Arial", 8, Font.BOLD, new BaseColor(0, 0, 128)));
            para1.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 4;
            cell.Padding = 5f;
            cell.PaddingTop = 0f;
            cell.Border = 0;
            cell.BorderWidthTop = 3f;
            cell.BorderWidthBottom = 3f;
            cell.BorderColor = new BaseColor(0, 32, 96);
            cell.AddElement(para1);
            table.AddCell(cell);

            para = new Paragraph("Marks and Numbers", FontFactory.GetFont("Arial", 7, Font.BOLD, new BaseColor(0, 0, 128)));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.PaddingTop = 0f;
            cell.Border = 0;
            cell.BorderWidthBottom = 1f;
            cell.BorderWidthRight = 1f;
            cell.BorderWidthLeft = 0f;
            cell.BorderColor = new BaseColor(0, 32, 96);
            cell.AddElement(para);
            table.AddCell(cell);

            para1 = new Paragraph("\t\t\t Description of Packages & Goods" +
                "\n (Continued on attached Bill of Lading Rider Page(s),if applicated)", FontFactory.GetFont("Arial", 7, Font.BOLD, new BaseColor(0, 0, 128)));
            para1.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Padding = 5f;
            cell.PaddingTop = 0f;
            cell.Border = 0;
            cell.BorderWidthBottom = 1f;
            cell.BorderWidthRight = 1f;
            cell.BorderColor = new BaseColor(0, 32, 96);
            cell.AddElement(para1);
            table.AddCell(cell);

            para = new Paragraph("Gross Weight", FontFactory.GetFont("Arial", 7, Font.BOLD, new BaseColor(0, 0, 128)));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Padding = 5f;
            cell.PaddingTop = 3f;
            cell.Border = 0;
            cell.BorderWidthBottom = 1f;
            cell.BorderWidthRight = 1f;
            cell.BorderWidthLeft = 0f;
            cell.BorderColor = new BaseColor(0, 32, 96);
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("Measurement", FontFactory.GetFont("Arial", 7, Font.BOLD, new BaseColor(0, 0, 128)));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Padding = 5f;
            cell.PaddingTop = 3f;
            cell.Border = 0;
            cell.BorderWidthBottom = 1f;
            cell.BorderWidthRight = 0f;
            cell.BorderWidthLeft = 0f;
            cell.BorderColor = new BaseColor(0, 32, 96);
            cell.AddElement(para);
            table.AddCell(cell);

            //For Data
            para = new Paragraph(shipment.BLDetailsModel.MarksandNo, FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.PaddingTop = 0f;
            cell.Border = 0;
            cell.BorderWidthRight = 1f;
            cell.BorderWidthLeft = 0f;
            cell.BorderColor = new BaseColor(0, 32, 96);
            cell.AddElement(para);
            table.AddCell(cell);
                
            para1 = new Paragraph(shipment.BLDetailsModel.CargoDescription, FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
            para1.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Rowspan = 8;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Padding = 5f;
            cell.PaddingTop = 0f;
            cell.Border = 0;
            cell.BorderWidthRight = 1f;
            cell.BorderColor = new BaseColor(0, 32, 96);
            cell.AddElement(para1);
            table.AddCell(cell);

            /*
            decimal smd = Convert.ToDecimal(shipment.BLDetailsModel.TotalGweight);
            double smd2 = Convert.ToDouble(shipment.BLDetailsModel.TotalGweight);
            float sm3 = float.Parse(shipment.BLDetailsModel.TotalGweight);
            string sm4 = Math.Round(Convert.ToDouble(shipment.BLDetailsModel.TotalGweight), 2).ToString();

             string ch  = sm3.ToString("0.00");
             string ch1  = smd2.ToString("0.00");

            float.Parse(shipment.BLDetailsModel.TotalGweight).ToString("0.00")
            */

            para = new Paragraph(" ", FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Padding = 5f;
            cell.PaddingTop = 3f;
            cell.Border = 0;
            cell.BorderWidthRight = 1f;
            cell.BorderColor = new BaseColor(0, 32, 96);
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph(" ", FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Padding = 5f;
            cell.PaddingTop = 3f;
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            int CountingContainers = 0;

            for(int i = 0; i < allocateEquipment.Count; i++)
            {
                if(i > 6)
                {
                    break;
                }

                CountingContainers = CountingContainers + 1;

                para = new Paragraph(allocateEquipment[i].ContainerNo + " / " + allocateEquipment[i].SealNo, FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
                cell = new PdfPCell();
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Padding = 5f;
                cell.PaddingTop = 3f;
                cell.Border = 0;
                cell.BorderWidthRight = 1f;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph(float.Parse(allocateEquipment[i].GrossWeight).ToString("0.00"), FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Padding = 5f;
                cell.PaddingTop = 3f;
                cell.Border = 0;
                cell.BorderWidthRight = 1f;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph(float.Parse(allocateEquipment[i].Measurement).ToString("0.00"), FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Padding = 5f;
                cell.PaddingTop = 3f;
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);
            }

            if(CountingContainers < 7)
            {
                for(int i = 0; i < 7 - CountingContainers; i++)
                {
                    para = new Paragraph(" ", FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
                    cell = new PdfPCell();
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Padding = 5f;
                    cell.PaddingTop = 3f;
                    cell.Border = 0;
                    cell.BorderWidthRight = 1f;
                    cell.AddElement(para);
                    table.AddCell(cell);

                    para = new Paragraph(" ", FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
                    cell = new PdfPCell();
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Padding = 5f;
                    cell.PaddingTop = 3f;
                    cell.Border = 0;
                    cell.BorderWidthRight = 1f;
                    cell.AddElement(para);
                    table.AddCell(cell);

                    para = new Paragraph(" ", FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
                    cell = new PdfPCell();
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Padding = 5f;
                    cell.PaddingTop = 3f;
                    cell.Border = 0;
                    cell.AddElement(para);
                    table.AddCell(cell);
                }
            }

            //Last Row
            para = new Paragraph("Freight Payable at", FontFactory.GetFont("Arial", 7, Font.BOLD, new BaseColor(0, 0, 128)));
            para1 = new Paragraph(shipment.BLDetailsModel.HBLFreightPayment +
                "\n\n", FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.PaddingTop = 0f;
            cell.Border = 0;
            cell.BorderWidthRight = 1f;
            cell.BorderWidthTop = 3f;
            cell.BorderWidthBottom = 3f;
            cell.BorderColor = new BaseColor(0, 32, 96);
            cell.AddElement(para);
            cell.AddElement(para1);
            table.AddCell(cell);

            para = new Paragraph("No. of Original B/L issued", FontFactory.GetFont("Arial", 7, Font.BOLD, new BaseColor(0, 0, 128)));
            para1 = new Paragraph(shipment.BLDetailsModel.NoofOriginalBLissued +
                "\n\n", FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
            para1.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Padding = 5f;
            cell.PaddingTop = 0f;
            cell.Border = 0;
            cell.BorderWidthTop = 3f;
            cell.BorderWidthBottom = 3f;
            cell.BorderWidthRight = 1f;
            cell.BorderColor = new BaseColor(0, 32, 96);
            cell.AddElement(para);
            cell.AddElement(para1);
            table.AddCell(cell);

            para = new Paragraph("Place and date of issue", FontFactory.GetFont("Arial", 7, Font.BOLD, new BaseColor(0, 0, 128)));
            para1 = new Paragraph(shipment.BLDetailsModel.PlaceofIssue +
                "\n" + shipment.BLDetailsModel.BLFinalisedDate.ToString("dd-MM-yyyy") +
                "\n", FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 2;
            cell.Padding = 5f;
            cell.PaddingTop = 3f;
            cell.Border = 0;
            cell.BorderWidthTop = 3f;
            cell.BorderWidthBottom = 3f;
            cell.BorderColor = new BaseColor(0, 32, 96);
            cell.AddElement(para);
            cell.AddElement(para1);
            table.AddCell(cell);
            pdfDoc.Add(table);

            table = new PdfPTable(2);
            table.WidthPercentage = 101;
            table.SpacingBefore = 0f;
            table.SpacingAfter = 10f;

            para = new Paragraph("** applicable only when the documents is used as a Combined Transport Bill of Lading", FontFactory.GetFont("Arial", 7, Font.BOLD, new BaseColor(0, 0, 128)));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Colspan = 2;
            cell.Padding = 5f;
            cell.Border = 0;
            cell.BorderWidthBottom = 3f;
            cell.BorderColor = new BaseColor(0, 32, 96);
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("Laden On Board", FontFactory.GetFont("Arial", 7, Font.BOLD, new BaseColor(0, 0, 128)));
            para1 = new Paragraph(shipment.BLDetailsModel.LadenOnBoard.ToString("dd-MM-yyyy") +
                "\n\n", FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
            para.Alignment = Element.ALIGN_CENTER;
            para1.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Padding = 5f;
            cell.Border = 0;
            cell.BorderWidthRight = 1f;
            cell.BorderColor = new BaseColor(0, 32, 96);
            cell.AddElement(para);
            cell.AddElement(para1);
            table.AddCell(cell);

            para = new Paragraph("SIGNED as a Carrier", FontFactory.GetFont("Arial", 7, Font.BOLD, new BaseColor(0, 0, 128)));
            para1 = new Paragraph("LEGEND LOGISTICS (ASIA) PTE LTD OR ITS AGENT" +
                "\n\n", FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.Border = 0;
            cell.AddElement(para);
            cell.AddElement(para1);
            table.AddCell(cell);
            pdfDoc.Add(table);

            para1 = new Paragraph("ALL business is transacted only in accordance with Singapore Logistics Association's Standard Trading Conditions.", FontFactory.GetFont("Arial", 7, Font.BOLD, new BaseColor(0, 0, 128)));
            para1.Alignment = Element.ALIGN_CENTER;
            pdfDoc.Add(para1);

            if (PrintName == "ORIGINAL" || PrintName == "NON-NEGOTIABLE")
            {
                //Starts from new page
                pdfDoc.NewPage();

                para1 = new Paragraph("TERMS AND CONDITIONS OF BILL OF LADING", FontFactory.GetFont("Arial", 9, Font.BOLD));
                para1.Alignment = Element.ALIGN_CENTER;
                para1.SpacingBefore = 20f;
                para1.SpacingAfter = 5f;
                pdfDoc.Add(para1);

                table = new PdfPTable(2);
                table.WidthPercentage = 100;
                table.SpacingAfter = 0f;
                table.SplitLate = false;

                Paragraph Para01 = new Paragraph("1. DEFINITION.", FontFactory.GetFont("Arial", 3.6f, Font.BOLD));
                Paragraph Content01 = new Paragraph("a) “Bill of Lading” as used herein includes conventional bills of lading, as well as electronic, express and laser bills of lading, sea waybills and all like documents," +
                    "howsoever generated, covering the Carriage of Goods hereunder, whether or not issued to the Merchant." +
                    "\nb) “Carriage” means the whole of the operations and services undertaken or performed by or on behalf of the Carrier with respect to the Goods." +
                    "\nc) “Carrier” means the Company named on the face side hereof and on whose behalf this Bill of Lading was issued, whether acting as carrier or bailee." +
                    "\nd) “Charges” means freight, deadfreight, demurrage and all expenses and money obligations incurred and payable by the Merchant." +
                    "\ne) “Container” means any container(closed or open top), van, trailer, flatbed, transportable tank, railroad car, vehicle, flat, flatrack, pallet, skid, platform, cradle, sling - load" +
                    "or any other article of transport." +
                    "\nf) “Goods” means the cargo received from the shipper and described on the face side hereof and any Container not supplied by or on behalf of the Carrier." +
                    "\ng) “Merchant” means the shipper, consignee, receiver, holder of this Bill of Lading, owner of the cargo or person entitled to the possession of the cargo and the servants" +
                    "and agents of any of these, all of whom shall be jointly and severally liable to the Carrier for the payment of all Charges, and for the performance of the obligations of any" +
                    "of them under this Bill of Lading." +
                    "\nh) “On Board” or similar words endorsed on this Bill of Lading mean that in a Port to Port movement, the Goods have been loaded on board the Vessel or are in the" +
                    "custody of the actual ocean carrier; and in the event of Intermodal transportation, if the originating carrier is an inland or coastal carrier, means that the Goods have been" +
                    "loaded on board rail cars or another mode of transport at the Place of Receipt or are in the custody of a Participating carrier and en route to the Port of Loading named on" +
                    "the reverse side." +
                    "\ni) “Participating carrier” means any other carrier by water, land or air, performing any part of the Carriage, including inland carriers, whether acting as sub-carrier," +
                    "connecting carrier, substitute carrier or bailee." +
                    "\nj) “Person” means an individual, a partnership, a body corporate or any other entity of whatsoever nature." +
                    "\nk) “Vessel” means the ocean vessel named on the face side hereof, and any substitute vessel, feedership, barge, or other means of conveyance by water used in whole" +
                    "or in part by the Carrier to fulfill this contract.", FontFactory.GetFont("Arial", 3.6f));

                Paragraph Para02 = new Paragraph("2. CARRIER’S TARIFFS.", FontFactory.GetFont("Arial", 3.6f, Font.BOLD));
                Paragraph Content02 = new Paragraph("The Goods carried hereunder are subject to all terms and conditions of the Carrier’s applicable tariff(s), which are hereby incorporated herein.Copies of the relevant" +
                    "provisions of the applicable tariff(s) are obtainable from the Carrier upon request. In the event of any conflict between the terms and conditions of such tariff(s) and the" +
                    "Terms and Conditions of this Bill of Lading, this Bill of Lading shall prevail.", FontFactory.GetFont("Arial", 3.6f));

                Paragraph Para03 = new Paragraph("3. WARRANTY/ACKNOWLEDGMENT.", FontFactory.GetFont("Arial", 3.6f, Font.BOLD));
                Paragraph Content03 = new Paragraph("The Merchant warrants that in agreeing to the Terms and Conditions hereof, it is, or is the agent and has the authority of, the owner or person entitled to the possession" +
                    "of the Goods or any person who has a present or future interest in the Goods.When the Merchant instructs or as a matter of course permits the Carrier or its agents to" +
                    "prepare and release one or more original Bills of Lading to the consignee, the Merchant understands and agrees that such instruction or course of dealing, once provided" +
                    "or allowed, is irrevocable by the Merchant regarding this shipment, and the Carrier is without any responsibility or liability upon delivery of the cargo pursuant to said" +
                    "instruction or course of dealing and any and all revocations by the Merchant to be completely null and void.", FontFactory.GetFont("Arial", 3.6f));

                Paragraph Para04 = new Paragraph("4. RESPONSIBILITY.", FontFactory.GetFont("Arial", 3.6f, Font.BOLD));
                Paragraph Content04 = new Paragraph("a) Except where the Carriage covered by this Bill of Lading is to or from a port or locality where there is in force a compulsorily applicable ordinance or statute similar in" +
                    "nature to the International Convention for the Unification of Certain Rules Relating to Bills of Lading, dated at Brussels, August 25, 1924, the provisions of which cannot" +
                    "be departed from, this Bill of Lading shall have effect subject to the Carriage of Goods by Sea Act of the United States(COGSA), approved April 16, 1936, and nothing" +
                    "herein contained, unless otherwise stated, shall be deemed a surrender by the Carrier of any of its rights, immunities, exemptions, limitations or exonerations or an" +
                    "increase of any of its responsibilities or liabilities under COGSA or, as the case may be, such ordinances or statutes.The provisions of COGSA or such compulsorily" +
                    "applicable ordinances or statutes(except as otherwise specifically provided herein) shall govern before loading on and after discharge from the vessel and throughout the" +
                    "entire time the Goods or Containers or other packages are in the care, custody and/or control of the Carrier, its agents, servants, Participating carriers or independent" +
                    "contractors(inclusive of all subcontractors), whether engaged by or acting for the Carrier or any other person, and during the entire time the Carrier is responsible for the" +
                    "Goods." +
                    "\nb) The Carrier shall not be liable in any capacity whatsoever for any delay, non-delivery, misdelivery, acts of thieves, hijacking, act of God, force majeure, quarantine," +
                    "strikes or lockouts, riots or civil disobedience or any other loss or damage to or in connection with the Goods or Containers or other packages occurring at any time" +
                    "contemplated under subdivision a) of this Clause." +
                    "\nc) The Carrier shall, irrespective of which law is applicable under subdivision a) of this Clause, be entitled to the benefit of the provisions of Sections 4281 to 4287" +
                    "inclusive, and 4289 of the Revised Statutes of the United States and amendments thereto from time to time made (46 U.S.Code, Sections 181 through 188), as if the" +
                    "same were expressly set forth herein, including but not limited to the Fire Statute, R.S. 4282 (46 U.S.Code, Section 182)." +
                    "\nd) The rights, defenses, exemptions, limitations of and exonerations from liability and immunities of whatsoever nature provided for in this Bill of Lading shall apply in any" +
                    "action or proceeding against the Carrier, its agents and servants and/or any Participating carrier or independent contractor.", FontFactory.GetFont("Arial", 3.6f));

                Paragraph Para05 = new Paragraph("5. THROUGH TRANSPORTATION", FontFactory.GetFont("Arial", 3.6f, Font.BOLD));
                Paragraph Content05 = new Paragraph("When either the Place of Receipt or Place of Delivery set forth herein is an inland point or place other than the Port of Loading (Through Transportation basis), the" +
                    "Carrier will procure transportation to or from the sea terminal and such inland point(s) or place(s) and, notwithstanding anything in this Bill of Lading, but always subject" +
                    "to Clause 4 hereof, the following shall apply:" +
                    "a) If the loss or damage arose during a part of the carriage herein made subject to COGSA or other legislation, as set forth in Clause 4 a) hereof, said legislation shall" +
                    "apply." +
                    "\nb) If the loss or damage not falling within a) above, but which concerns compulsorily applicable laws and would have applied if the Merchant had made a separate and" +
                    "direct contract with the Carrier, a Participating carrier or independent contractor, as referred to in Clause 4 a), then the liability of the Carrier, Participating carrier and" +
                    "independent contractor, their agents and servants, shall be subject to the provisions of such law.If it should be determined that the Carrier bears any responsibility for" +
                    "loss or damage occurring during the care, custody and/or control of any Participating carrier or independent contractor, and be subject to law compulsorily applicable to" +
                    "their bills of lading, receipts, tariffs and/or law, then the Carrier shall be entitled to all rights, defenses, immunities, exemptions, limitations of and exonerations from" +
                    "liability of whatsoever nature accorded under such bill of lading, receipt, tariff and/or applicable law, provided however, that nothing contained herein shall be deemed a" +
                    "surrender by the Carrier of any of its rights, defenses and immunities or an increase of any of its responsibilities or liabilities under this Bill of Lading, the Carrier’s" +
                    "applicable tariff or laws applicable or relating to such Carriage.", FontFactory.GetFont("Arial", 3.6f));

                Paragraph Para06 = new Paragraph("6. SUBCONTRACTING: BENEFICIARIES.", FontFactory.GetFont("Arial", 3.6f, Font.BOLD));
                Paragraph Content06 = new Paragraph("a) The Carrier shall be entitled to subcontract on any terms the whole or any part of the Carriage, loading, unloading, storing, warehousing, handling and any and all" +
                    "duties whatsoever undertaken by it in relation to the Goods or Containers." +
                    "\nb) It is understood and agreed that if it should be adjudged that any person or entity other than or in addition to the Carrier is under any responsibility with respect to the" +
                    "Goods, all exemptions, limitations of and exonerations from liability provided by law or by the Terms and Conditions hereof shall be available to all Carrier’s agents," +
                    "servants, employees, representatives, all Participating (including inland) carriers and all stevedores, terminal operators, warehousemen, crane operators, watchmen," +
                    "carpenters, ship cleaners, surveyors and all independent contractors whatsoever.In entering into this contract, the Carrier, to the extent of these provisions, does so not" +
                    "only on its own behalf but also as agent and trustee for the aforesaid persons." +
                    "\nc) The Carrier undertakes to procure such services as necessary and shall have the right at its sole discretion to select any mode of land, sea or air transport and to" +
                    "arrange participation by other carriers to accomplish the total or any part of the carriage from Port of Loading to Port of Discharge or from Place of Receipt to Place of" +
                    "Delivery, or any combination thereof, except as may be otherwise provided herein." +
                    "\nd) No agent or servant of the Carrier or other person or class named in subdivision b) hereof shall have power to waive or vary any of the terms hereof unless such waiver" +
                    "or variation is in writing and is specifically authorized or ratified in writing by an officer or director of the Carrier having actual authority to bind the Carrier to such waiver or" +
                    "variation.", FontFactory.GetFont("Arial", 3.6f));

                Paragraph Para07 = new Paragraph("7. MERCHANT’S RESPONSIBILITY: DESCRIPTION OF GOODS.", FontFactory.GetFont("Arial", 3.6f, Font.BOLD));
                Paragraph Content07 = new Paragraph("a) The description and particulars of the Goods set out on the face hereof and any description, particular or other representation appearing on the Goods or documents" +
                    "relating thereto are furnished by the Merchant, and the Merchant warrants to the Carrier that the description, particulars and any representation made, including, but not" +
                    "limited to, weight, content, measure, quantity, quality, condition, marks, numbers and value are correct." +
                    "\nb) The Merchant warrants it has complied with all applicable laws, regulations and requirements of Customs, port and other authorities and shall bear and pay all duties," +
                    "taxes, fines, imposts, expenses and losses incurred or suffered by reason thereof or by reason of any illegal, incorrect or insufficient marking, numbering, addressing or" +
                    "any other particulars relative to the Goods." +
                    "\nc) The Merchant further warrants that the Goods are packed in a manner adequate to withstand the ordinary risks of Carriage having regard to their nature and in" +
                    "compliance with all laws, regulations and requirements which may be applicable." +
                    "\nd) No Goods which are or may become dangerous, inflammable or damaging or which are or may become liable to damage any property or person whatsoever shall be" +
                    "tendered to the Carrier for Carriage without the Carrier’s prior express consent in writing and without the Container or other article of transport in which the Goods are to" +
                    "be transported and the Goods being distinctly marked on the outside so as to indicate the nature and character of any such articles and as to comply with all applicable" +
                    "laws, regulations and requirements.If any such articles are delivered to the Carrier without such written consent and marking or if, in the opinion of the Carrier, the" +
                    "articles are or are liable to become of a dangerous, inflammable or damaging nature, the same may at any time be destroyed, disposed of, abandoned or rendered" +
                    "harmless without compensation to the Merchant and without prejudice to the Carrier’s right to Charges." +
                    "\ne) The Merchant shall be liable for all loss or damage of any kind whatsoever, including but not limited to, contamination, soiling, detention and demurrage before, during" +
                    "and after the Carriage of property(including but not limited to Containers) of the Carrier or any person(other than the Merchant) or vessel caused by the Merchant or any" +
                    "person acting on its behalf or for which the Merchant is otherwise responsible." +
                    "\nf) The Merchant shall defend, indemnify, and hold harmless the Carrier against any loss, damage, claim, liability or expense whatsoever arising from any breach of the" +
                    "provisions of this Clause 7 or from any cause in connection with the Goods for which the Carrier is not responsible.", FontFactory.GetFont("Arial", 3.6f));

                Paragraph Para08 = new Paragraph("8. ISO TANKS & CONTAINERS.", FontFactory.GetFont("Arial", 3.6f, Font.BOLD));
                Paragraph Content08 = new Paragraph("Goods may be stuffed by the Carrier in or on ISO Tanks/Containers, and may be stuffed with other goods.ISO Tanks/Containers, whether stuffed by the Carrier or" +
                    "received fully stuffed, may be carried on or under deck without notice, and the Merchant expressly agrees that cargo stuffed in an ISO Tank/Container and carried on" +
                    "deck is considered for all legal purposes to be cargo stowed under deck.Goods stowed in ISO Tanks/Containers on deck shall be subject to the legislation referred to in" +
                    "Clause 4. hereof and will contribute in General Average and receive compensation in General Average, as the case may be." +
                    "The Terms and Conditions of this Bill of Lading shall govern the responsibility of the Carrier with respect to the supply of an ISO Tank/Container to the Merchant." +
                    "If an ISO Tank/Container has been stuffed by or on behalf of the Merchant, the Carrier, any Participating carrier, all independent contractors and all persons rendering" +
                    "any service whatsoever hereunder shall not be liable for any loss or damage to the Goods, Containers or other packages or to any other goods caused \n(1) by the manner" +
                    "in which the ISO Tank/Container has been stuffed and its contents secured, \n(2) by the unsuitability of the Goods for carriage in ISO Tanks/Containers or for the ISO" +
                    "Tank /type of Container requested by and furnished to the Merchant, or \n(3) condition of the ISO Tank/Container furnished, which the Merchant acknowledges has been" +
                    "inspected by it or on its behalf before stuffing and sealing." +
                    "The Merchant shall defend, indemnify and hold harmless the Carrier, Participating carriers, independent contractors, their agents and servants, against any loss," +
                    "damage, claim, liability or expense whatsoever arising from one or more of the matters covered by a), b) and c) above.", FontFactory.GetFont("Arial", 3.6f));

                Paragraph Para09 = new Paragraph("9. CONTAINERS WITH REEFER APPARATUS", FontFactory.GetFont("Arial", 3.6f, Font.BOLD));
                Paragraph Content09 = new Paragraph("Containers with temperature or atmosphere control apparatus for refrigeration will not be furnished unless contracted for expressly in writing at time of booking and, when" +
                    "furnished, may entail increased Charges. In the absence of an express request, it shall be conclusively presumed that the use of a dry container is appropriate for the" +
                    "Goods." +
                    "Merchant must provide Carrier with desired temperature range in writing at time of booking and insert same on the face side of the Bill of Lading, and where so provided," +
                    "Carrier is to exercise due diligence to maintain the temperature within a range of plus or minus 5 degrees Fahrenheit of the temperature requested by the Merchant on" +
                    "the face hereof while the Containers are in its care, custody and/or control or that of any Participating carrier or independent contractor, their agents or servants. The" +
                    "Carrier does not accept any responsibility for the functioning of temperature or atmosphere-controlled Containers not owned or leased by Carrier or for latent defects not" +
                    "discoverable by the exercise of due diligence." +
                    "Where the Container is stuffed or partially stuffed by or on behalf of the Merchant, the Merchant warrants that it has properly pre-cooled the Container, that the Goods" +
                    "have been properly stuffed and secured within the Container and that the temperature controls have been properly set prior to delivery of the Container to the Carrier, its" +
                    "agents, servants, or any Participating carrier or independent contractor.The Merchant accepts responsibility for all damage or loss of whatsoever nature resulting from a" +
                    "breach of any of these warranties, including but not limited to other cargo consolidated in the Container with the Merchant’s Goods or to any other cargo, property or" +
                    "person damaged or injured as a result thereof, and the Merchant agrees to defend, indemnify and hold the Carrier, Participating carriers and independent contractors," +
                    "their agents and servants, harmless from and against all claims, suits, proceedings a nd other consequences thereof regardless of their nature and merit.", FontFactory.GetFont("Arial", 3.6f));

                Paragraph Para10 = new Paragraph("10. OPTION OF INSPECTION.", FontFactory.GetFont("Arial", 3.6f, Font.BOLD));
                Paragraph Content10 = new Paragraph("The Carrier and any Participating carrier shall be entitled, but under no obligation, to open any Container at any time and to inspect the contents. If it thereupon appears" +
                    "that the contents or any part thereof cannot safely or properly be carried or carried further, either at all or without incurring any additional expense, the Carrier and" +
                    "Participating carrier may abandon the transportation thereof and/or take any measures and/or incur any reasonable additional expenses to continue the Carriage or to" +
                    "store the Goods, which storage shall be deemed to constitute due delivery under this Bill of Lading.The Merchant shall indemnify the Carrier against any reasonable" +
                    "additional Charges so incurred.", FontFactory.GetFont("Arial", 3.6f));

                cell = new PdfPCell();
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Padding = 5f;
                cell.Border = 0;
                cell.AddElement(Para01);
                cell.AddElement(Content01);
                cell.AddElement(Para02);
                cell.AddElement(Content02);
                cell.AddElement(Para03);
                cell.AddElement(Content03);
                cell.AddElement(Para04);
                cell.AddElement(Content04);
                cell.AddElement(Para05);
                cell.AddElement(Content05);
                cell.AddElement(Para06);
                cell.AddElement(Content06);
                cell.AddElement(Para07);
                cell.AddElement(Content07);
                cell.AddElement(Para08);
                cell.AddElement(Content08);
                cell.AddElement(Para09);
                cell.AddElement(Content09);
                cell.AddElement(Para10);
                cell.AddElement(Content10);
                table.AddCell(cell);

                Paragraph Para11 = new Paragraph("11. METHODS AND ROUTES OF TRANSPORTATION", FontFactory.GetFont("Arial", 3.6f, Font.BOLD));
                Paragraph Content11 = new Paragraph("With respect to the Goods or Containers or other packages, the Carrier may at any time and without notice to the Merchant:" +
                    "\na) use any means of transport(water, land and / or air) or storage whatsoever; " +
                    "\nb) forward, transship or retain on board or carry on another vessel or conveyance or by any other means of transport than that named on the reverse side hereof; " +
                    "\nc) carry Goods on or under deck at its option; " +
                    "\nd) proceed by any route in its sole and absolute discretion and whether the nearest, most direct, customary or advertised route or in or out of geographical rotation; " +
                    "\ne) proceed to or stay at any place whatsoever once or more often and in any order or omit calling at any port, whether scheduled or not; " +
                    "\nf) store, vanned or devanned, at any place whatsoever, ashore or afloat, in the open or covered; " +
                    "\ng) proceed with or without pilots; " +
                    "\nh) carry livestock, contraband, explosives, munitions, warlike stores, dangerous or hazardous Goods or Goods of any and all kinds; " +
                    "\ni) drydock or stop at any unscheduled or unadvertised port for bunkers, repairs or for any purpose whatsoever; " +
                    "\nj) discharge and require the Merchant to take delivery, vanned or devanned; " +
                    "\nk) comply with any orders, directions or recommendations given by any government or authority or by any person or body acting or purporting to act with the" +
                    "authority of any government or authority or having under the terms of the insurance on the Vessel or other conveyance employed by the Carrier, the right to" +
                    "give such orders, directions or recommendations." +
                    "\nl) take any other steps or precautions as may appear reasonable to the Carrier under the circumstances." +
                    "The liberties set out in subdivisions a) through l) may be invoked for any purpose whatsoever even if not connected with the Carriage covered by this Bill of" +
                    "Lading, and any action taken or omitted to be taken, and any delay arising therefrom, shall be deemed to be within the contractual and contemplated Carriage" +
                    "and not be an unreasonable deviation", FontFactory.GetFont("Arial", 3.6f));

                Paragraph Para12 = new Paragraph("12. MATTERS AFFECTING PERFORMANCE.", FontFactory.GetFont("Arial", 3.6f, Font.BOLD));
                Paragraph Content12 = new Paragraph("In any situation whatsoever and wheresoever occurring and whether existing or anticipated before commencement of, during or after the Carriage, which in" +
                    "the judgment of the Carrier is likely to give rise to any hindrance, risk, capture, seizure, detention, damage, delay, difficulty or disadvantage or loss to the" +
                    "Carrier or any part of the Goods, or make it unsafe, imprudent, impracticable or unlawful for any reason to receive, keep, load, carry or discharge them or any" +
                    "part of them or commence or continue the Carriage at the Port of Discharge or of the usual or intended place of discharge or Delivery, or to give rise to" +
                    "danger, delay or difficulty of whatsoever nature in proceeding by the usual or intended route, the Carrier and any Participating carrier, without notice to the" +
                    "Merchant, may decline to receive, keep, load, carry or discharge the Goods, or may discharge the Goods and may require the Merchant to take delivery and," +
                    "upon failure to do so, may warehouse them at the risk and expense of the Merchant and Goods or may forward or transship them as provided in this Bill of" +
                    "Lading, or the Carrier may retain the Goods on board until the return of the Vessel to the Port of Loading or to the Port of Discharge or any other point or until" +
                    "such time as the Carrier deems advisable and thereafter discharge them at any place whatsoever.In such event, as herein provided, such shall be at the risk" +
                    "and expense of the Merchant and Goods, and such action shall constitute complete delivery and performance under this contract, and the Carrier shall be" +
                    "free from any further responsibility. For any service rendered as herein above provided or for any delay or expense to the Carrier, Participating carrier and/or" +
                    "Vessel caused as a result thereof, the Carrier shall, in addition to full Charges, be entitled to reasonable extra compensation, and shall have a lien on the" +
                    "Goods for same. Notice of disposition of the Goods shall be sent to the Merchant named in this Bill of Lading within a reasonable time thereafter." +
                    "All actions taken by the Carrier hereunder shall be deemed to be within the contractual and contemplated carriage and not be an unreasonable deviation." +
                    "In no circumstance whatsoever shall the Carrier be liable for direct, indirect or consequential loss, profit of any kind or damage caused by delay or any reason" +
                    "whatsoever", FontFactory.GetFont("Arial", 3.6f));

                Paragraph Para13 = new Paragraph("13. DELIVERY", FontFactory.GetFont("Arial", 3.6f, Font.BOLD));
                Paragraph Content13 = new Paragraph("If delivery of the Goods or Containers or other packages or any part thereof is not taken by the Merchant when and where and at such time and place as the" +
                    "Carrier is entitled to have the Merchant take delivery, they shall be considered to have been delivered to the Merchant, and thereafter always to be at the risk" +
                    "and expense of the Merchant and Goods." +
                    "If the Goods are stowed within a Container owned or leased by the Carrier, the Carrier shall be entitled to devan the contents of any such Container," +
                    "whereupon the Goods shall be considered to have been delivered to the Merchant and the Carrier, may at its option, subject to its lien and without notice," +
                    "elect to have same remain where they are or sent to a warehouse or other place, always at the risk and expense of the Merchant and Goods.", FontFactory.GetFont("Arial", 3.6f));

                Paragraph Para14 = new Paragraph("14. CHARGES, INCLUDING FREIGHT.", FontFactory.GetFont("Arial", 3.6f, Font.BOLD));
                Paragraph Content14 = new Paragraph("The Charges payable hereunder have been calculated on the basis of particulars furnished by or on behalf of the Merchant. The Carrier shall, at any time, be" +
                    "entitled to inspect, reweigh, remeasure or revalue the contents and, if any of the particulars furnished by the Merchant are found to be incorrect, the Charges" +
                    "shall be adjusted accordingly and the Merchant shall be responsible to pay the correct Charges and all expenses incurred by the Carrier in checking said" +
                    "particulars or any of them." +
                    "Charges shall be deemed earned on acceptance of the Goods or Containers or other packages for shipment by the Carrier and shall be paid by the Merchant" +
                    "in full, without any offset, counter claim or deduction, cargo and / or vessel or other conveyance lost, or not lost, and shall be non - returnable in any event." +
                    "The Merchant shall remain responsible for all Charges, regardless whether the Bill of Lading states, in words or symbols, that it is “Prepaid,” “to be Prepaid”" +
                    "or “Collect,” including, but not limited to, costs, expenses and reasonable attorneys’ fees incurred by the Carrier in pursuing Charges. Payment of Charges to" +
                    "a freight forwarder, broker or to anyone other than the Carrier shall not be deemed payment to the Carrier and shall be at the Merchant’s risk." +
                    "In arranging for any services with respect to the Goods, the Carrier shall be considered the exclusive agent of the Merchant for all purposes, and any" +
                    "payment of charges to other than the Carrier shall not, in any event, be considered payment to the Carrier." +
                    "The Merchant shall defend, indemnify and hold the Carrier, Participating carriers, independent contractors, their agents and servants, harmless from and" +
                    "against all liability, loss damage and expense which may be sustained or incurred relative to the above.", FontFactory.GetFont("Arial", 3.6f));

                Paragraph Para15 = new Paragraph("15. CARRIER’S LIEN.", FontFactory.GetFont("Arial", 3.6f, Font.BOLD));
                Paragraph Content15 = new Paragraph("The Carrier shall have a lien on the Goods and any documents relating thereto, inclusive of any Container owned or leased by the Merchant, as well as on" +
                    "any Charges of whatsoever nature due any other person, and any documents relating thereto, which lien shall survive delivery, for all sums due under this" +
                   "contract or any other contract or undertaking to which the Merchant was partly or otherwise involved, including, but not limited to, General Average" +
                    "contributions, salvage, demurrage and the cost of recovering such sums, inclusive of attorney fees.Such lien may be enforced by the Carrier by public or" +
                    "private sale at the expense of and without notice to the Merchant." +
                    "The Merchant agrees to defend, indemnify and hold the Carrier, Participating carriers, independent contractors, their agents and servants, harmless from" +
                    "and against all liability, loss, damage or expense which may be sustained or incurred by the Carrier relative to the above and the Merchant agrees to submit to" +
                    "the jurisdiction of any court, tribunal or other body before whom the Carrier may be brought, whether said proceeding is of a civil or criminal nature", FontFactory.GetFont("Arial", 3.6f));

                Paragraph Para16 = new Paragraph("16. RUST.", FontFactory.GetFont("Arial", 3.6f, Font.BOLD));
                Paragraph Content16 = new Paragraph("It is agreed that superficial rust, oxidation or any like condition due to moisture, is not a condition of damage but is inherent to the nature of the Goods." +
                    "Acknowledgement of receipt of the Goods in apparent good order and condition is not a representation that such conditions of rust, oxidation or the like did" +
                    "not exist on receipt.", FontFactory.GetFont("Arial", 3.6f));

                Paragraph Para17 = new Paragraph("17.BOTH-TO-BLAME COLLISION.", FontFactory.GetFont("Arial", 3.6f, Font.BOLD));
                Paragraph Content17 = new Paragraph("If the Vessel on which the Goods are carried (the carrying Vessel) comes into collision with any other vessel or object (the non-carrying vessel or object) as a" +
                    "result of the negligence of the non - carrying vessel or object or the owner of, charterer of, or person responsible for the non-carrying vessel or object, the" +
                    "Merchant undertakes to defend, indemnify and hold harmless the Carrier against all claims by or liability to(and any expense arising therefrom) any vessel or" +
                    "person in respect of any loss of or damage to, or any claim whatsoever of the Merchant paid or payable to the Merchant by the non - carrying vessel or object" +
                    "or the owner of, charterer of or person responsible for the non-carrying vessel or object and set off, recouped or recovered by such vessel, object or person" +
                    "against the Carrier, the carrying vessel or her owners or charterers.This provision is to remain in effect in other jurisdictions, even if unenforceable in the" +
                    "courts of the United States.", FontFactory.GetFont("Arial", 3.6f));

                Paragraph Para18 = new Paragraph("18.GENERAL AVERAGE", FontFactory.GetFont("Arial", 3.6f, Font.BOLD));
                Paragraph Content18 = new Paragraph("(a) If General Average is declared, it shall be adjusted according to the York/ Antwerp Rules of 1994 and all subsequent amendments thereto from time to" +
                    "time made, at any place at the option of any person entitled to declare General Average, and the Amended Jason Clause as approved by BIMCO is to be" +
                    "considered as incorporated herein, and the Merchant shall provide such security as may be required in this connection." +
                    "\nb) Notwithstanding a) above, the Merchant shall defend, indemnify and hold harmless the Carrier, Participating carriers, independent contractors, their" +
                    "agents and servants, in respect of any claim(and any expense arising therefrom) of a General Average nature which may be made against the Carrier and / or" +
                    "any Participating carrier and shall provide such security as may be required in this connection." +
                    "\nc) Neither the Carrier nor any Participating carrier shall be under any obligation to take any steps whatsoever to post security for General Average or to" +
                    "collect security for General Average contributions due the Merchant", FontFactory.GetFont("Arial", 3.6f));

                Paragraph Para19 = new Paragraph("19. LIMITATION OF LIABILITY.", FontFactory.GetFont("Arial", 3.6f, Font.BOLD));
                Paragraph Content19 = new Paragraph("Except as otherwise provided in this Clause or elsewhere in this Bill of Lading, in case of any loss or damage to or in connection with cargo exceeding in" +
                    "actual value the equivalent of $500 lawful money of the United States, per package, or in case of cargo not shipped in packages, per shipping unit, the value" +
                    "of the cargo shall be deemed to be $500 per package or per shipping unit.The Carrier’s liability, if any, shall be determined on the basis of a value of $500" +
                    "per package or per shipping unit or pro rata in case of partial loss or damage, unless the nature of the cargo and valuation higher than $500 per package or" +
                    "per shipping unit shall have been declared by the Merchant before shipment and inserted in this Bill of Lading, and extra freight paid if required.In such case," +
                    "if the actual value of the cargo per package or per shipping unit shall exceed such declared value, the value shall nevertheless be deemed to be declared" +
                    "value and the Carrier’s liability, if any, shall not exceed the declared value." +
                    "The words “shipping unit” shall mean each physical unit or piece of cargo not shipped in a package, including articles or things of any description" +
                    "whatsoever, except cargo shipped in bulk, and irrespective of the weight or measurement unit employed in calculating freight and related charges." +
                    "As to cargo shipped in bulk, the limitation applicable thereto shall be the limitation provided in Section 1304(5) of COGSA, or such other legislation," +
                    "convention or law as may be applicable, and in no event shall anything herein be construed as a waiver of limitation as to cargo shipped in bulk." +
                    "Where a Container is not stuffed by or on behalf of the Carrier or the parties characterize the Container as a package or a lump sum freight is assessed, in" +
                    "any of these events, each Container and its contents shall be deemed a single package and Carrier’s liability limited to $500 with respect to each such" +
                    "package, except as otherwise provided in this Clause or elsewhere in this Bill of Lading." +
                    "In the event this provision should be held invalid during that period in which compulsory legislation shall apply of its own force and effect, such as during the" +
                   "tackle-to-tackle period, it shall nevertheless apply during all non-compulsory periods such as, but not limited to, all periods prior to loading and subsequent to" +
                    "discharge from the Vessel for which the Carrier remains responsible.", FontFactory.GetFont("Arial", 3.6f));

                Paragraph Para20 = new Paragraph("20. NOTICE OF CLAIM: TIME FOR SUIT.", FontFactory.GetFont("Arial", 3.6f, Font.BOLD));
                Paragraph Content20 = new Paragraph("As to any loss or damage presumed to have occurred during the Carrier’s period of responsibility, the Carrier must be notified in writing of any such loss or" +
                    "damage or claim before or at the time of discharge / removal of the Goods by the Merchant or, if the loss or damage is not then apparent, within 3 consecutive" +
                    "days after discharge/ delivery or the date when the Goods should have been discharged/ delivered.If not so notified, discharge, removal or delivery, depending" +
                    "upon the law applicable, shall be prima facie evidence of discharge / delivery in good order by the Carrier of such Goods." +
                    "In any event, the Carrier shall be discharged from all liability of whatsoever nature unless suit is brought within 1 year after delivery of the Goods or the date" +
                    "when the Goods should have been delivered, provided however, that if any claim should arise during a part of the transport which is subject by applicable law" +
                    "and/or tariff and/or contract to a shorter period for notice of claim or commencement of suit, any liability whatsoever of the Carrier shall cease unless proper" +
                    "claim is made in writing and suit is brought within such shorter period." +
                    "Suit shall not be deemed “brought” unless jurisdiction is obtained over the Carrier by service of process or by an agreement to appear. In the event this" +
                   "provision is held invalid during that period in which compulsory legislation shall apply of its own force and effect, such as during the tackle-to-tackle period, it" +
                    "shall nevertheless apply during all non- compulsory periods during which the Carrier remains responsible.", FontFactory.GetFont("Arial", 3.6f));

                Paragraph Para21 = new Paragraph("21. LAW AND JURISDICTION", FontFactory.GetFont("Arial", 3.6f, Font.BOLD));
                Paragraph Content21 = new Paragraph("a.) Governing Law" +
                    "Insolar as anything has not been dealt with by the terms and conditions of this Bill of Lading, Singapore law shall apply.Singapore law shall in any event apply" +
                    "in interpreting the terms and conditions hereof." +
                    "\nb.) Jurisdiction" +
                    "All disputes relating to this Bill of Lading shall be determined by the Courts of Singapore to he exclusion of the jurisdiction of the courts of my other country" +
                    "provided always that the Carrier may in its absolute and sole discretion invoke or voluntarily submit to the jurisdiction of the Counts of any other country" +
                    "which, but for the terms of this Bill of Lading,could properly assume jurisdiction to hear and determine such disputes, but shall not constitute a waiver of the" +
                    "terms of this provision in any other instance.", FontFactory.GetFont("Arial", 3.6f));

                cell = new PdfPCell();
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Padding = 5f;
                cell.Border = 0;
                cell.AddElement(Para11);
                cell.AddElement(Content11);
                cell.AddElement(Para12);
                cell.AddElement(Content12);
                cell.AddElement(Para13);
                cell.AddElement(Content13);
                cell.AddElement(Para14);
                cell.AddElement(Content14);
                cell.AddElement(Para15);
                cell.AddElement(Content15);
                cell.AddElement(Para16);
                cell.AddElement(Content16);
                cell.AddElement(Para17);
                cell.AddElement(Content17);
                cell.AddElement(Para18);
                cell.AddElement(Content18);
                cell.AddElement(Para19);
                cell.AddElement(Content19);
                cell.AddElement(Para20);
                cell.AddElement(Content20);
                cell.AddElement(Para21);
                cell.AddElement(Content21);
                table.AddCell(cell);

                pdfDoc.Add(table);
            }


            if (CountingContainers >= 7)
            {
                //3rd Page
                pdfDoc.NewPage();
                table = new PdfPTable(4);
                table.WidthPercentage = 100;
                table.SpacingBefore = 0f;
                table.SpacingAfter = 10f;
                //table.DefaultCell.FixedHeight = 500f;

                para1 = new Paragraph("PARTICULARS FURNISHED BY SHIPPER - NOT CHECKED BY CARRIER- CARRIER NOT RESPONSIBLE", FontFactory.GetFont("Arial", 8, Font.BOLD, new BaseColor(0, 0, 128)));
                para1.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Colspan = 4;
                cell.Padding = 5f;
                cell.PaddingTop = 0f;
                cell.Border = 0;
                cell.BorderWidthTop = 3f;
                cell.BorderWidthBottom = 3f;
                cell.BorderColor = new BaseColor(0, 32, 96);
                cell.AddElement(para1);
                table.AddCell(cell);

                para = new Paragraph("Container No.", FontFactory.GetFont("Arial", 7, Font.BOLD, new BaseColor(0, 0, 128)));
                cell = new PdfPCell();
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Padding = 5f;
                cell.PaddingTop = 0f;
                cell.Border = 0;
                cell.BorderWidthBottom = 1f;
                cell.BorderWidthRight = 1f;
                cell.BorderWidthLeft = 0f;
                cell.BorderColor = new BaseColor(0, 32, 96);
                cell.AddElement(para);
                table.AddCell(cell);

                para1 = new Paragraph("Seal Number.", FontFactory.GetFont("Arial", 7, Font.BOLD, new BaseColor(0, 0, 128)));
                para1.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Padding = 5f;
                cell.PaddingTop = 0f;
                cell.Border = 0;
                cell.BorderWidthBottom = 1f;
                cell.BorderWidthRight = 1f;
                cell.BorderColor = new BaseColor(0, 32, 96);
                cell.AddElement(para1);
                table.AddCell(cell);

                para = new Paragraph("Gross Weight " + shipment.BLDetailsModel.GrossWeightUnit, FontFactory.GetFont("Arial", 7, Font.BOLD, new BaseColor(0, 0, 128)));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Padding = 5f;
                cell.PaddingTop = 3f;
                cell.Border = 0;
                cell.BorderWidthBottom = 1f;
                cell.BorderWidthRight = 1f;
                cell.BorderWidthLeft = 0f;
                cell.BorderColor = new BaseColor(0, 32, 96);
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph("Measurement " + shipment.BLDetailsModel.MeasurementUnit, FontFactory.GetFont("Arial", 7, Font.BOLD, new BaseColor(0, 0, 128)));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Padding = 5f;
                cell.PaddingTop = 3f;
                cell.Border = 0;
                cell.BorderWidthBottom = 1f;
                cell.BorderWidthRight = 0f;
                cell.BorderWidthLeft = 0f;
                cell.BorderColor = new BaseColor(0, 32, 96);
                cell.AddElement(para);
                table.AddCell(cell);


                //For Data
                for (int i = 0; i < allocateEquipment.Count - 7; i++)
                {
                    para = new Paragraph(allocateEquipment[i + 7].ContainerNo, FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
                    cell = new PdfPCell();
                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell.Padding = 5f;
                    cell.PaddingTop = 0f;
                    cell.Border = 0;
                    cell.BorderWidthRight = 1f;
                    cell.BorderWidthLeft = 0f;
                    cell.BorderColor = new BaseColor(0, 32, 96);
                    cell.AddElement(para);
                    table.AddCell(cell);

                    para1 = new Paragraph(allocateEquipment[i + 7].SealNo, FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
                    para1.Alignment = Element.ALIGN_CENTER;
                    cell = new PdfPCell();
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Padding = 5f;
                    cell.PaddingTop = 0f;
                    cell.Border = 0;
                    cell.BorderWidthRight = 1f;
                    cell.BorderColor = new BaseColor(0, 32, 96);
                    cell.AddElement(para1);
                    table.AddCell(cell);

                    para = new Paragraph(float.Parse(allocateEquipment[i + 7].GrossWeight).ToString("0.00"), FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
                    para.Alignment = Element.ALIGN_CENTER;
                    cell = new PdfPCell();
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Padding = 5f;
                    cell.PaddingTop = 3f;
                    cell.Border = 0;
                    cell.BorderWidthRight = 1f;
                    cell.BorderColor = new BaseColor(0, 32, 96);
                    cell.AddElement(para);
                    table.AddCell(cell);

                    para = new Paragraph(float.Parse(allocateEquipment[i + 7].Measurement).ToString("0.00"), FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
                    para.Alignment = Element.ALIGN_CENTER;
                    cell = new PdfPCell();
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Padding = 5f;
                    cell.PaddingTop = 3f;
                    cell.Border = 0;
                    cell.AddElement(para);
                    table.AddCell(cell);
                }

                para = new Paragraph("Total Gross Weight    :  " + float.Parse(shipment.BLDetailsModel.TotalGweight).ToString("0.00"), FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
                para.Alignment = Element.ALIGN_RIGHT;
                cell = new PdfPCell();
                cell.Colspan = 3;
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.Padding = 5f;
                cell.PaddingTop = 3f;
                cell.PaddingRight = 20f;
                cell.Border = 0;
                cell.BorderWidthTop = 1.5f;
                cell.BorderWidthBottom = 1.5f;
                cell.BorderWidthRight = 1f;
                cell.BorderColor = new BaseColor(0, 32, 96);
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph("Total Measurement    :  " + float.Parse(shipment.BLDetailsModel.TotalContainerMeasurement).ToString("0.00"), FontFactory.GetFont("Arial", 7, new BaseColor(0, 0, 128)));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Padding = 5f;
                cell.PaddingTop = 3f;
                cell.PaddingRight = 10f;
                cell.Border = 0;
                cell.BorderWidthTop = 1.5f;
                cell.BorderWidthBottom = 1.5f;
                cell.BorderColor = new BaseColor(0, 32, 96);
                cell.AddElement(para);
                table.AddCell(cell);

                pdfDoc.Add(table);

            }

            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "inline;filename=Bill Of Lading "+PrintName+".pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
        }

        public JsonResult GetTankDetails(string sidx, string sort, int page, int rows)
        {
            string universalSerialNr = string.Empty;
            if (TempData["UniversalSerialNr"] != null)
            {
                universalSerialNr = TempData["UniversalSerialNr"].ToString();

                var tankData = DataContext.GetTanksDetails(universalSerialNr);
                int totalRecords = tankData.Count();
                var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

                var jsonData = new
                {
                    total = totalPages,
                    page,
                    records = totalRecords,
                    rows = (from tankGrid in tankData
                            select new
                            {
                                tankGrid.ID,
                                cell = new string[]
                                {
                                    tankGrid.ID,
                                    tankGrid.ContainerNo,
                                    tankGrid.SealNo,
                                    tankGrid.GrossWeight,
                                    tankGrid.NettWeight,
                                    tankGrid.Measurement,
                                    tankGrid.UniversalSerialNr
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

        [HttpPost]
        public void ProcessTankData(FormCollection postData)
        {
            //Creating new product object based on postData
            AllocateEquipment tankallocate = new AllocateEquipment();
            tankallocate.ContainerNo = postData["ContainerNo"];
            tankallocate.SealNo = postData["SealNo"];
            tankallocate.GrossWeight = postData["GrossWeight"];
            tankallocate.NettWeight = postData["NettWeight"];
            tankallocate.Measurement = postData["Measurement"];
            tankallocate.ID = postData["ID"];
            tankallocate.UniversalSerialNr = Session["UniversalSerialNr"].ToString();

            if (postData["oper"] == "edit")
            {

                try
                {

                    if (ModelState.IsValid)
                    {

                        ErrorLog errorLog = DataContext.UpdateTank(tankallocate);
                        if (!errorLog.IsError)
                        {
                            TempData["message"] = "Tank Allocation is created successfully";
                        }
                        else
                        {
                            TempData["message"] = errorLog.ErrorMessage;
                        }
                    }
                    else
                    {
                        TempData["message"] = "Tank Allocation is not created";
                    }
                }
                catch (Exception ex)
                {
                    TempData["message"] = string.Format("Error occured: {0}", ex.Message);
                    throw;
                }
            }

            if (postData["oper"] == "del")
            {

                try
                {
                    if (ModelState.IsValid)
                    {
                        ErrorLog errorLog = DataContext.DeleteTank(tankallocate.ID, tankallocate.UniversalSerialNr);
                        if (!errorLog.IsError)
                        {
                            TempData["message"] = "Tank Allocation is created successfully";
                        }
                        else
                        {
                            TempData["message"] = errorLog.ErrorMessage;
                        }
                    }
                    else
                    {
                        TempData["message"] = "Tank Allocation is not created";
                    }
                }
                catch (Exception ex)
                {
                    TempData["message"] = string.Format("Error occured: {0}", ex.Message);
                    throw;
                }
            }

        }

        public void ManifestCargo(string jobref) {

            ShipmentBL shipment = DataContext.GetShipmentFromJobRef(jobref);
            List<AllocateEquipment> allocateEquipment = DataContext.GetTanksDetails(shipment.ShipmentDetailsModel.UniversalSerialNr);

            Document pdfDoc = new Document(PageSize.A4, 20, 20, 20, 20);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.SetPageSize(PageSize.A4.Rotate());
            
            pdfDoc.AddTitle("Manifest Cargo");
            pdfDoc.Open();
            Chunk chunk;
            Paragraph para, para1, line;
            PdfPTable table;
            PdfPCell cell;
            ;

            table = new PdfPTable(2);
            table.SetTotalWidth(new float[] { 72, 300 });
            table.LockedWidth = true;
            table.SpacingAfter = 5f;

            cell = new PdfPCell();
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            Image image1 = Image.GetInstance(Server.MapPath("~/Content/img/LogoOnlyStar.png"));
            image1.ScaleAbsolute(70, 70);
            image1.Alignment = Element.ALIGN_RIGHT;
            cell.AddElement(image1);
            table.AddCell(cell);

            chunk = new Chunk("LEGEND LOGISTICS (INDIA) PTE LTD", FontFactory.GetFont("Times", 16, Font.BOLD, new BaseColor(0, 0, 128)));
            para = new Paragraph("NO. 402 RUSTOMJEE ASPIREE, IMAX ROAD, EVERARD NAGAR OFF EASTERN EXPRESS HIGHWAY SION, MUMBAI - 400022, INDIA." +
                                    "\nindia@legendasia.com" +
                                    "\nwww.legendasia.com", FontFactory.GetFont("Calibri", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.AddElement(chunk);
            cell.AddElement(para);
            table.AddCell(cell);
            pdfDoc.Add(table);


            para = new Paragraph("CARGO MANIFEST", FontFactory.GetFont("Arial", 14, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            para.SpacingAfter = -5f;
            pdfDoc.Add(para);

            line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            table = new PdfPTable(6);
            table.SetTotalWidth(new float[] { 100, 10, 210, 100, 10, 130 });
            table.LockedWidth = true;
            table.HorizontalAlignment = 1;
            table.SpacingBefore = 0f;
            table.SpacingAfter = 3f;

            para = new Paragraph("Shipping Agent", FontFactory.GetFont("Arial", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph(":", FontFactory.GetFont("Arial", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para1 = new Paragraph(shipment.BLDetailsModel.ShipperNameBL, FontFactory.GetFont("Arial", 9));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para1);
            table.AddCell(cell);

            para = new Paragraph("Shipping Line", FontFactory.GetFont("Arial", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para1 = new Paragraph(":", FontFactory.GetFont("Arial", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph("APL", FontFactory.GetFont("Arial", 9));      //Ankita will notify you
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para1);
            table.AddCell(cell);


            para = new Paragraph("Vessel & Voyage No", FontFactory.GetFont("Arial", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph(":", FontFactory.GetFont("Arial", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para1 = new Paragraph(shipment.BLDetailsModel.VesselDetails, FontFactory.GetFont("Arial", 9));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para1);
            table.AddCell(cell);

            para = new Paragraph("Port of Loading", FontFactory.GetFont("Arial", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph(":", FontFactory.GetFont("Arial", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para1 = new Paragraph(shipment.BLDetailsModel.LoadPort, FontFactory.GetFont("Arial", 9));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para1);
            table.AddCell(cell);


            para = new Paragraph("ETD ", FontFactory.GetFont("Arial", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph(":", FontFactory.GetFont("Arial", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para1 = new Paragraph(shipment.VesselModel.ETD.ToString("dd-MM-yyyy"), FontFactory.GetFont("Arial", 9));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para1);
            table.AddCell(cell);

            para = new Paragraph("Port Where Report is", FontFactory.GetFont("Arial", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para1 = new Paragraph(":", FontFactory.GetFont("Arial", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph(shipment.BLDetailsModel.DischPort, FontFactory.GetFont("Arial", 9));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para1);
            table.AddCell(cell);


            para = new Paragraph("Last Port of Call", FontFactory.GetFont("Arial", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph(":", FontFactory.GetFont("Arial", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para1 = new Paragraph(shipment.BLDetailsModel.PlaceofDelivery, FontFactory.GetFont("Arial", 9));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para1);
            table.AddCell(cell);

            para = new Paragraph("Name of the Master", FontFactory.GetFont("Arial", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph(":", FontFactory.GetFont("Arial", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para1 = new Paragraph("", FontFactory.GetFont("Arial", 9));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para1);
            table.AddCell(cell);
            pdfDoc.Add(table);

            line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 2)));
            line.SpacingAfter = -10f;
            pdfDoc.Add(line);

            line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 2)));
            pdfDoc.Add(line);

            table = new PdfPTable(9);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1;
            table.SpacingAfter = 10f;
            table.DefaultCell.Border = Rectangle.NO_BORDER;
            table.DefaultCell.BorderWidthBottom = 1f;
            table.DefaultCell.PaddingTop = 5f;
            table.DefaultCell.PaddingBottom = 10f;

            para = new Paragraph("Line No", FontFactory.GetFont("Arial", 8, Font.BOLD));
            cell = new PdfPCell();
            cell.PaddingTop = 5f;
            cell.PaddingBottom = 5f;
            cell.AddElement(para);
            table.AddCell(para);

            para = new Paragraph("BL No & Date", FontFactory.GetFont("Arial", 8, Font.BOLD));
            cell = new PdfPCell();
            cell.AddElement(para);
            table.AddCell(para);

            para = new Paragraph("Shipper Name & Address", FontFactory.GetFont("Arial", 8, Font.BOLD));
            cell = new PdfPCell();
            cell.AddElement(para);
            table.AddCell(para);

            para = new Paragraph("Name of Consignee /" +
                                "Importer", FontFactory.GetFont("Arial", 8, Font.BOLD));
            cell = new PdfPCell();
            cell.AddElement(para);
            table.AddCell(para);

            para = new Paragraph("Description of Goods", FontFactory.GetFont("Arial", 8, Font.BOLD));
            cell = new PdfPCell();
            cell.AddElement(para);
            table.AddCell(para);

            para = new Paragraph("No. of Pkgs &\n Type of Pkgs", FontFactory.GetFont("Arial", 8, Font.BOLD));
            cell = new PdfPCell();
            cell.AddElement(para);
            table.AddCell(para);

            para = new Paragraph("Gross Wt M", FontFactory.GetFont("Arial", 8, Font.BOLD));
            cell = new PdfPCell();
            cell.AddElement(para);
            table.AddCell(para);

            para = new Paragraph("Marks & Nos.", FontFactory.GetFont("Arial", 8, Font.BOLD));
            cell = new PdfPCell();
            cell.AddElement(para);
            table.AddCell(para);

            para = new Paragraph("Container No. /" +
                    "\nSeal No. / " +
                    "\nSize : FCL or LCL", FontFactory.GetFont("Arial", 8, Font.BOLD));
            cell = new PdfPCell();
            cell.AddElement(para);
            table.AddCell(para);

            //Data
            para = new Paragraph(" ", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.PaddingTop = 5f;
            cell.PaddingBottom = 5f;
            cell.AddElement(para);
            table.AddCell(para);

            para = new Paragraph(jobref +
                    "\n"+shipment.BLDetailsModel.BLFinalisedDate.ToString("dd-MM-yyyy"), FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.PaddingTop = 5f;
            cell.PaddingBottom = 5f;
            cell.AddElement(para);
            table.AddCell(para);

            para = new Paragraph(shipment.BLDetailsModel.ShipperNameBL +
                "\n" + shipment.BLDetailsModel.ShipperAddressBL, FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.PaddingTop = 5f;
            cell.PaddingBottom = 5f;
            cell.AddElement(para);
            table.AddCell(para);

            para = new Paragraph(shipment.BLDetailsModel.ConsigneeNameBL +
                "\n"+shipment.BLDetailsModel.ConsigneeAddressBL, FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.PaddingTop = 5f;
            cell.PaddingBottom = 5f;
            cell.AddElement(para);
            table.AddCell(para);

            para = new Paragraph(shipment.BLDetailsModel.CargoDescription, FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.PaddingTop = 5f;
            cell.PaddingBottom = 5f;
            cell.AddElement(para);
            table.AddCell(para);

            para = new Paragraph(shipment.BLDetailsModel.NoOfPkgs.ToString() +" "+ shipment.BLDetailsModel.PkgType, FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.PaddingTop = 5f;
            cell.PaddingBottom = 5f;
            cell.AddElement(para);
            table.AddCell(para);

            para = new Paragraph(shipment.BLDetailsModel.TotalGweight + " " + shipment.BLDetailsModel.GrossWeightUnit, FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.PaddingTop = 5f;
            cell.PaddingBottom = 5f;
            cell.AddElement(para);
            table.AddCell(para);

            para = new Paragraph(shipment.BLDetailsModel.MarksandNo, FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.PaddingTop = 5f;
            cell.PaddingBottom = 5f;
            cell.AddElement(para);
            table.AddCell(para);

            para = new Paragraph(allocateEquipment[0].ContainerNo +
                    "\n" + allocateEquipment[0].SealNo +
                    "\nFCL / FCL", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.PaddingTop = 5f;
            cell.PaddingBottom = 5f;
            cell.AddElement(para);
            table.AddCell(para);
            pdfDoc.Add(table);

            para = new Paragraph("We hereby certify that item No. to Against IGM No. is for Account of our principals. We as Agent are responsible for the full outturn of cargo manifested under the above items and will be liable to the customs " +
                    "for any penalty or other dues in case of any shortlending / survey shortages.", FontFactory.GetFont("Arial", 8));
            pdfDoc.Add(para);

            para1 = new Paragraph("We hereby hold SGSIN agents of the vessel fully indemnified from any shortlendings/survey shortages under the above items.", FontFactory.GetFont("Arial", 8));
            para1.SpacingBefore = 10f;
            para1.SpacingAfter = 20f;
            pdfDoc.Add(para1);

            para = new Paragraph("For Legend Shipping services pvt limited", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.SpacingAfter = 30f;
            pdfDoc.Add(para);

            para1 = new Paragraph("As Agent", FontFactory.GetFont("Arial", 8));
            pdfDoc.Add(para1);

            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "inline;filename=Manifest Cargo.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
        }

        public ActionResult MailSend(string jobref)
        {
            if (ModelState.IsValid)
            {
                string username = HttpContext.User.Identity.Name;
                MailMessage mail = new MailMessage();
                mail.To.Add("gauruv.grover@legendlogisticsltd.com");
                mail.From = new MailAddress("lcms@legendlogisticsltd.com");
                mail.Subject = "Re-Print BL Request for JobRef " + jobref;
                string Body = string.Format("{0} has sent Re-Print Request for JobRef {1}. Kindly allow user to re-print same.", username, jobref);
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.office365.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("lcms@legendlogisticsltd.com", "CtrInd@legend"); // Enter senders User name and password  
                smtp.EnableSsl = true;
                smtp.Send(mail);

                return Json(new { success = true, msg = "Mail Request sent successfully" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, msg = "Mail Request not sent. Please try again." }, JsonRequestBehavior.AllowGet);
            }
            //return RedirectToAction("ShipmentDetails", "ShipmentDetails", new { JobRef = jobref });
        }
    }
}