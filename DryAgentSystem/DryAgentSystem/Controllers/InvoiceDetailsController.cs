using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using DryAgentSystem.Data;
using DryAgentSystem.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace DryAgentSystem.Controllers
{
    public class InvoiceDetailsController : Controller
    {

        [HttpGet]
        [Authorize]
        public ActionResult InvoiceDetails(Invoice invoicedetails)
        {


            if (TempData["invoiceobj"] != null)
            {
                invoicedetails = (Invoice)TempData["invoiceobj"];
            }
            invoicedetails.InvoiceNo = DataContext.GetInvoiceFromUSN(invoicedetails.UniversalSerialNr).InvoiceNo;
            if(String.IsNullOrEmpty(invoicedetails.InvoiceNo))
            {
                InvoiceDetails(invoicedetails, "Save");
                //ExportInvoicePDF(invoicedetails.JobRefNo);
                //call print method here
            }
            else
            {
                //ExportInvoicePDF(invoicedetails.JobRefNo);
            }
            //TempData["UniversalSerialNr"] = invoicedetails.UniversalSerialNr;
            //TempData["InvoiceNo"] = invoicedetails.InvoiceNo;

            return RedirectToAction("ShipmentDetails", "ShipmentDetails", new { JobRef = invoicedetails.JobRefNo });
        }

        [HttpPost]
        public void InvoiceDetails(Invoice invoice, string submit)
        {

            if (submit == "Save")
            {
                if (ModelState.IsValid)
                {
                    ErrorLog errorLog = DataContext.SaveExportInvoice(invoice);
                    if (!errorLog.IsError)
                    {
                        invoice.InvoiceNo = errorLog.ErrorMessage;
                        TempData["Message"] = "Invoice successfully created Invoice No. " + invoice.InvoiceNo + ". Please click Print Invoice to view PDF";
                    }
                    else
                    {
                        TempData["Message"] = errorLog.ErrorMessage;
                    }
                }

                ModelState.Clear();
                //return RedirectToAction("InvoiceDetails", "InvoiceDetails", new { InvoiceNo = invoice.InvoiceNo });
            }
            else
            {
                //return View(invoice);
            }

        }

        //public JsonResult GetInvoiceLineItems(string sidx, string sort, int page, int rows)
        //{
        //    string universalSerialNr = string.Empty;
        //    if (TempData["UniversalSerialNr"] != null)
        //    {
        //        universalSerialNr = TempData["UniversalSerialNr"].ToString();
        //        List<InvoiceDetails> invoicelineitems = new List<InvoiceDetails>();
        //        if(TempData["InvoiceNo"] != null)
        //        {
        //            invoicelineitems = DataContext.GetInvoiceChargesList(universalSerialNr);
        //        }
        //        else
        //        {
        //            invoicelineitems = DataContext.GetSalesChargesList(universalSerialNr);
        //        }
                
        //        int totalRecords = invoicelineitems.Count();
        //        var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

        //        var jsonData = new
        //        {
        //            total = totalPages,
        //            page,
        //            records = totalRecords,
        //            rows = (from InvoiceDetailsGrid in invoicelineitems
        //                    select new
        //                    {
        //                        InvoiceDetailsGrid.InvoiceNo,
        //                        cell = new string[]
        //                        {
        //                        InvoiceDetailsGrid.ID,
        //                        InvoiceDetailsGrid.Description,
        //                        InvoiceDetailsGrid.Quantity.ToString(),
        //                        InvoiceDetailsGrid.Currency,
        //                        InvoiceDetailsGrid.UnitRate.ToString(),
        //                        InvoiceDetailsGrid.ExRate.ToString(),
        //                        InvoiceDetailsGrid.AmountUSD
        //                        }
        //                    }).ToArray()
        //        };
        //        return Json(jsonData, JsonRequestBehavior.AllowGet);
        //    }
            
        //    else
        //    {
        //        return Json(new object[] { new object() }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public void ExportInvoicePDF(string jobref)
        {
            ShipmentBL shipment = DataContext.GetShipmentFromJobRef(jobref);
            Invoice invoice = DataContext.GetInvoiceFromUSN(shipment.ShipmentDetailsModel.UniversalSerialNr);
            List<InvoiceDetails> invoiceDetails = DataContext.GetInvoiceChargesList(shipment.ShipmentDetailsModel.UniversalSerialNr);

            Document pdfDoc = new Document(PageSize.A4, 20, 20, 20, 20);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.AddTitle("Export Invoice");
            pdfDoc.Open();
            Chunk chunk;
            Paragraph para, para1;
            PdfPTable table;
            PdfPCell cell;

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
            para = new Paragraph("NO. 402 RUSTOMJEE ASPIREE, IMAX ROAD, EVERARD NAGAR OFF EASTERN EXPRESS HIGHWAY SION, MUMBAI - 400022, INDIA" +
                                    "\nindia@legendasia.com" +
                                    "\nwww.legendasia.com", FontFactory.GetFont("Calibri", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.AddElement(chunk);
            cell.AddElement(para);
            table.AddCell(cell);
            pdfDoc.Add(table);

            table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1;
            table.DefaultCell.PaddingLeft = 5f;

            para = new Paragraph("EXPORT INVOICE", FontFactory.GetFont("Arial", 13, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.Colspan = 6;
            cell.BackgroundColor = new BaseColor(211, 211, 211);
            cell.PaddingBottom = 10f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph(invoice.BillingParty +
                                "\n"+invoice.BillingPartyAddress+
                                "\n ", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Colspan = 3;
            cell.Rowspan = 4;
            cell.BorderWidthBottom = 0f;
            cell.AddElement(para);
            table.AddCell(cell);

            para1 = new Paragraph("INVOICE NO.", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.BorderWidthBottom = 0f;
            cell.BorderWidthRight = 0f;
            cell.BorderWidthLeft = 0f;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph(": "+invoice.InvoiceNo, FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.BorderWidthBottom = 0f;
            cell.BorderWidthLeft = 0f;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph("DATE", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph(": "+invoice.InvoiceDate.ToString("dd-MM-yyyy"), FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthRight = 0.5f;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph("JOB REF", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph(": "+jobref, FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthRight = 0.5f;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph("CREDIT TERMS", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph(": "+invoice.CreditTerms, FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthRight = 0.5f;
            cell.AddElement(para1);
            table.AddCell(cell);

            para = new Paragraph("ATTENTION", FontFactory.GetFont("Arial", 8, Font.BOLD));
            cell = new PdfPCell();
            cell.BorderWidthTop = 0f;
            cell.BorderWidthRight = 0f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph(": FINANCE TEAM", FontFactory.GetFont("Arial", 8, Font.BOLD));
            cell = new PdfPCell();
            cell.Colspan = 2;
            cell.BorderWidthTop = 0f;
            cell.BorderWidthLeft = 0f;
            cell.AddElement(para);
            table.AddCell(cell);

            para1 = new Paragraph("DUE DATE ", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthBottom = 0.5f;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph(": "+invoice.DueDate.ToString("dd-MM-yyyy"), FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthRight = 0.5f;
            cell.BorderWidthBottom = 0.5f;
            cell.AddElement(para1);
            table.AddCell(cell);

            para = new Paragraph("VESSEL/VOY", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.BorderWidthTop = 0f;
            cell.BorderWidthRight = 0f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph(": "+shipment.VesselModel.VesselName+" - "+shipment.VesselModel.VoyNo, FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Colspan = 2;
            cell.BorderWidthTop = 0f;
            cell.BorderWidthLeft = 0f;
            cell.AddElement(para);
            table.AddCell(cell);

            para1 = new Paragraph("MASTER BL", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthBottom = 0.5f;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph(": "+shipment.ShipmentDetailsModel.MBLMAWB, FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthRight = 0.5f;
            cell.BorderWidthBottom = 0.5f;
            cell.AddElement(para1);
            table.AddCell(cell);

            para = new Paragraph("POL", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.BorderWidthTop = 0f;
            cell.BorderWidthRight = 0f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph(": "+invoice.LoadPort, FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.BorderWidthTop = 0f;
            cell.BorderWidthLeft = 0f;
            cell.BorderWidthRight = 0f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("ETD : "+invoice.ETD.ToString("dd-MM-yyyy"), FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.BorderWidthTop = 0f;
            cell.BorderWidthLeft = 0f;
            cell.AddElement(para);
            table.AddCell(cell);

            para1 = new Paragraph("HOUSE BL", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthBottom = 0.5f;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph(": "+shipment.ShipmentDetailsModel.HBLHAWB, FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthRight = 0.5f;
            cell.BorderWidthBottom = 0.5f;
            cell.AddElement(para1);
            table.AddCell(cell);

            para = new Paragraph("POD", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.BorderWidthTop = 0f;
            cell.BorderWidthRight = 0f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph(": "+invoice.DischargePort, FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.BorderWidthTop = 0f;
            cell.BorderWidthLeft = 0f;
            cell.BorderWidthRight = 0f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("ETA : "+invoice.ETA.ToString("dd-MM-yyyy"), FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.BorderWidthTop = 0f;
            cell.BorderWidthLeft = 0f;
            cell.AddElement(para);
            table.AddCell(cell);

            para1 = new Paragraph("QUANTITY", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthBottom = 0.5f;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph(": "+shipment.ShipmentDetailsModel.Quantity+" x "+shipment.ShipmentDetailsModel.EquipmentType, FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthRight = 0.5f;
            cell.BorderWidthBottom = 0.5f;
            cell.AddElement(para1);
            table.AddCell(cell);

            para = new Paragraph("PRODUCT GROUP", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.BorderWidthTop = 0f;
            cell.BorderWidthRight = 0f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph(": "+shipment.ShipmentDetailsModel.ProductName, FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Colspan = 2;
            cell.BorderWidthTop = 0f;
            cell.BorderWidthLeft = 0f;
            cell.AddElement(para);
            table.AddCell(cell);

            para1 = new Paragraph("GROSS WT.", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthBottom = 0.5f;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph(": "+invoice.Grossweight+invoice.GrossweightUnit, FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthRight = 0.5f;
            cell.BorderWidthBottom = 0.5f;
            cell.AddElement(para1);
            table.AddCell(cell);

            para = new Paragraph("REMARKS", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.BorderWidthTop = 0f;
            cell.BorderWidthRight = 0f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph(": "+invoice.Remarks, FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Colspan = 4;
            cell.BorderWidthTop = 0f;
            cell.BorderWidthLeft = 0f;
            cell.AddElement(para);
            table.AddCell(cell);
            pdfDoc.Add(table);

            para = new Paragraph("Freight Charges:", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.SpacingBefore = 5f;
            para.SpacingAfter = 3f;
            para.IndentationLeft = 2f;
            pdfDoc.Add(para);

            table = new PdfPTable(9);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1;
            table.DefaultCell.PaddingLeft = 5f;
            table.SpacingAfter = 5f;

            para = new Paragraph("Description", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_LEFT;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("HSN Code", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("Eqp. Type", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("Qty", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("Rate", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);            

            para = new Paragraph("Currency", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("GST", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("Ex.Rate", FontFactory.GetFont("Arial", 7.7f, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("Amount in USD", FontFactory.GetFont("Arial", 7.7f, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            for (int i = 0; i < invoiceDetails.Count; i++)
            {
                para = new Paragraph(invoiceDetails[i].Description, FontFactory.GetFont("Arial", 7));
                para.Alignment = Element.ALIGN_LEFT;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph(" ", FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph(shipment.ShipmentDetailsModel.EquipmentType, FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph(invoiceDetails[i].Quantity.ToString(), FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph(invoiceDetails[i].UnitRate.ToString(), FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph(invoiceDetails[i].Currency, FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph(invoiceDetails[i].TaxPercent.ToString(), FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph(invoiceDetails[i].ExRate.ToString(), FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph(invoiceDetails[i].AmountUSD, FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);
            }

            para = new Paragraph(invoice.Amountinwords, FontFactory.GetFont("Arial", 8));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Colspan = 6;
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("GST  7%", FontFactory.GetFont("Arial", 8));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();            
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph(":", FontFactory.GetFont("Arial", 8));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph(" ", FontFactory.GetFont("Arial", 8));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();            
            cell.Border = 0;            
            cell.AddElement(para);
            table.AddCell(cell);


            para = new Paragraph(" ", FontFactory.GetFont("Arial", 8));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Colspan = 6;
            cell.Border = 0;
            cell.BorderWidthTop = 1f;
            cell.BorderWidthBottom = 1f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("Total", FontFactory.GetFont("Arial", 8,Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            cell.BorderWidthTop = 1f;
            cell.BorderWidthBottom = 1f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph(" ", FontFactory.GetFont("Arial", 8));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthTop = 1f;
            cell.BorderWidthBottom = 1f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph(invoice.AmountinUSDSUM, FontFactory.GetFont("Arial", 8,Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            cell.BorderWidthTop = 1f;
            cell.BorderWidthBottom = 1f;
            cell.AddElement(para);
            table.AddCell(cell);

            pdfDoc.Add(table);


            /*
            para = new Paragraph("Local Charges :", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.SpacingBefore = 5f;
            para.SpacingAfter = 3f;
            para.IndentationLeft = 2f;
            pdfDoc.Add(para);

            table = new PdfPTable(9);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1;
            table.DefaultCell.PaddingLeft = 5f;

            para = new Paragraph("Description", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_LEFT;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("HSN Code", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("Eqp. Type", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("Qty", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("Rate", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("Total Amount", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("Currency", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("Ex.Rate in SGD", FontFactory.GetFont("Arial", 7.7f, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("Amount in SGD", FontFactory.GetFont("Arial", 7.7f, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            for (int i = 0; i < 1; i++)
            {
                para = new Paragraph("BL FEE", FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_LEFT;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph("HSN Code", FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph("20GP", FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph("2", FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph("4000.00", FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph("4000.00", FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph("SGD", FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph("1.000000", FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph("4000.00", FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);
            }
            pdfDoc.Add(table);

            para = new Paragraph("Tax Details:", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.SpacingBefore = 15f;
            para.SpacingAfter = 3f;
            para.IndentationLeft = 2f;
            pdfDoc.Add(para);

            table = new PdfPTable(8);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1;
            table.DefaultCell.PaddingLeft = 5f;

            para = new Paragraph("Charge Head", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_LEFT;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("Size/Type ", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("Tax type", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("Percentage", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("Currency", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("Total Tax Amount", FontFactory.GetFont("Arial", 7.7f, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("Ex.Rate in SGD", FontFactory.GetFont("Arial", 7.7f, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("Amount in SGD", FontFactory.GetFont("Arial", 7.7f, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            for (int i = 0; i < 1; i++)
            {
                para = new Paragraph("BL FEE", FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_LEFT;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph("20GP", FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph("GST", FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph("5.00", FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph("SGD", FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph("229.20", FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph("1.000000", FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph("238.15", FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);
            }

            para = new Paragraph("\nAMOUNT IN WORDS" +
                "\nFive Thousand Three Hundred One Singapore Dollar and Sixteen Cents Only", FontFactory.GetFont("Arial", 8));
            para.Alignment = Element.ALIGN_LEFT;
            cell = new PdfPCell();
            cell.Colspan = 4;
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("\n\nGST 5.00", FontFactory.GetFont("Arial", 8));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Colspan = 3;
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("\n\n238.15", FontFactory.GetFont("Arial", 8));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Colspan = 1;
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph(" ", FontFactory.GetFont("Arial", 8));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Colspan = 4;
            cell.Border = 0;
            cell.BorderWidthTop = 0.5f;
            cell.BorderWidthBottom = 0.5f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("Total", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Colspan = 3;
            cell.Border = 0;
            cell.BorderWidthTop = 0.5f;
            cell.BorderWidthBottom = 0.5f;
            cell.PaddingBottom = 5f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("5301.16", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthTop = 0.5f;
            cell.BorderWidthBottom = 0.5f;
            cell.PaddingBottom = 5f;
            cell.AddElement(para);
            table.AddCell(cell);
            pdfDoc.Add(table);
            */


            table = new PdfPTable(2);
            table.SetTotalWidth(new float[] { 80, 50 });
            table.LockedWidth = true;
            table.SpacingBefore = 3f;
            table.HorizontalAlignment = 0;

            para = new Paragraph("Equipment No", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_LEFT;
            cell = new PdfPCell();
            cell.BackgroundColor = new BaseColor(211, 211, 211);
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("Size/Type", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_LEFT;
            cell = new PdfPCell();
            cell.BackgroundColor = new BaseColor(211, 211, 211);
            cell.AddElement(para);
            table.AddCell(cell);

            if (invoice.ContainerNo != null)
            {
                String[] Containers = invoice.ContainerNo.Split(',');

                for (int i = 0; i < Containers.Count(); i++)
                {
                    para = new Paragraph(Containers[i], FontFactory.GetFont("Arial", 8));
                    para.Alignment = Element.ALIGN_LEFT;
                    cell = new PdfPCell();
                    cell.AddElement(para);
                    table.AddCell(cell);

                    para = new Paragraph(shipment.ShipmentDetailsModel.EquipmentType, FontFactory.GetFont("Arial", 8));
                    para.Alignment = Element.ALIGN_LEFT;
                    cell = new PdfPCell();
                    cell.AddElement(para);
                    table.AddCell(cell);
                }
            }
            else
            {
                para = new Paragraph(" ", FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_LEFT;
                cell = new PdfPCell();
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph(" ", FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_LEFT;
                cell = new PdfPCell();
                cell.AddElement(para);
                table.AddCell(cell);
            }
            pdfDoc.Add(table);

            para = new Paragraph("Bank Details", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.SpacingBefore = 2f;
            para.SpacingAfter = 10f;
            pdfDoc.Add(para);

            para = new Paragraph("Terms and Conditions:", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.SpacingAfter = 2f;
            pdfDoc.Add(para);

            para = new Paragraph("1) Terms of Sale" +
                "Clear demarcation of terms of sales will wipe-out any potential chance of misunderstanding or disagreement from any of the parties. Hence, it is of utmost" +
                "importance to mention the terms of sales like - cost, quantity, single unit cost, delivery date or time of service, payment method or credit, if any.", FontFactory.GetFont("Arial", 8));
            para.SpacingAfter = 4f;
            pdfDoc.Add(para);

            para = new Paragraph("2) Payment in Advance" +
                "It is one of the most usual payment terms, where the service provider asks for full or partial payment before the delivery of product or service. This is prevalent in" +
                "the service industry and is followed to avoid after - sales non - payment recovery.It is practiced to avoid out -of - pocket expenses to finish the project.", FontFactory.GetFont("Arial", 8));
            para.SpacingAfter = 4f;
            pdfDoc.Add(para);

            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "inline;filename=Export Invoice.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
        }
    }
}