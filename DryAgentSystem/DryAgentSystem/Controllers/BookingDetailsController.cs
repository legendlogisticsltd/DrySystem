using DryAgentSystem.Data;
using DryAgentSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace DryAgentSystem.Controllers
{
    public class BookingDetailsController : Controller
    {
        //TempData["message"] = "";
        // GET: RateRequestDetails

        [Authorize]
        //[ValidateAntiForgeryToken]
        //[Route("RateRequest/RateRequestDetails")]
        public ActionResult BookingDetails(string BookingID)
        {
            
            Booking booking = new Booking();
            booking = DataContext.GetBookingfromID(BookingID);
            
            TempData["UniversalSerialNr"] = booking.UniversalSerialNr;
            Session["UniversalSerialNr"] = booking.UniversalSerialNr;
            Session["BookingID"] = BookingID;
            booking.VesselModel.BookingStatus = booking.BookingStatus;


            if (TempData["bookingobj"] != null)
            {
                booking = (Booking)TempData["bookingobj"];
            }
            ViewBag.CollectionList = DataContext.GetCollectionPorts(booking.LoadPort);
            ViewBag.PortList = DataContext.GetCountryPorts();
            ViewBag.shipmentlist = DataContext.ShipmentTerm();
            ViewBag.equipmentlist = DataContext.EquipmentType();
            if (booking.CutoffDate.ToString() == "1/1/0001 12:00:00 AM")
            {
                booking.CutoffDate = DateTime.Today;
            }
            //ViewBag.PaymentList = DataContext.GetPaymentTerm();

            return View(booking);
        }

        [HttpPost]
        public ActionResult BookingDetailsTab(Booking booking, string submit)
        {
            ViewBag.PortList = DataContext.GetCountryPorts();
            ViewBag.shipmentlist = DataContext.ShipmentTerm();
            ViewBag.equipmentlist = DataContext.EquipmentType();
            //ViewBag.PaymentList = DataContext.GetPaymentTerm();

            if (submit == "Save")
            {
                if (ModelState.IsValid)
                {
                    ErrorLog errorLog = DataContext.SaveBooking(booking);
                    if (!errorLog.IsError)
                    {
                        booking.BookingID = errorLog.ErrorMessage;
                        TempData["Message"] = "Booking Request successfully created Booking ID " + booking.BookingID;
                    }
                    else
                    {
                        TempData["Message"] = errorLog.ErrorMessage;
                    }
                }

                ModelState.Clear();
                return RedirectToAction("BookingDetails", "BookingDetails", new { BookingID = booking.BookingID });
            }
            if (submit == "Update")
            {
                if (ModelState.IsValid)
                {
                    ErrorLog errorLog = DataContext.UpdateBooking(booking);
                    if (!errorLog.IsError)
                    {
                        TempData["message"] = "Booking Request successfully updated";
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
                return RedirectToAction("BookingDetails", "BookingDetails", new { BookingID = booking.BookingID });
            }
            if (submit == "Create Shipment")
            {
                ShipmentBL shipmentnew = new ShipmentBL();
                if(ModelState.IsValid)
                {
                    shipmentnew.ShipmentDetailsModel.ShipmentTerm = booking.ShipmentTerm;
                    shipmentnew.BLDetailsModel.LoadPort = booking.LoadPort;
                    shipmentnew.BLDetailsModel.DischPort = booking.DischargePort;
                    shipmentnew.BLDetailsModel.PlaceofReceipt = booking.PlaceOfReceipt;
                    shipmentnew.BLDetailsModel.PlaceofDelivery = booking.PlaceOfDelivery;
                    shipmentnew.BLDetailsModel.CargoDescription = booking.CargoType;
                    shipmentnew.ShipmentDetailsModel.UniversalSerialNr = booking.UniversalSerialNr;

                    TempData["shipmentobj"] = shipmentnew;
                    ModelState.Clear();
                    return RedirectToAction("ShipmentDetails", "ShipmentDetails");
                }
                else
                {
                    TempData["Message"] = "Please check the fields, some of the fields are not in correct format";

                    ModelState.Clear();
                    return View();
                }
            }
            else
            {
                return View();
            }
            
        }

        public ActionResult BookingDetailsTab()
        {
            return PartialView();
        }

        public ActionResult VesselDetailsTab()
        {
            ViewBag.PortList = DataContext.GetCountryPorts();
            return PartialView();
        }

        [HttpPost]
        public ActionResult VesselDetailsTab(Vessel vessel, string submit)
        {
            ViewBag.PortList = DataContext.GetCountryPorts();
            string bookingid = Session["BookingID"].ToString();
            if (submit == "Create")
            {
                vessel.UniversalSerialNr = Session["UniversalSerialNr"].ToString();
                CreateVessel(vessel);
                Session["UniversalSerialNr"] = null;
            }
            if (submit == "Update")
            {
                UpdateVessel(vessel);
            }
            if (submit == "Delete")
            {
                DeleteVessel(vessel.UniversalSerialNr, vessel.ID);
            }
            //return View(vessel);
            return RedirectToAction("BookingDetails", "BookingDetails", new { BookingID = bookingid });
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

        
        public string CreateVessel(Vessel vessel)
        {
            
            try
            {
                if (ModelState.IsValid )
                {
                    ErrorLog errorLog = DataContext.CreateVessel(vessel);
                    if (!errorLog.IsError)
                    {
                        TempData["message"] = "Vessel Schedule is created successfully";
                    }
                    else
                    {
                        TempData["message"] = errorLog.ErrorMessage;
                    }
                }
                else
                {
                    TempData["message"] = "Vessel Schedule is not created";
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = string.Format("Error occured: {0}", ex.Message);
                throw;
            }
            return TempData["message"].ToString();
        }

        public string UpdateVessel(Vessel vessel)
        {
            
            try
            {
                if (ModelState.IsValid)
                {
                    ErrorLog errorLog = DataContext.UpdateVessel(vessel);
                    if (!errorLog.IsError)
                    {
                        TempData["message"] = "Vessel Schedule successfully updated";
                    }
                    else
                    {
                        TempData["message"] = errorLog.ErrorMessage;
                    }
                    TempData["message"] = "Saved Vessel Data Successfully";
                }
                else
                {
                    TempData["message"] = "Vessel Schedule not updated. Please check and try again";
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = string.Format("Error occured: {0}", ex.Message);
                throw;
            }
            return TempData["message"].ToString();
        }

        public string DeleteVessel(string universalSerialNr, string vesselID)
        {
            
            try
            {
                if (ModelState.IsValid)
                {
                    ErrorLog errorLog = DataContext.DeleteVessel(universalSerialNr, vesselID);
                    if (!errorLog.IsError)
                    {
                        TempData["message"] = "Vessel Schedule successfully deleted";
                    }
                    else
                    {
                        TempData["message"] = errorLog.ErrorMessage;
                    }
                }
                else
                {
                    TempData["message"] = "Vessel Schedule deletion not successful";
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = string.Format("Error occured: {0}", ex.Message);
                throw;
            }
            return TempData["message"].ToString();
        }

        public void PrintBC(string bookingID)
        {

            Booking booking = DataContext.GetBookingfromID(bookingID);

            QuoteRef quoteRef = DataContext.GetQuoteRequestFromQuoteID(booking.QuoteRefID);

            List<Vessel> vessels = DataContext.GetVesselsDetails(booking.UniversalSerialNr).ToList();

            Document pdfDoc = new Document(PageSize.A4, 40, 40, 20, 20);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.AddTitle("Booking Confirmation");
            pdfDoc.Open(); 
            Chunk chunk;
            Paragraph para, para1, para2, para3;
            PdfPTable table;
            PdfPCell cell;

            table = new PdfPTable(2);
            table.SetTotalWidth(new float[] { 72, 350 });
            table.LockedWidth = true;
            table.SpacingAfter = -20f;

            cell = new PdfPCell();
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            Image image1 = Image.GetInstance(Server.MapPath("~/Content/Img/LogoOnlyStar.png"));
            image1.ScaleAbsolute(70, 70);
            image1.Alignment = Element.ALIGN_RIGHT;
            cell.AddElement(image1);
            table.AddCell(cell);

            chunk = new Chunk("LEGEND CONTAINER LINE PTE LTD", FontFactory.GetFont("Times", 19, Font.BOLD, BaseColor.BLUE));
            para = new Paragraph("531 Upper Cross Street, #04-59 Hong Lim Complex, Singapore 050531." +
                                    "\nTel: +65 6221 4844 Fax: +65 6225 4644" +
                                    "\nCO.Reg No and GST Reg No. 201209737N", FontFactory.GetFont("Calibri", 9));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.AddElement(chunk);
            cell.AddElement(para);
            table.AddCell(cell);
            pdfDoc.Add(table);

            para = new Paragraph("\nBOOKING CONFIRMATION", FontFactory.GetFont("Courier", 14, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            pdfDoc.Add(para);

            //Booking Confirmation Table
            table = new PdfPTable(4);
            table.HorizontalAlignment = 1;
            table.SpacingBefore = 5f;
            table.SetTotalWidth(new float[] { 40, 285, 85, 110 });
            table.LockedWidth = true;
            table.SpacingAfter = 1f;

            para = new Paragraph(" TO \n ATTN \n TEL \n FAX", FontFactory.GetFont("Courier", 10, Font.BOLD));
            para1 = new Paragraph(" : "+booking.AddressTo+" \n : "+booking.AddressAttn+" \n : "+booking.AddressTel+" \n : "+booking.AddressFax, FontFactory.GetFont("Courier", 10));
            para2 = new Paragraph("\n\n\n BOOKING DATE", FontFactory.GetFont("Courier", 10, Font.BOLD));
            para3 = new Paragraph("\n\n\n : "+booking.BookingDate.ToString("dd-MM-yyyy")+" \n", FontFactory.GetFont("Courier", 10));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthTop = 1;
            cell.BorderWidthLeft = 1;
            cell.BorderWidthBottom = 1;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.AddElement(para);
            table.AddCell(cell);

            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthTop = 1;
            cell.BorderWidthBottom = 1;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.AddElement(para1);
            table.AddCell(cell);

            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthTop = 1;
            cell.BorderWidthBottom = 1;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.AddElement(para2);
            table.AddCell(cell);

            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthTop = 1;
            cell.BorderWidthRight = 1;
            cell.BorderWidthBottom = 1;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.PaddingBottom = 10f;
            cell.AddElement(para3);
            table.AddCell(cell);
            pdfDoc.Add(table);

            para = new Paragraph("WE ARE PLEASED TO CONFIRM YOUR BOOKING AS FOLLOWS :", FontFactory.GetFont("Courier", 10));
            pdfDoc.Add(para);

            table = new PdfPTable(2);
            table.HorizontalAlignment = 0;
            table.DefaultCell.Padding = 4f;
            table.DefaultCell.Border = Rectangle.NO_BORDER;
            table.SetTotalWidth(new float[] { 150, 370 });
            table.LockedWidth = true;

            para = new Paragraph("BOOKING NO. \n" +
                                "VESSEL \n" +
                                "VOYAGE \n" +
                                "POL \n" +
                                "POD \n" +
                                "FINAL DEST \n" +
                                "ETD "+booking.LoadPort+"\n" +
                                "ETA "+booking.DischargePort+"\n" +
                                "CARRIER \n" +
                                "CARRIER BKG REF NO. \n" +
                                "GROSS WEIGHT \n" +
                                "MEASUREMENT \n" +
                                "COMMODITY \n" +
                                "NO. OF CONTAINER \n" +
                                "COLLECTION YARD \n" +
                                "RELEASE ORDER NO \n" +
                                "REMARKS", FontFactory.GetFont("Courier", 10, Font.BOLD));

            para1 = new Paragraph(":  "+booking.BookingNo+"\n" +
                                ":  "+vessels[0].VesselName+" \n" +
                                ":  "+vessels[0].VoyNo+" \n" +
                                ":  "+booking.LoadPort+" \n" +
                                ":  "+booking.DischargePort+" \n" +
                                ":  "+quoteRef.PlaceOfDelivery+" \n" +
                                ":  "+vessels[0].ETD.ToString("dd-MM-yyyy") +" \n" +
                                ":  " +vessels[vessels.Count - 1].ETA.ToString("dd-MM-yyyy") + " \n" +
                                ":  "+vessels[0].Carrier+"\n" +
                                ":  "+vessels[0].CarrierBookingRefNo+" \n" +
                                ":  "+booking.Grossweight+" \n" +
                                ":  "+booking.GrossweightMeasurement+" \n" +
                                ":  "+booking.Commodity+" \n" +
                                ":  "+quoteRef.Quantity+" \t X \t "+quoteRef.EquipmentType+"\n" +
                                ":  "+booking.CollectionYard+"\n" +
                                ":  "+booking.ContainerReleaseOrderNo+" \n" +
                                ":  "+booking.Remark, FontFactory.GetFont("Courier", 10));

            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Border = 0;
            cell.Padding = 5f;
            cell.AddElement(para);
            table.AddCell(cell);

            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Border = 0;
            cell.Padding = 5f;
            cell.AddElement(para1);
            table.AddCell(cell);
            pdfDoc.Add(table);

            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            table = new PdfPTable(6);
            table.HorizontalAlignment = 1;
            table.DefaultCell.Border = Rectangle.NO_BORDER;
            table.SetTotalWidth(new float[] { 88, 85, 85, 88, 85, 85 });
            table.LockedWidth = true;

            para = new Paragraph("Vessel Name",FontFactory.GetFont("Courier", 10,Font.BOLD));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Border = 0;
            cell.PaddingBottom = 10f;
            cell.BorderWidthBottom = 1f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("Voy No", FontFactory.GetFont("Courier", 10, Font.BOLD));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Border = 0;
            cell.PaddingBottom = 10f;
            cell.PaddingLeft = 5f;
            cell.BorderWidthBottom = 1f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("Load Port", FontFactory.GetFont("Courier", 10, Font.BOLD));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Border = 0;
            cell.PaddingBottom = 10f;
            cell.BorderWidthBottom = 1f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("Discharge Port", FontFactory.GetFont("Courier", 10, Font.BOLD));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Border = 0;
            cell.PaddingBottom = 10f;
            cell.BorderWidthBottom = 1f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("ETD POD", FontFactory.GetFont("Courier", 10, Font.BOLD));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.PaddingLeft = 20f;
            cell.Border = 0;
            cell.PaddingBottom = 10f;
            cell.BorderWidthBottom = 1f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("ETA POL", FontFactory.GetFont("Courier", 10, Font.BOLD));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.PaddingLeft = 20f;
            cell.Border = 0;
            cell.PaddingBottom = 10f;
            cell.BorderWidthBottom = 1f;
            cell.AddElement(para);
            table.AddCell(cell);

            for(int i = 0 ; i < vessels.Count;i++) {
                para = new Paragraph(vessels[i].VesselName, FontFactory.GetFont("Courier", 10));
                cell = new PdfPCell();
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph(vessels[i].VoyNo, FontFactory.GetFont("Courier", 10));
                cell = new PdfPCell();
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                cell.PaddingLeft = 5f;                
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph(vessels[i].LoadPort, FontFactory.GetFont("Courier", 10));
                cell = new PdfPCell();
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph(vessels[i].DischPort, FontFactory.GetFont("Courier", 10));
                cell = new PdfPCell();
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph(vessels[i].ETD.ToString("dd-MM-yyyy"), FontFactory.GetFont("Courier", 10));
                cell = new PdfPCell();
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingLeft = 5f;
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph(vessels[i].ETA.ToString("dd-MM-yyyy"), FontFactory.GetFont("Courier",10));
                cell = new PdfPCell();
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.PaddingLeft = 5f;
                cell.Border = 0;                
                cell.AddElement(para);
                table.AddCell(cell);
            }

            pdfDoc.Add(table);


            para = new Paragraph("\n\n a) IMPORTANT NOTE: SAFETY OF LIFE AT SEA (SOLAS) ANNOUNCEMENT – EFFECTIVE 1ST JULY 2016" +
                                    "All shippers shall follow SOLAS(Safety Of Life At Sea) Regulations, effective 1st July 2016 and will be responsible to provide the" +
                                    "Carrier with the Verified Gross Mass(“VGM”) for all export sea shipments tendered before the containers are loaded on board the vessel." +
                                    "Container will not be loaded on the vessel if there is no submission of the certified VGM before loading. Full liability or any" +
                                   " associated incidental charges charged by the Carrier / Port Of Authority for rejected containers will fully be under shipper’s account." +
                                     "LCL shipment: We will bill the VGM fee to your esteem organization should there be any charges imposed by the consolidators." +
                                     "\n b) Fumigation with ISPM regulation is required for all wooden packaging materials with compliance to the respective countries" +
                                     "regulation." +
                                     "\n c) Kindly notify us immediately should there be any discrepancies on the above information.We will consider your booking details are" +
                                    "correct if there is no feedback received from your esteem organization.", FontFactory.GetFont("Courier", 7));
            pdfDoc.Add(para);
            line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            DateTime dateTime = DateTime.Now;
            chunk = new Chunk("Prepared By", FontFactory.GetFont("Courier", 10));
            para = new Paragraph(chunk + "    :  "+ Session["Name"] + "                     "+dateTime.ToString("dd-MM-yyyy h:mm tt"), FontFactory.GetFont("Courier", 10, Font.BOLD));
            pdfDoc.Add(para);

            para = new Paragraph("ALL BUSINESS TRANSACTED WITH LEGEND SHIPPING PTE LTD IS CONDUCTED TO \n SLA SINGAPORE LOGISTICS ASSOCIATION CONDITIONS.", FontFactory.GetFont("Courier", 10));
            para.Alignment = Element.ALIGN_CENTER;
            pdfDoc.Add(para);

            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "inline;filename=Booking Confirmation.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
        }

        public void PrintCRO(string bookingID) {

            Booking booking = DataContext.GetBookingfromID(bookingID);
            QuoteRef quoteRef = DataContext.GetQuoteRequestFromQuoteID(booking.QuoteRefID);
            List<Vessel> vessels = DataContext.GetVesselsDetails(booking.UniversalSerialNr).ToList();

            Document pdfDoc = new Document(PageSize.A4, 40, 40, 20, 20);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.AddTitle("CONTAINER RELEASE ORDER");
            pdfDoc.Open();
            Chunk chunk;
            Paragraph para;
            PdfPTable table;
            PdfPCell cell;

            table = new PdfPTable(2);
            table.SetTotalWidth(new float[] { 72, 350 });
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

            chunk = new Chunk("LEGEND CONTAINER LINE PTE LTD", FontFactory.GetFont("Times", 19, Font.BOLD, BaseColor.BLUE));
            para = new Paragraph("531 Upper Cross Street, #04-59 Hong Lim Complex, Singapore 050531." +
                                    "\nTel: +65 6221 4844 Fax: +65 6225 4644" +
                                    "\nCO.Reg No and GST Reg No. 201209737N", FontFactory.GetFont("Calibri", 9));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.AddElement(chunk);
            cell.AddElement(para);
            table.AddCell(cell);
            pdfDoc.Add(table);


            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 3)));
            pdfDoc.Add(line);

            chunk = new Chunk("CONTAINER RELEASE ORDER", FontFactory.GetFont("Courier", 15, Font.BOLD));
            para = new Paragraph(chunk);
            para.Alignment = Element.ALIGN_CENTER;
            pdfDoc.Add(para);


            table = new PdfPTable(4);
            table.SetTotalWidth(new float[] { 200, 65, 100, 150 });
            table.LockedWidth = true;
            table.SpacingBefore = 5f;
            table.SpacingAfter = 5f;

            chunk = new Chunk("To,", FontFactory.GetFont("Courier", 9, Font.BOLD));
            para = new Paragraph(booking.CollectionYard+"\n"+booking.Address+"\nPhone No: "+booking.PhoneNo+"\nEmail id: "+booking.Email, FontFactory.GetFont("Courier", 9));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.BorderWidthRight = 0;
            cell.AddElement(chunk);
            cell.AddElement(para);
            table.AddCell(cell);

            cell = new PdfPCell();
            cell.BorderWidthLeft = 0;
            table.AddCell(cell);

            Paragraph para1 = new Paragraph("CRO No \n" +
                "CRO Date \n" +
                "Booking Ref No \n" +
                "Pick Up Date \n" +
                "POR \n" +
                "POL \n" +
                "POD \n" +
                "PFD ", FontFactory.GetFont("Courier", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.BorderWidthRight = 0;
            cell.AddElement(para1);
            table.AddCell(cell);

            DateTime dateTime = DateTime.Now;
            para = new Paragraph(":  "+booking.ContainerReleaseOrderNo+" \n" +
                ":  "+ dateTime.ToString("dd-MM-yyyy") + " \n" +
                ":  "+booking.BookingNo+" \n" +
                ":  "+booking.CROPickUpDate.ToString("dd-MM-yyyy")+" \n" +
                ":  "+quoteRef.PlaceOfReceipt+" \n" +
                ":  "+booking.LoadPort+" \n" +
                ":  "+booking.DischargePort+" \n" +
                ":  "+quoteRef.PlaceOfDelivery+" ", FontFactory.GetFont("Courier", 9));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.BorderWidthLeft = 0;
            cell.AddElement(para);
            table.AddCell(cell);
            pdfDoc.Add(table);

            para = new Paragraph("Vessel Details", FontFactory.GetFont("Courier", 10, Font.BOLD | Font.UNDERLINE));
            pdfDoc.Add(para);


            //2nd Table Vessel
            table = new PdfPTable(4);
            table.SetTotalWidth(new float[] { 100,165,70,180}); //90,100
            table.LockedWidth = true;
            //table.WidthPercentage = 100;
            table.SpacingBefore = 5f;

            para = new Paragraph("Vessel Name \n" +
                "ETA ", FontFactory.GetFont("Courier", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para1 = new Paragraph(": "+vessels[0].VesselName+" \n" +
                ": "+vessels[vessels.Count - 1].ETA.ToString("dd-MM-yyyy"), FontFactory.GetFont("Courier", 9));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.Border = 0;
            cell.AddElement(para1);
            table.AddCell(cell);

            para = new Paragraph("Voyage \n" +
                "ETD ", FontFactory.GetFont("Courier", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para1 = new Paragraph(": "+vessels[0].VoyNo+" \n" +
                ": " + vessels[0].ETD.ToString("dd-MM-yyyy"), FontFactory.GetFont("Courier", 9));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.Border = 0;
            cell.AddElement(para1);
            table.AddCell(cell);

            /*
            para = new Paragraph("Via No \n" +
                "Port Cut Off \n" +
                "Rotation No/Date", FontFactory.GetFont("Courier", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para1 = new Paragraph(": Data\n" +
                ": "+booking.CutoffDate.ToString("dd-MM-yyyy")+" \n" +
                ": Data \n", FontFactory.GetFont("Courier", 9));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.Border = 0;
            cell.AddElement(para1);
            table.AddCell(cell);
            */
            pdfDoc.Add(table);


            //3rd size and type table
            table = new PdfPTable(5);
            table.SetTotalWidth(new float[] { 85, 70, 80, 140, 145 });
            table.LockedWidth = true;
            table.SpacingBefore = 20f;
            table.PaddingTop = 10f;

            para1 = new Paragraph("Size & Type", FontFactory.GetFont("Courier", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 3f;
            cell.Border = 0;
            cell.BorderWidthTop = 1;
            cell.BorderWidthBottom = 1;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph("Quantity \n", FontFactory.GetFont("Courier", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 3f;
            cell.Border = 0;
            cell.BorderWidthTop = 1;
            cell.BorderWidthBottom = 1;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph("Commodity", FontFactory.GetFont("Courier", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 3f;
            cell.Border = 0;
            cell.BorderWidthTop = 1;
            cell.BorderWidthBottom = 1;
            cell.AddElement(para1);
            table.AddCell(cell);

            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 3f;
            cell.Border = 0;
            cell.BorderWidthTop = 1;
            cell.BorderWidthBottom = 1;
            table.AddCell(cell);

            para1 = new Paragraph("Remark", FontFactory.GetFont("Courier", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 3f;
            cell.PaddingLeft = 45f;
            cell.Border = 0;
            cell.BorderWidthTop = 1;
            cell.BorderWidthBottom = 1;
            cell.AddElement(para1);
            table.AddCell(cell);

            // 3rd table Data Stars Here
            para1 = new Paragraph(quoteRef.EquipmentType, FontFactory.GetFont("Courier", 9));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 3f;
            cell.Border = 0;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph(quoteRef.Quantity.ToString(), FontFactory.GetFont("Courier", 9));
            cell = new PdfPCell();
            cell.Padding = 3f;
            cell.PaddingLeft = 15f;
            cell.Border = 0;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph(booking.Commodity, FontFactory.GetFont("Courier", 9));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 3f;
            cell.Border = 0;
            cell.AddElement(para1);
            table.AddCell(cell);

            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 3f;
            cell.Border = 0;
            table.AddCell(cell);

            para1 = new Paragraph(booking.CRORemarks, FontFactory.GetFont("Courier", 9));
            cell = new PdfPCell();
            cell.Padding = 3f;
            cell.PaddingLeft = 15f;
            cell.Border = 0;
            cell.AddElement(para1);
            table.AddCell(cell);
            pdfDoc.Add(table);

            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "inline;filename=Container Release Order.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
        }


        [HttpGet]
        public ActionResult Confirm(string bookingID)
        {
            //if (ModelState.IsValid)


            var vessels = DataContext.GetVesselsDetails(DataContext.GetBookingfromID(bookingID).UniversalSerialNr);
            if(vessels.Count<1)
            {
                TempData["message"] = "Please create Vessel Schedule before Confirming Booking";
            }
            else
            {
                ErrorLog errorLog = DataContext.ConfirmBooking(bookingID);

                if (!errorLog.IsError)
                {
                    TempData["message"] = "Booking Request successfully Confirmed";
                }
                else
                {
                    TempData["message"] = errorLog.ErrorMessage;
                }
            }
            

            return RedirectToAction("BookingDetails", "BookingDetails", new { BookingID = bookingID });
        }
        
        [HttpGet]
        public ActionResult Issue(string bookingID)
        {
            //if (ModelState.IsValid)
            

            ErrorLog errorLog = DataContext.IssueBooking(bookingID);
            
            if (!errorLog.IsError)
            {
                TempData["message"] = "Booking Request successfully Issued";
            }
            else
            {
                TempData["message"] = errorLog.ErrorMessage;
            }

            return RedirectToAction("BookingDetails", "BookingDetails", new { BookingID = bookingID });
        }

        [HttpGet]
        public ActionResult Edit(string bookingID)
        {
            //if (ModelState.IsValid)

            ErrorLog errorLog = DataContext.EditBooking(bookingID);

            if (!errorLog.IsError)
            {
                TempData["message"] = "Booking Request successfully opened for editing";
            }
            else
            {
                TempData["message"] = errorLog.ErrorMessage;
            }

            return RedirectToAction("BookingDetails", "BookingDetails", new { BookingID = bookingID });
        }
    }
}
