using DryAgentSystem.Data;
using DryAgentSystem.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DryAgentSystem.Controllers
{
    public class ImportDocumentController : Controller
    {
        // GET: ImportDocument
        [Authorize]
        [Route("ImportDocument")]

        public ActionResult ImportDocument()
        {
            SearchParameters statusdisplay = new SearchParameters();
            ViewBag.statuslist = DataContext.GetImportDocStatus();
            ViewBag.PortList = DataContext.GetCountryPorts();
            //ViewBag.CompanyList = DataContext.GetCompany();
            return View();
        }

        [HttpPost]
        [Route("ImportDocument")]
        public ActionResult ImportDocument(SearchParameters filter, string submit)
        {
            ViewBag.statuslist = DataContext.GetImportDocStatus();
            ViewBag.PortList = DataContext.GetCountryPorts();
            //ViewBag.jobRefList = DataContext.GetCompany();

            if (submit == "Search")
            {
                TempData["SearchParameters"] = filter;
                return View();
            }

            else
            {
                return RedirectToAction("ImportDocument", "ImportDocument");
            }
        }

        public JsonResult GetBLData(string sidx, string sort, int page, int rows)
        {
            SearchParameters search = new SearchParameters();
            if (TempData["SearchParameters"] != null)
            {
                search = (SearchParameters)TempData["SearchParameters"];
            }
            var BLData = DataContext.GetBLDetails(search);
            int totalRecords = BLData.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = (from BLDataGrid in BLData
                        select new
                        {
                            BLDataGrid.JobRef,
                            cell = new string[]
                            {
                                BLDataGrid.JobRef,
                                BLDataGrid.BLTypes,
                                BLDataGrid.ShipperNameBL,
                                BLDataGrid.ConsigneeNameBL,
                                BLDataGrid.DischPort,
                                BLDataGrid.LoadPort,
                                BLDataGrid.CountryPOL
                            }
                        }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

        public void CreateCAN(string JobRefId)
        {
            ShipmentBL DetBL = DataContext.GetCRADetailsJobRef(JobRefId);
            //QuoteRef quoteRef = DataContext.GetQuoteRequestFromQuoteID(booking.QuoteRefID);
            //List<Vessel> vessels = DataContext.GetVesselsDetails(booking.UniversalSerialNr).ToList();

            Document pdfDoc = new Document(PageSize.A4, 40, 40, 20, 20);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.AddTitle("ARRIVAL NOTICE");
            pdfDoc.Open();
            Chunk chunk, chunk1;
            Paragraph para, para2, para3, paraSp;
            PdfPTable table;
            PdfPCell cell;

            #region PDF Logo Address And Name

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

            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 3)));
            pdfDoc.Add(line);

            chunk = new Chunk("ARRIVAL NOTICE", FontFactory.GetFont("Arial", 13, Font.BOLD));
            para = new Paragraph(chunk);
            para.Alignment = Element.ALIGN_CENTER;
            pdfDoc.Add(para);

            #endregion

            #region Line 1 Shipper

            table = new PdfPTable(4);
            table.SetTotalWidth(new float[] { 200, 65, 100, 150 });
            table.LockedWidth = true;
            table.SpacingBefore = 5f;
            //table.SpacingAfter = 5f;

            chunk = new Chunk("SHIPPER,", FontFactory.GetFont("Arial", 9, Font.BOLD));
            para = new Paragraph(DetBL.BLDetailsModel.ShipperAddressBL, FontFactory.GetFont("Arial", 9));

            paraSp = new Paragraph(" ", FontFactory.GetFont("Arial", 9));

            chunk1 = new Chunk("CONSIGNEE,", FontFactory.GetFont("Arial", 9, Font.BOLD));
            para2 = new Paragraph(DetBL.BLDetailsModel.ConsigneeAddressBL, FontFactory.GetFont("Arial", 9));

            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.BorderWidthRight = 0;
            cell.AddElement(chunk);
            cell.AddElement(para);
            cell.AddElement(paraSp);
            cell.AddElement(chunk1);
            cell.AddElement(para2);
            table.AddCell(cell);

            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.BorderWidthLeft = 0;
            table.AddCell(cell);

            Paragraph para1 = new Paragraph("BL NO \n" +
                "DATE \n" +
                "TERM \n" +
                "CARRIER \n" +
                "VESSEL \n" +
                "VOYAGE \n" +
                "POL \n" +
                "POD \n" +
                "ETA ", FontFactory.GetFont("Arial", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.BorderWidthRight = 0;
            cell.AddElement(para1);
            table.AddCell(cell);

            DateTime dateTime = DateTime.Now;
            para = new Paragraph(":  " + DetBL.BLDetailsModel.JobRef + " \n" +
                ":  " + dateTime.ToString("dd-MM-yyyy") + " \n" +
                ":  " + DetBL.ShipmentDetailsModel.ShipmentTerm + " \n" +
                ":  " + DetBL.VesselModel.Carrier + " \n" +
                ":  " + DetBL.VesselModel.VesselName + " \n" +
                ":  " + DetBL.VesselModel.VoyNo + " \n" +
                ":  " + DetBL.BLDetailsModel.LoadPort + " \n" +
                ":  " + DetBL.BLDetailsModel.DischPort + " \n" +
                ":  " + DetBL.VesselModel.ETA.ToString("dd-MM-yyyy") + " ", FontFactory.GetFont("Arial", 9));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.BorderWidthLeft = 0;
            cell.AddElement(para);
            table.AddCell(cell);
            pdfDoc.Add(table);

            #endregion

            #region Line 2 DESCRIPTION OF GOODS

            table = new PdfPTable(4);
            table.SetTotalWidth(new float[] { 200, 65, 240, 10 });
            table.LockedWidth = true;
            table.SpacingAfter = 5f;

            chunk = new Chunk("DESCRIPTION OF GOODS,", FontFactory.GetFont("Arial", 9, Font.BOLD));
            para = new Paragraph(DetBL.BLDetailsModel.CargoDescription, FontFactory.GetFont("Arial", 9));

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

            chunk = new Chunk("PLEASE PRESENT THE ORIGINAL BILL OF LADING, IF REQUIRED AND ANY IMPORT PERMIT(S) IN EXCHANGE FOR OUR DELIVERY ORDER PRIOR TO VESSEL ARRIVAL. COLLECTION/ EXCHANGE OF DELIVERY ORDER FROM OUR OFFICE DURING THE HOURS AS FOLLOWS: -  \n", FontFactory.GetFont("Arial", 9));
            Paragraph paraS = new Paragraph(" ", FontFactory.GetFont("Arial", 9));
            Paragraph para4 = new Paragraph("MON TO FRI : 9:00 AM TO 5:00 PM ", FontFactory.GetFont("Arial", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 5f;
            cell.BorderWidthRight = 0;
            cell.AddElement(chunk);
            cell.AddElement(paraS);
            cell.AddElement(para4);
            table.AddCell(cell);

            cell = new PdfPCell();
            cell.BorderWidthLeft = 0;
            table.AddCell(cell);
            pdfDoc.Add(table);

            #endregion

            #region Grid Header
            //3rd size and type table
            table = new PdfPTable(7);
            table.SetTotalWidth(new float[] { 80, 60, 80, 80, 70, 70, 70 });
            table.LockedWidth = true;
            table.SpacingBefore = 20f;
            table.PaddingTop = 10f;

            para1 = new Paragraph("Equipment No", FontFactory.GetFont("Arial", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 3f;
            cell.Border = 0;
            cell.BorderWidthTop = 1;
            cell.BorderWidthBottom = 1;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph("Type \n", FontFactory.GetFont("Arial", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 3f;
            cell.Border = 0;
            cell.BorderWidthTop = 1;
            cell.BorderWidthBottom = 1;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph("Line Seal No", FontFactory.GetFont("Arial", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 3f;
            cell.Border = 0;
            cell.BorderWidthTop = 1;
            cell.BorderWidthBottom = 1;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph("No of Pkgs(s)", FontFactory.GetFont("Arial", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 3f;
            //cell.PaddingLeft = 45f;
            cell.Border = 0;
            cell.BorderWidthTop = 1;
            cell.BorderWidthBottom = 1;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph("Pkg Type", FontFactory.GetFont("Arial", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 3f;
            //cell.PaddingLeft = 45f;
            cell.Border = 0;
            cell.BorderWidthTop = 1;
            cell.BorderWidthBottom = 1;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph("Volume", FontFactory.GetFont("Arial", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 3f;
            //cell.PaddingLeft = 45f;
            cell.Border = 0;
            cell.BorderWidthTop = 1;
            cell.BorderWidthBottom = 1;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph("Gross Wt", FontFactory.GetFont("Arial", 9, Font.BOLD));
            cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Padding = 3f;
            //cell.PaddingLeft = 45f;
            cell.Border = 0;
            cell.BorderWidthTop = 1;
            cell.BorderWidthBottom = 1;
            cell.AddElement(para1);
            table.AddCell(cell);

            #endregion

            #region Grid Data Line
            for (int i = 0; i < DetBL.DevAllocateEquipmentModel.Count; i++)
            {
                // 3rd table Data Stars Here
                //Equipment No
                para3 = new Paragraph(DetBL.DevAllocateEquipmentModel[i].ContainerNo, FontFactory.GetFont("Arial", 9));
                cell = new PdfPCell();
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Padding = 3f;
                cell.Border = 0;
                cell.AddElement(para3);
                table.AddCell(cell);

                //Type
                para3 = new Paragraph(DetBL.QuoteRefModel.EquipmentType, FontFactory.GetFont("Arial", 9));
                cell = new PdfPCell();
                cell.Padding = 3f;
                //cell.PaddingLeft = 15f;
                cell.Border = 0;
                cell.AddElement(para3);
                table.AddCell(cell);

                //Line Seal No
                para3 = new Paragraph(DetBL.DevAllocateEquipmentModel[i].SealNo, FontFactory.GetFont("Arial", 9));
                cell = new PdfPCell();
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Padding = 3f;
                cell.Border = 0;
                cell.AddElement(para3);
                table.AddCell(cell);

                //No of Pkgs(s)
                para3 = new Paragraph(DetBL.BLDetailsModel.NoOfPkgs.ToString(), FontFactory.GetFont("Arial", 9));
                cell = new PdfPCell();
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Padding = 3f;
                cell.Border = 0;
                cell.AddElement(para3);
                table.AddCell(cell);

                //Pkg Type
                para3 = new Paragraph(DetBL.BLDetailsModel.PkgType, FontFactory.GetFont("Arial", 9));
                cell = new PdfPCell();
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Padding = 3f;
                //cell.PaddingLeft = 15f;
                cell.Border = 0;
                cell.AddElement(para3);
                table.AddCell(cell);

                //Volume
                //int decVol = Convert.ToInt32(DetBL.DevAllocateEquipmentModel[i].Measurement);
                string DecVolume = Convert.ToDecimal(DetBL.DevAllocateEquipmentModel[i].Measurement).ToString("#.00");

                if (DecVolume == "")
                {
                    para3 = new Paragraph("0.00" + " " + DetBL.DevAllocateEquipmentModel[i].MeasurementUnit, FontFactory.GetFont("Arial", 9));
                }
                else
                {
                    para3 = new Paragraph(DecVolume + " " + DetBL.DevAllocateEquipmentModel[i].MeasurementUnit, FontFactory.GetFont("Arial", 9));
                }
                cell = new PdfPCell();
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.Padding = 3f;
                cell.Border = 0;
                cell.AddElement(para3);
                table.AddCell(cell);

                //Gross Wt

                //int decGW = Convert.ToInt32(DetBL.BLDetailsModel.TotalGweight);
                //string DecGrossW = Convert.ToDecimal(DetBL.BLDetailsModel.TotalGweight).ToString("#.00");
                //if (DecGrossW == 0)
                //{
                //    para3 = new Paragraph("0.00", FontFactory.GetFont("Arial", 9));
                //}
                //else
                //{
                //    para3 = new Paragraph(DecGrossW, FontFactory.GetFont("Arial", 9));
                //}

                string DecGrossW = Convert.ToDecimal(DetBL.DevAllocateEquipmentModel[i].GrossWeight).ToString("#.00");

                if (DecGrossW == "")
                {
                    para3 = new Paragraph("0.00" + " " + DetBL.DevAllocateEquipmentModel[i].GrossWeightUnit, FontFactory.GetFont("Arial", 9));
                }
                else
                {
                    para3 = new Paragraph(DecGrossW + " " + DetBL.DevAllocateEquipmentModel[i].GrossWeightUnit, FontFactory.GetFont("Arial", 9));
                }
                cell = new PdfPCell();
                cell.Padding = 3f;
                //cell.PaddingLeft = 15f;
                cell.Border = 0;
                cell.AddElement(para3);
                table.AddCell(cell);
            }
            pdfDoc.Add(table);

            #endregion
            para = new Paragraph(" ", FontFactory.GetFont("Arial", 10, Font.BOLD));
            pdfDoc.Add(para);
            para = new Paragraph(" ", FontFactory.GetFont("Arial", 10, Font.BOLD));
            pdfDoc.Add(para);
            para = new Paragraph(" ", FontFactory.GetFont("Arial", 10, Font.BOLD));
            pdfDoc.Add(para);

            para = new Paragraph("Remarks", FontFactory.GetFont("Arial", 10, Font.BOLD));
            pdfDoc.Add(para);
            para = new Paragraph(DetBL.ShipmentDetailsModel.Remark, FontFactory.GetFont("Arial", 10));
            pdfDoc.Add(para);
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "inline;filename=Cargo Arrival Notice.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
        }


        public void CreateImportInvoice(string JobRefId)
        {
            ShipmentBL DetShipBL = DataContext.GetShipmentDetailsFromJobRefForImportInvoice(JobRefId);

            ErrorLog errorlog = DataContext.SaveSalesInvoiceData(DetShipBL);
            if (!errorlog.IsError)
            {
                GenerateImportInvoicePDF(DetShipBL.ShipmentDetailsModel.JobRef);
            }
            else
            {
                TempData["Message"] = errorlog.ErrorMessage;
            }
            //GenerateImportInvoicePDF("LDBJEAMAA21050011");

        }

        public void GenerateImportInvoicePDF(string JobRefId)
        {
            ImportInvoiceDetails InVDet = DataContext.GetImportInvoiceDetailsJobRef(JobRefId);

            Document pdfDoc = new Document(PageSize.A4, 20, 20, 20, 20);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.AddTitle("Import Invoice");
            pdfDoc.Open();
            Chunk chunk;
            Paragraph para, para1;
            PdfPTable table;
            PdfPCell cell;
            string DecAmtSumTax = "";
            string DecGSTAmtTaxSum = "";
            string DecTotal = "";

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

            para = new Paragraph("PROFORMA IMPORT INVOICE", FontFactory.GetFont("Arial", 13, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.Colspan = 6;
            cell.BackgroundColor = new BaseColor(211, 211, 211);
            cell.PaddingBottom = 10f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph(InVDet.SalesInvoiceDryModel.BillingPartyAddress +
                                "\n ", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Colspan = 3;
            cell.Rowspan = 4;
            cell.BorderWidthBottom = 0f;
            cell.AddElement(para);
            table.AddCell(cell);

            para1 = new Paragraph("  INVOICE NO.", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.BorderWidthBottom = 0f;
            cell.BorderWidthRight = 0f;
            cell.BorderWidthLeft = 0f;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph(": " + InVDet.SalesInvoiceDryModel.InvoiceNo, FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.BorderWidthBottom = 0f;
            cell.BorderWidthLeft = 0f;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph("  DATE", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph(": " + InVDet.SalesInvoiceDryModel.InvoiceDate.ToString("dd-MM-yyyy"), FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthRight = 0.5f;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph("  JOB REF", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph(": " + InVDet.SalesInvoiceDryModel.JobRefNo, FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthRight = 0.5f;
            cell.AddElement(para1);
            table.AddCell(cell);

            string CerditTerm = string.Empty;
            if (InVDet.SalesInvoiceDryModel.CreditTerms <= 0)
            {
                CerditTerm = "COD";
            }
            else
            {
                CerditTerm = InVDet.SalesInvoiceDryModel.CreditTerms.ToString();
            }

            para1 = new Paragraph("  CREDIT TERMS", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph(": " + CerditTerm, FontFactory.GetFont("Arial", 8));
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

            string DueDate = string.Empty;
            if (InVDet.SalesInvoiceDryModel.CreditTerms <= 0)
            {
                DueDate = DateTime.Now.ToString("dd-MM-yyyy");
            }
            else
            {
                DueDate = InVDet.SalesInvoiceDryModel.DueDate.ToString("dd-MM-yyyy");
            }

            para1 = new Paragraph("  DUE DATE ", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthBottom = 0.5f;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph(": " + DueDate, FontFactory.GetFont("Arial", 8));
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

            para = new Paragraph(": " + InVDet.SalesInvoiceDryModel.VesselName + " / " + InVDet.SalesInvoiceDryModel.VoyageNo, FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Colspan = 2;
            cell.BorderWidthTop = 0f;
            cell.BorderWidthLeft = 0f;
            cell.AddElement(para);
            table.AddCell(cell);

            para1 = new Paragraph("  MASTER BL", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthBottom = 0.5f;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph(": " + InVDet.SalesInvoiceDryModel.MBLHAWB, FontFactory.GetFont("Arial", 8));
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

            para = new Paragraph(": " + InVDet.SalesInvoiceDryModel.LoadPort, FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.BorderWidthTop = 0f;
            cell.BorderWidthLeft = 0f;
            cell.BorderWidthRight = 0f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("ETD : " + InVDet.SalesInvoiceDryModel.ETD.ToString("dd-MM-yyyy"), FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.BorderWidthTop = 0f;
            cell.BorderWidthLeft = 0f;
            cell.AddElement(para);
            table.AddCell(cell);

            para1 = new Paragraph("  HOUSE BL", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthBottom = 0.5f;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph(": " + InVDet.SalesInvoiceDryModel.HBLHAWB, FontFactory.GetFont("Arial", 8));
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

            para = new Paragraph(": " + InVDet.SalesInvoiceDryModel.DischargePort, FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.BorderWidthTop = 0f;
            cell.BorderWidthLeft = 0f;
            cell.BorderWidthRight = 0f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("ETA : " + InVDet.SalesInvoiceDryModel.ETA.ToString("dd-MM-yyyy"), FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.BorderWidthTop = 0f;
            cell.BorderWidthLeft = 0f;
            cell.AddElement(para);
            table.AddCell(cell);

            para1 = new Paragraph("  QUANTITY", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthBottom = 0.5f;
            cell.AddElement(para1);
            table.AddCell(cell);

            if (InVDet.salesInvoiceLineItemDryModel.Count > 0)
            {
                para1 = new Paragraph(": " + InVDet.ShipmentDetailsModel.QuantityLifting + " x " + InVDet.ShipmentDetailsModel.EquipmentType, FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.BorderWidthRight = 0.5f;
                cell.BorderWidthBottom = 0.5f;
                cell.AddElement(para1);
                table.AddCell(cell);
            }
            else
            {
                para1 = new Paragraph(": " + "0" + " x " + InVDet.ShipmentDetailsModel.EquipmentType, FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.BorderWidthRight = 0.5f;
                cell.BorderWidthBottom = 0.5f;
                cell.AddElement(para1);
                table.AddCell(cell);
            }

            para = new Paragraph("REMARKS", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.BorderWidthTop = 0f;
            cell.BorderWidthRight = 0f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph(": " + InVDet.SalesInvoiceDryModel.Remarks, FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Colspan = 2;
            cell.BorderWidthTop = 0f;
            cell.BorderWidthLeft = 0f;
            cell.AddElement(para);
            table.AddCell(cell);

            para1 = new Paragraph("  GROSS WT.", FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthBottom = 0.5f;
            cell.AddElement(para1);
            table.AddCell(cell);

            para1 = new Paragraph(": " + InVDet.SalesInvoiceDryModel.Grossweight.ToString("0.00") +" "+ InVDet.SalesInvoiceDryModel.GrossWeightUnit, FontFactory.GetFont("Arial", 8));
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthRight = 0.5f;
            cell.BorderWidthBottom = 0.5f;
            cell.AddElement(para1);
            table.AddCell(cell);
            pdfDoc.Add(table);

            para = new Paragraph("Freight Charges:", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.SpacingBefore = 10f;
            para.SpacingAfter = 10f;
            para.IndentationLeft = 2f;
            pdfDoc.Add(para);

            table = new PdfPTable(8);
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

            para = new Paragraph("Ex-Rate", FontFactory.GetFont("Arial", 8, Font.BOLD)); //Ex.Rate in USD
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

            for (int i = 0; i < InVDet.salesInvoiceLineItemDryModel.Count; i++)
            {
                para = new Paragraph(InVDet.salesInvoiceLineItemDryModel[i].Description, FontFactory.GetFont("Arial", 7));
                para.Alignment = Element.ALIGN_LEFT;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph(InVDet.ShipmentDetailsModel.EquipmentType, FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph(InVDet.salesInvoiceLineItemDryModel[i].Quantity.ToString(), FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                double decRt = InVDet.salesInvoiceLineItemDryModel[i].UnitRate;
                string DecRate = Convert.ToDecimal(decRt).ToString("#.00");

                para = new Paragraph(DecRate, FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph(InVDet.salesInvoiceLineItemDryModel[i].Currency, FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                para = new Paragraph(InVDet.salesInvoiceLineItemDryModel[i].TaxAmount.ToString(), FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                decimal decExrate = Convert.ToDecimal(InVDet.salesInvoiceLineItemDryModel[i].ExRate);
                string Exrate = Math.Round(decExrate, 4).ToString("0.0000");

                para = new Paragraph(Exrate, FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);

                decimal decAmountUSD = Convert.ToDecimal(InVDet.salesInvoiceLineItemDryModel[i].AmountUSD);
                string AmountUSD = "";
                if (decAmountUSD == 0)
                {
                    AmountUSD = "0.00";
                }
                else
                {
                    AmountUSD = Math.Round(decAmountUSD, 2).ToString("0.00");
                }

                para = new Paragraph(AmountUSD, FontFactory.GetFont("Arial", 8));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para);
                table.AddCell(cell);
            }
            //-------// Blank Space Line
            para = new Paragraph(" ", FontFactory.GetFont("Arial", 8));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Colspan = 5;
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
            //-------// SubTotal Line
            para = new Paragraph(" ", FontFactory.GetFont("Arial", 8));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Colspan = 5;
            cell.Border = 0;
            cell.BorderWidthTop = 1f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("SubTotal", FontFactory.GetFont("Arial", 8));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthTop = 1f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph(":", FontFactory.GetFont("Arial", 8));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthTop = 1f;
            cell.AddElement(para);
            table.AddCell(cell);

            if (InVDet.SalesInvoiceDryModel.AmountinUSDSUMWTax == 0)
            {
                DecAmtSumTax = "0.00";
            }
            else
            {
                double decAmtTax = InVDet.SalesInvoiceDryModel.AmountinUSDSUMWTax;
                DecAmtSumTax = Convert.ToDecimal(decAmtTax).ToString("0.00");
            }

            para = new Paragraph(DecAmtSumTax, FontFactory.GetFont("Arial", 8));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.BorderWidthTop = 1f;
            cell.AddElement(para);
            table.AddCell(cell);
            //-------// GST Line
            para = new Paragraph(InVDet.SalesInvoiceDryModel.AmountInWords, FontFactory.GetFont("Arial", 8));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Colspan = 5;
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("GST 7%", FontFactory.GetFont("Arial", 8));

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

            if (InVDet.SalesInvoiceDryModel.TaxAmountSum == 0)
            {
                DecGSTAmtTaxSum = "0.00";
            }
            else
            {
                double decGSTAmt = InVDet.SalesInvoiceDryModel.TaxAmountSum;
                DecGSTAmtTaxSum = Convert.ToDecimal(decGSTAmt).ToString("0.00");
            }

            para = new Paragraph(DecGSTAmtTaxSum, FontFactory.GetFont("Arial", 8));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(para);
            table.AddCell(cell);
            //-------// Total Line
            para = new Paragraph(" ", FontFactory.GetFont("Arial", 8));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Colspan = 5;
            cell.Border = 0;
            //cell.BorderWidthTop = 1f;
            cell.BorderWidthBottom = 1f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph("Grand Total", FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            //cell.BorderWidthTop = 1f;
            cell.BorderWidthBottom = 1f;
            cell.AddElement(para);
            table.AddCell(cell);

            para = new Paragraph(":", FontFactory.GetFont("Arial", 8));
            para.Alignment = Element.ALIGN_CENTER;
            cell = new PdfPCell();
            cell.Border = 0;
            //cell.BorderWidthTop = 1f;
            cell.BorderWidthBottom = 1f;
            cell.AddElement(para);
            table.AddCell(cell);

            if (InVDet.SalesInvoiceDryModel.AmountinUSDSUM == 0)
            {
                DecTotal = "0.00";
            }
            else
            {
                double decTot = InVDet.SalesInvoiceDryModel.AmountinUSDSUM;
                DecTotal = Convert.ToDecimal(decTot).ToString("0.00");
            }
            para = new Paragraph("USD  "+DecTotal, FontFactory.GetFont("Arial", 8, Font.BOLD));
            para.Alignment = Element.ALIGN_LEFT;
            cell = new PdfPCell();
            cell.PaddingBottom = 5f;
            cell.Border = 0;
            //cell.BorderWidthTop = 1f;
            cell.BorderWidthBottom = 1f;
            cell.AddElement(para);
            table.AddCell(cell);

            pdfDoc.Add(table);

            table = new PdfPTable(2);
            table.SetTotalWidth(new float[] { 80, 50 });
            table.LockedWidth = true;
            table.SpacingBefore = 10f;
            para.SpacingAfter = 10f;
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

            if (InVDet.salesInvoiceLineItemDryModel.Count > 0)
            {
                if (InVDet.salesInvoiceLineItemDryModel[0].ContainerNo != null)
                {
                    String[] Containers = InVDet.salesInvoiceLineItemDryModel[0].ContainerNo.Split(',');

                    for (int i = 0; i < Containers.Count(); i++)
                    {
                        para = new Paragraph(Containers[i], FontFactory.GetFont("Arial", 8));
                        para.Alignment = Element.ALIGN_LEFT;
                        cell = new PdfPCell();
                        cell.AddElement(para);
                        table.AddCell(cell);

                        para = new Paragraph(InVDet.ShipmentDetailsModel.EquipmentType, FontFactory.GetFont("Arial", 8));
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
            para.SpacingBefore = 10f;
            para.SpacingAfter = 10f;
            pdfDoc.Add(para);

            para = new Paragraph("Terms and Conditions:", FontFactory.GetFont("Arial", 8, Font.BOLD));
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
            Response.AddHeader("content-disposition", "inline;filename=Import Invoice.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
        }

    }
}
