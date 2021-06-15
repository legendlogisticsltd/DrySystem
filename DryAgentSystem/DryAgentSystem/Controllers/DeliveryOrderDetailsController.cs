using DryAgentSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DryAgentSystem.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace DryAgentSystem.Controllers
{
    public class DeliveryOrderDetailsController : Controller
    {
        // GET: DeliveryOrderDetails
        [HttpGet]
        [Authorize]
        public ActionResult DeliveryOrderDetails(string JobRef)
        {
            var dischargePlan = DataContext.GetDischargePlanFromIDNo(JobRef);
            TempData["UniversalSerialNr"] = dischargePlan.UniversalSerialNr;
            return View(dischargePlan);
        }

        //[HttpGet]
        //[Authorize]
        public JsonResult GetDeliveryContainers(string sidx, string sord, int page, int rows)
        {
            string universalSerialNr = string.Empty;
            if (TempData["UniversalSerialNr"] != null)
            {
                universalSerialNr = TempData["UniversalSerialNr"].ToString();
                //SearchParameters filter = new SearchParameters();

                //if (TempData["filterobj"] != null)
                //{
                //    filter = (SearchParameters)TempData["filterobj"];
                //}
                var movementData = DataContext.FetchDeliveryContainenrsFromJobRef(universalSerialNr);
                int totalRecords = movementData.Count();
                var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);

                var jsonData = new
                {
                    total = totalPages,
                    page,
                    records = totalRecords,
                    rows = (from movementDataGrid in movementData
                        select new
                        {
                            cell = new string[]
                            {
                                movementDataGrid.ContainerNo,
                                movementDataGrid.SealNo,
                                movementDataGrid.DONumber,
                                movementDataGrid.DOIssueDate.ToString("MM-dd-yyyy") == "01-01-0001" ? null : movementDataGrid.DOIssueDate.ToString("MM-dd-yyyy"),
                                movementDataGrid.VesselName,
                                movementDataGrid.ATA.ToString("MM-dd-yyyy"),
                                movementDataGrid.ETA.ToString("MM-dd-yyyy"),
                                movementDataGrid.JobRefSerialNo,
                                movementDataGrid.UniversalSerialNr
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
        //public void UpdateRecords(List<DevAllocateEquipment> containerNo, string JobRef)
        //{
        //    if (containerNo != null)
        //    {
        //        ErrorLog errorlog = DataContext.UpdateContainersDetails(containerNo, JobRef);
        //        string universalSerialNr = string.Empty;
        //        if (!errorlog.IsError)
        //        {
        //            universalSerialNr = errorlog.ErrorMessage;
        //            CreateDeliveryOrder(universalSerialNr);
        //        }
        //    }
        //    //else
        //    //{
        //    //    TempData["Message"] = "Please select container";
        //    //}
        //    //return RedirectToAction("DeliveryOrderDetails", "DeliveryOrderDetails", new { JobRef = JobRef });
        //}

        public ActionResult UpdateRecords(List<DevAllocateEquipment> containerNo, string JobRef, string DishTo, string DishAttn)
        {
            object doData = new object();
            if (containerNo != null)
            {
                doData = DataContext.UpdateContainersDetails(containerNo, JobRef, DishTo, DishAttn);
                TempData["DoJsonData"] = doData;
                return Json(doData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                TempData["Message"] = "Please select container(s)";
                Response.StatusCode = 400;
                return Json(doData, JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpGet]
        public void CreateDeliveryOrder()
        {
            object doJsondata = new object();
            if (TempData["DoJsonData"] != null)
            {
                doJsondata = TempData["DoJsonData"];
                string json = JsonConvert.SerializeObject(doJsondata);

                DOJsonData model = new JavaScriptSerializer().Deserialize<DOJsonData>(json);

                var username = HttpContext.User.Identity.Name;
                DeliveryDetails DeliveryDet = DataContext.GetDeliveryOrderDetailsFromJobRef(model.UniversalSerialNr, model.DONumber);

                Document pdfDoc = new Document(PageSize.A4, 40, 40, 20, 20);
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                pdfDoc.AddTitle("DELIVERY ORDER");
                pdfDoc.Open();

                Chunk chunk;
                Paragraph para, para1;
                PdfPTable table;
                PdfPCell cell;

                #region Header Name
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

                para = new Paragraph("EXCHANGE D/O WITHOUT PERMIT", FontFactory.GetFont("Arial", 13, Font.BOLD));
                para.Alignment = Element.ALIGN_CENTER;
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 6;
                //cell.BackgroundColor = new BaseColor(211, 211, 211);
                cell.PaddingBottom = 10f;
                cell.AddElement(para);
                table.AddCell(cell);
                pdfDoc.Add(table);
                #endregion

                #region Table Content

                table = new PdfPTable(4);
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 1;
                table.DefaultCell.PaddingLeft = 5f;

                para1 = new Paragraph(" ", FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 4;
                cell.AddElement(para1);
                table.AddCell(cell);

                para1 = new Paragraph("DATE", FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para1);
                table.AddCell(cell);

                if (DeliveryDet.DevAllocateEquipmentModel.Count > 0)
                {
                    para1 = new Paragraph(":    " + DeliveryDet.DevAllocateEquipmentModel[0].DOIssueDate.ToString("dd-MM-yyyy"), FontFactory.GetFont("Arial", 8));
                }
                else
                {
                    para1 = new Paragraph(":    " + DateTime.Now.ToString("dd-MM-yyyy"), FontFactory.GetFont("Arial", 8));
                }
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 3;
                cell.AddElement(para1);
                table.AddCell(cell);

                para1 = new Paragraph("TO", FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para1);
                table.AddCell(cell);

                para1 = new Paragraph(":    " + (string.IsNullOrEmpty(DeliveryDet.DischargePlanModel.DishTo) ? string.Empty : DeliveryDet.DischargePlanModel.DishTo.ToUpperInvariant()), FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 3;
                cell.AddElement(para1);
                table.AddCell(cell);

                para1 = new Paragraph("ATTN", FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para1);
                table.AddCell(cell);

                para1 = new Paragraph(":    " + (string.IsNullOrEmpty(DeliveryDet.DischargePlanModel.DishAttn) ? string.Empty : DeliveryDet.DischargePlanModel.DishAttn.ToUpperInvariant()), FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 3;
                cell.AddElement(para1);
                table.AddCell(cell);

                para1 = new Paragraph(" ", FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 4;
                cell.AddElement(para1);
                table.AddCell(cell);

                para1 = new Paragraph("DEAR SIRS,", FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 4;
                cell.AddElement(para1);
                table.AddCell(cell);

                para1 = new Paragraph(" ", FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 4;
                cell.AddElement(para1);
                table.AddCell(cell);

                para1 = new Paragraph("JOB NO", FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para1);
                table.AddCell(cell);

                para1 = new Paragraph(":    " + DeliveryDet.DischargePlanModel.JobRef, FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 3;
                cell.AddElement(para1);
                table.AddCell(cell);

                para1 = new Paragraph("OB/L NO.", FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para1);
                table.AddCell(cell);

                para1 = new Paragraph(":    " + DeliveryDet.ShipmentDetailsModel.MBLMAWB, FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 3;
                cell.AddElement(para1);
                table.AddCell(cell);

                para1 = new Paragraph("VESSEL", FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para1);
                table.AddCell(cell);

                para1 = new Paragraph(":    " + DeliveryDet.DischargePlanModel.VesselName, FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 3;
                cell.AddElement(para1);
                table.AddCell(cell);

                para1 = new Paragraph("VOYAGE", FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para1);
                table.AddCell(cell);

                para1 = new Paragraph(":    " + DeliveryDet.DischargePlanModel.Voyage, FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 3;
                cell.AddElement(para1);
                table.AddCell(cell);

                para1 = new Paragraph("PORT OF LOADING", FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para1);
                table.AddCell(cell);

                para1 = new Paragraph(":    " + (string.IsNullOrEmpty(DeliveryDet.DischargePlanModel.LoadPort) ? string.Empty : DeliveryDet.DischargePlanModel.LoadPort.ToUpperInvariant()), FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 3;
                cell.AddElement(para1);
                table.AddCell(cell);

                para1 = new Paragraph("ETA", FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para1);
                table.AddCell(cell);

                para1 = new Paragraph(":    " + DeliveryDet.DischargePlanModel.ETA.ToString("dd-MM-yyyy"), FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 3;
                cell.AddElement(para1);
                table.AddCell(cell);

                para1 = new Paragraph("ATA", FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para1);
                table.AddCell(cell);

                para1 = new Paragraph(":    " + DeliveryDet.DischargePlanModel.ATA.ToString("dd-MM-yyyy"), FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 3;
                cell.AddElement(para1);
                table.AddCell(cell);

                para1 = new Paragraph("PKG", FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para1);
                table.AddCell(cell);

                para1 = new Paragraph(":    " + DeliveryDet.BLDetailsModel.NoOfPkgs, FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 3;
                cell.AddElement(para1);
                table.AddCell(cell);

                para1 = new Paragraph("VOL", FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para1);
                table.AddCell(cell);
                if (DeliveryDet.DevAllocateEquipmentModel.Count > 0)
                {
                    double decVOL = DeliveryDet.DevAllocateEquipmentModel[0].Measurement;
                    string DecVol = Convert.ToDecimal(decVOL).ToString("#.00");
                    if (decVOL == 0)
                    {
                        para1 = new Paragraph(":    " + "0.00" + " " + DeliveryDet.DevAllocateEquipmentModel[0].MeasurementUnit, FontFactory.GetFont("Arial", 8));
                    }
                    else
                    {
                        para1 = new Paragraph(":    " + DecVol + " " + DeliveryDet.DevAllocateEquipmentModel[0].MeasurementUnit, FontFactory.GetFont("Arial", 8));
                    }
                }
                else
                {
                    para1 = new Paragraph(":    " + "0.00", FontFactory.GetFont("Arial", 8));
                }
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 3;
                cell.AddElement(para1);
                table.AddCell(cell);

                para1 = new Paragraph("WT", FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para1);
                table.AddCell(cell);
                if (DeliveryDet.DevAllocateEquipmentModel.Count > 0)
                {
                    double decGW = DeliveryDet.DevAllocateEquipmentModel[0].GrossWeight;
                    string DecGrossWeight = Convert.ToDecimal(decGW).ToString("#.00");
                    if (decGW == 0)
                    {
                        para1 = new Paragraph(":    " + "0.00" + " " + DeliveryDet.DevAllocateEquipmentModel[0].GrossWeightUnit, FontFactory.GetFont("Arial", 8));
                    }
                    else
                    {
                        para1 = new Paragraph(":    " + DecGrossWeight + " " + DeliveryDet.DevAllocateEquipmentModel[0].GrossWeightUnit, FontFactory.GetFont("Arial", 8));
                    }
                }
                else
                {
                    para1 = new Paragraph(":    " + "0.00", FontFactory.GetFont("Arial", 8));

                }
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 3;
                cell.AddElement(para1);
                table.AddCell(cell);

                para1 = new Paragraph("CONTAINER /SEAL NO.", FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.AddElement(para1);
                table.AddCell(cell);

                string ParaContainer = "";
                if (DeliveryDet.DevAllocateEquipmentModel.Count > 0)
                {
                    for (int i = 0; i < DeliveryDet.DevAllocateEquipmentModel.Count; i++)
                    {
                        if (ParaContainer == "")
                        {
                            ParaContainer = ":    " + DeliveryDet.DevAllocateEquipmentModel[i].ContainerNo + "/" + (string.IsNullOrEmpty(DeliveryDet.DevAllocateEquipmentModel[i].SealNo) ? string.Empty : DeliveryDet.DevAllocateEquipmentModel[i].SealNo.ToUpperInvariant()) + "/" + DeliveryDet.QuoteRefModel.EquipmentType;
                        }
                        else
                        {
                            ParaContainer = ParaContainer + "\n" + "     " + DeliveryDet.DevAllocateEquipmentModel[i].ContainerNo + "/" + (string.IsNullOrEmpty(DeliveryDet.DevAllocateEquipmentModel[i].SealNo) ? string.Empty : DeliveryDet.DevAllocateEquipmentModel[i].SealNo.ToUpperInvariant()) + "/" + DeliveryDet.QuoteRefModel.EquipmentType;
                        }
                    }
                }

                para1 = new Paragraph(ParaContainer, FontFactory.GetFont("Arial", 8));
                //para1 = new Paragraph(": " + DeliveryDet.DevAllocateEquipmentModel[i].ContainerNo + "//" + DeliveryDet.DevAllocateEquipmentModel[i].SealNo + "\n", FontFactory.GetFont("Arial", 8));
                cell = new PdfPCell();
                cell.Border = 0;
                cell.Colspan = 3;
                cell.AddElement(para1);
                table.AddCell(cell);

                pdfDoc.Add(table);

                para = new Paragraph(" \n ", FontFactory.GetFont("Arial", 8));
                pdfDoc.Add(para);

                para = new Paragraph(" \n ", FontFactory.GetFont("Arial", 8));
                pdfDoc.Add(para);

                #endregion

                #region Content1
                para = new Paragraph("WE WOULD VERY MUCH APPRECIATE IF YOU COULD RELEASE THE DELIVERY ORDER TO US FOR THE ABOVE - MENTIONED VESSEL. \n ", FontFactory.GetFont("Arial", 8));

                Paragraph paraT = new Paragraph("WE WILL SUBMIT THE IMPORT DECLARATION PERMIT TO YOU AS SOON AS IT IS AVAILABLE. \n ", FontFactory.GetFont("Arial", 8));

                Paragraph paraT1 = new Paragraph("IN CONSIDERATION OF YOU ISSUING TO US THE DELIVERY ORDER(S) WITHOUT PRESENTATION OF IMPORT DECLARATION PERMIT, WE HEREBY INDEMNIFY YOU AGAINST ANY CLAIMS THEREFORE. \n ", FontFactory.GetFont("Arial", 8));

                Paragraph paraT2 = new Paragraph("THANK YOU FOR YOUR KIND ASSISTANCE AND CO-OPERATION. \n ", FontFactory.GetFont("Arial", 8));

                Paragraph paraT3 = new Paragraph("THANK YOU \n ", FontFactory.GetFont("Arial", 8));

                Paragraph paraT4 = new Paragraph("LEGEND LOGISTICS (ASIA) PTE LTD \n ", FontFactory.GetFont("Arial", 8));

                pdfDoc.Add(para);
                pdfDoc.Add(paraT);
                pdfDoc.Add(paraT1);
                pdfDoc.Add(paraT2);
                pdfDoc.Add(paraT3);
                pdfDoc.Add(paraT4);
                #endregion

                #region Content2
                para = new Paragraph("PREPARED BY : " + username, FontFactory.GetFont("Arial", 8, Font.BOLD));

                Paragraph paraK1 = new Paragraph("SEA IMPORT DEPARTMENT \n ", FontFactory.GetFont("Arial", 8));

                Paragraph paraK2 = new Paragraph("DOCUMENT IS COMPUTER GENERATED AND DOES NOT REQUIRE SIGNATURE" + " \n", FontFactory.GetFont("Arial", 8));
                paraK2.SpacingAfter = 10f;
                paraK2.Alignment = Element.ALIGN_CENTER;
                Paragraph paraK3 = new Paragraph("ALL BUSINESS TRANSACTED ARE SUBJECTED TO OUR COMPANY STANDARD TRADING CONDITIONS.", FontFactory.GetFont("Arial", 7));
                paraK3.Alignment = Element.ALIGN_CENTER;
                Paragraph paraK4 = new Paragraph("A COPY IS AVAILABLE UPON REQUEST.", FontFactory.GetFont("Arial", 7));
                paraK4.Alignment = Element.ALIGN_CENTER;
                pdfDoc.Add(para);
                pdfDoc.Add(paraK1);
                pdfDoc.Add(paraK2);
                paraK2.Alignment = Element.ALIGN_CENTER;
                pdfDoc.Add(paraK3);
                paraK3.Alignment = Element.ALIGN_CENTER;
                pdfDoc.Add(paraK4);
                paraK4.Alignment = Element.ALIGN_CENTER;

                #endregion

                pdfWriter.CloseStream = false;
                pdfDoc.Close();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "inline;filename=DO without permit.pdf");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc);
                Response.End();
            }
        }
    }
}