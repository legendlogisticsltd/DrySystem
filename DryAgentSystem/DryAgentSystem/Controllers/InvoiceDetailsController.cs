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
        public void InvoiceDetails(string jobref)
        {
            ShipmentBL shipment = DataContext.GetShipmentFromJobRef(jobref);
            Invoice invoicenew = new Invoice();
            if (ModelState.IsValid)
            {
                invoicenew.UniversalSerialNr = shipment.ShipmentDetailsModel.UniversalSerialNr;
                invoicenew.Grossweight = shipment.BLDetailsModel.TotalGweight;
                invoicenew.GrossweightUnit = shipment.BLDetailsModel.GrossWeightUnit;
                invoicenew.BillingParty = shipment.ShipmentDetailsModel.Shipper;
                invoicenew.Remarks = shipment.ShipmentDetailsModel.Remark;
                invoicenew.JobRefNo = shipment.ShipmentDetailsModel.JobRef;
                invoicenew.VesselName = shipment.TransVessel[0].VesselName;
                invoicenew.VoyageNo = shipment.TransVessel[0].VoyNo;

                ModelState.Clear();
            }
            else
            {
                TempData["Message"] = "Please check the fields, some of the fields are not in correct format";
                ModelState.Clear();
            }

            invoicenew.ProformaInvoiceNo = DataContext.GetInvoiceFromUSN(invoicenew.UniversalSerialNr).ProformaInvoiceNo;
            if(String.IsNullOrEmpty(invoicenew.ProformaInvoiceNo))
            {
                InvoiceDetails(invoicenew, "Save");
                ExportInvoicePDF(invoicenew.JobRefNo);
                
            }
            else
            {
                ExportInvoicePDF(invoicenew.JobRefNo);
            }
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
                        invoice.ProformaInvoiceNo = errorLog.ErrorMessage;
                        //TempData["Message"] = "Invoice successfully created Invoice No. " + invoice.InvoiceNo + ". Please click Print Invoice to view PDF";
                    }
                    else
                    {
                        TempData["Message"] = errorLog.ErrorMessage;
                    }
                }

                ModelState.Clear();
                
            }
            else
            {
                TempData["Message"] = "Invoice was not saved. Please check and try again";
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
            table.SetTotalWidth(new float[] { 72, 350 });
            table.LockedWidth = true;
            table.SpacingAfter = 5f;

            cell = new PdfPCell();
            cell.Border = 0;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            Image image1 = Image.GetInstance(Server.MapPath("~/Content/Img/LogoOnlyStar.png"));
            image1.ScaleAbsolute(70, 70);
            image1.Alignment = Element.ALIGN_RIGHT;
            cell.AddElement(image1);
            table.AddCell(cell);

            chunk = new Chunk("LEGEND CONTAINER LINE PTE LTD", FontFactory.GetFont("Times", 19, Font.BOLD, new BaseColor(0, 0, 128)));
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

            table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1;
            table.DefaultCell.PaddingLeft = 5f;

            para = new Paragraph("PROFORMA EXPORT INVOICE", FontFactory.GetFont("Arial", 13, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.Colspan = 6;
            cell.BackgroundColor = new BaseColor(211, 211, 211);
            cell.PaddingBottom = 10f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph((string.IsNullOrEmpty(invoice.BillingPartyAddress) ? string.Empty : invoice.BillingPartyAddress.ToUpperInvariant()) +
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

            para1 = new Paragraph(": "+invoice.ProformaInvoiceNo, FontFactory.GetFont("Arial", 8));
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

            string CreditTerm = string.Empty;
            if (invoice.CreditTerms <= 0)
            {
                CreditTerm = "COD";
            }
            else
            {
                CreditTerm = invoice.CreditTerms.ToString();
            }

            para1 = new Paragraph("CREDIT TERMS", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph(": "+CreditTerm, FontFactory.GetFont("Arial", 8));
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

            para = new Paragraph(": "+invoice.VesselName+" - "+invoice.VoyageNo, FontFactory.GetFont("Arial", 8));
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

            para1 = new Paragraph(": "+shipment.ShipmentDetailsModel.QuantityLifting+" x "+shipment.ShipmentDetailsModel.EquipmentType, FontFactory.GetFont("Arial", 8));
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

            para = new Paragraph(": " + (string.IsNullOrEmpty(invoice.Remarks) ? string.Empty : invoice.Remarks.ToUpperInvariant()), FontFactory.GetFont("Arial", 8));
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

            para1 = new Paragraph(": "+invoice.Grossweight+" "+invoice.GrossweightUnit, FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthRight = 0.5f;
            cell.BorderWidthBottom = 0.5f;
            cell.AddElement(para1);
            table.AddCell(cell);
            pdfDoc.Add(table);

            para = new Paragraph("FREIGHT CHARGES:", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.SpacingBefore = 5f;
            para.SpacingAfter = 3f;
            para.IndentationLeft = 2f;
            pdfDoc.Add(para);

            table = new PdfPTable(8);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 1;
            table.DefaultCell.PaddingLeft = 5f;
            table.SpacingAfter = 5f;

            para = new Paragraph("DESCRIPTION", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_LEFT;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("EQP. TYPE", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("QTY", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("RATE", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);            

            para = new Paragraph("CURRENCY", FontFactory.GetFont("Arial", 8, Font.BOLD));
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

            para = new Paragraph("EX.RATE", FontFactory.GetFont("Arial", 7.7f, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("AMOUNT IN USD", FontFactory.GetFont("Arial", 7.7f, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            //float.Parse(shipment.BLDetailsModel.TotalGweight).ToString("0.00")

            for (int i = 0; i < invoiceDetails.Count; i++)
            {
                para = new Paragraph((string.IsNullOrEmpty(invoiceDetails[i].Description) ? string.Empty : invoiceDetails[i].Description.ToUpperInvariant()), FontFactory.GetFont("Arial", 7));
                para.Alignment = Element.ALIGN_LEFT;
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

                para = new Paragraph(invoiceDetails[i].UnitRate.ToString("0.00"), FontFactory.GetFont("Arial", 8));
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

                para = new Paragraph(invoiceDetails[i].TaxPercent.ToString("0.00"), FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph(invoiceDetails[i].ExRate.ToString("0.00"), FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph(invoiceDetails[i].AmountUSD.ToString("0.00"), FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_RIGHT;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);
            }

            //Dummy data for space
            para = new Paragraph(" ", FontFactory.GetFont("Arial", 7));
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

            para = new Paragraph(" ", FontFactory.GetFont("Arial", 8));
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
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph(" ", FontFactory.GetFont("Arial", 8));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            //Data Ends here


            para = new Paragraph(" ", FontFactory.GetFont("Arial", 8,Font.BOLD));
            para.Alignment = Element.ALIGN_LEFT;
            cell = new PdfPCell();
            cell.Colspan = 5;
            cell.Border = 0;
            cell.BorderWidthTop = 1;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("SUB TOTAL", FontFactory.GetFont("Arial", 8));
            para.Alignment = Element.ALIGN_LEFT;
            cell = new PdfPCell();            
            cell.Border = 0;
            cell.BorderWidthTop = 1;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph(":", FontFactory.GetFont("Arial", 8));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthTop = 1;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph(invoice.AmountinUSDSUMWTax.ToString("0.00"), FontFactory.GetFont("Arial", 8));
            para.Alignment = Element.ALIGN_RIGHT;
            cell = new PdfPCell();            
            cell.Border = 0;
            cell.BorderWidthTop = 1;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("AMOUNT IN WORDS:", FontFactory.GetFont("Arial", 8,Font.BOLD));
            para.Alignment = Element.ALIGN_LEFT;
            cell = new PdfPCell();
            cell.Colspan = 5;
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("GST  7.00%", FontFactory.GetFont("Arial", 8));
            para.Alignment = Element.ALIGN_LEFT;
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

            para = new Paragraph(invoice.TaxAmountSum.ToString("0.00"), FontFactory.GetFont("Arial", 8));
            para.Alignment = Element.ALIGN_RIGHT;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);


            para = new Paragraph((string.IsNullOrEmpty(invoice.Amountinwords) ? string.Empty : invoice.Amountinwords.ToUpperInvariant()), FontFactory.GetFont("Arial", 8));
            para.Alignment = Element.ALIGN_LEFT;
            cell = new PdfPCell();
            cell.Colspan = 5;
            cell.Border = 0;
            cell.BorderWidthBottom = 1f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("GRAND TOTAL", FontFactory.GetFont("Arial", 8,Font.BOLD));
            para.Alignment = Element.ALIGN_LEFT;
            cell = new PdfPCell();
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            cell.BorderWidthBottom = 1f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph(":", FontFactory.GetFont("Arial", 8));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthBottom = 1f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("USD   "+invoice.AmountinUSDSUM.ToString("0.00"), FontFactory.GetFont("Arial", 8,Font.BOLD));
            para.Alignment = Element.ALIGN_RIGHT;
            cell = new PdfPCell();
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            cell.BorderWidthBottom = 1f;
            cell.AddElement(para);
            table.AddCell(cell);

            pdfDoc.Add(table);


            table = new PdfPTable(2);
            table.SetTotalWidth(new float[] { 80, 50 });
            table.LockedWidth = true;
            table.SpacingBefore = 3f;
            table.HorizontalAlignment = 0;

            para = new Paragraph("EQUIPMENT NO", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_LEFT;
            cell = new PdfPCell();
            cell.BackgroundColor = new BaseColor(211, 211, 211);
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("SIZE/TYPE", FontFactory.GetFont("Arial", 8, Font.BOLD));
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

            para = new Paragraph("BANK DETAILS", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.SpacingBefore = 10f;
            para.SpacingAfter = 10f;
            pdfDoc.Add(para);

            para = new Paragraph("TERMS AND CONDITIONS:", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.SpacingAfter = 7f;
            pdfDoc.Add(para);

            para = new Paragraph("1) Terms of Sale", FontFactory.GetFont("Arial", 8));
            para.SpacingAfter = 4f;
            pdfDoc.Add(para);

            para = new Paragraph("Clear demarcation of terms of sales will wipe-out any potential chance of misunderstanding or disagreement from any of the parties. Hence, it is of utmost " +
                "importance to mention the terms of sales like - cost, quantity, single unit cost, delivery date or time of service, payment method or credit, if any.", FontFactory.GetFont("Arial", 8));
            para.SpacingAfter = 8f;
            pdfDoc.Add(para);

            para = new Paragraph("2) Payment in Advance", FontFactory.GetFont("Arial", 8));
            para.SpacingAfter = 4f;
            pdfDoc.Add(para);

            para = new Paragraph("It is one of the most usual payment terms, where the service provider asks for full or partial payment before the delivery of product or service. This is prevalent in " +
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