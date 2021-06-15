using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DryAgentSystem.Models
{
    public class Invoice
    {
        [Display(Name = "Acct Month")]
        public string AcctMonth { get; set; }

        [Display(Name = "Address1")]
        public string Address1 { get; set; }

        [Display(Name = "Address2")]
        public string Address2 { get; set; }

        [Display(Name = "Address3")]
        public string Address3 { get; set; }

        [Display(Name = "Address4")]
        public string Address4 { get; set; }

        [Display(Name = "Amount in USD SUM")]
        public double AmountinUSDSUM { get; set; }

        [Display(Name = "Amount USD SUM WTax")]
        public double AmountinUSDSUMWTax { get; set; }

        [Display(Name = "Billing Party Address")]
        public string BillingPartyAddress { get; set; }

        [Display(Name = "Billing Party")]
        public string BillingParty { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Amount in words")]
        public string Amountinwords { get; set; }

        [Display(Name = "Billing Items")]
        public string BillingItems { get; set; }

        [Display(Name = "Cancel Reason")]
        public string CancelReason { get; set; }

        [Display(Name = "checklist")]
        public string checklist { get; set; }

        [Display(Name = "Creation Time stamp")]
        public DateTime CreationTimestamp { get; set; }

        [Display(Name = "Credit Terms")]
        public int CreditTerms { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Dest Code")]
        public string DestCode { get; set; }

        [Display(Name = "Dest Name")]
        public string DestName { get; set; }

        [Display(Name = "Discharge Port")]
        public string DischargePort { get; set; }

        [Display(Name = "Discharge Code")]
        public string DischargeCode { get; set; }

        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        [Display(Name = "ETA")]
        public DateTime ETA { get; set; }

        [Display(Name = "ETD")]
        public DateTime ETD { get; set; }

        [Display(Name = "Gross Weight")]
        public double Grossweight { get; set; }

        [Display(Name = "Gross Weight Unit")]
        public string GrossweightUnit { get; set; }

        [Display(Name = "HBLHAWB")]
        public string HBLHAWB { get; set; }

        [Display(Name = "ID")]
        public string ID { get; set; }

        [Display(Name = "IDCompany")]
        public string IDCompany { get; set; }

        [Display(Name = "Id shipment sales Li")]
        public string IdshipmentsalesLi { get; set; }

        [Display(Name = "Invoice No.")]
        public string InvoiceNo { get; set; }
        public string ProformaInvoiceNo { get; set; }

        [Display(Name = "Invoice Amt")]
        public string InvoiceAmt { get; set; }

        [Display(Name = "Invoice Date")]
        public DateTime InvoiceDate { get; set; }

        [Display(Name = "Invoice Month")]
        public int InvoiceMonth { get; set; }

        [Display(Name = "Invoice No. Test")]
        public string InvoiceNoTest { get; set; }

        [Display(Name = "Invoice Prefix")]
        public string InvoicePrefix { get; set; }

        [Display(Name = "Job Ref No.")]
        public string JobRefNo { get; set; }

        [Display(Name = "Invoice Type")]
        public string InvoiceType { get; set; }

        [Display(Name = "l1")]
        public string l1 { get; set; }

        [Display(Name = "Label")]
        public string Label { get; set; }

        [Display(Name = "Load Port")]
        public string LoadPort { get; set; }

        [Display(Name = "Load port Code")]
        public string LoadportCode { get; set; }

        [Display(Name = "LOG")]
        public string LOG { get; set; }

        [Display(Name = "MBLHAWB")]
        public string MBLHAWB { get; set; }

        [Display(Name = "Modification Timestamp")]
        public string ModificationTimestamp { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Ops Acct Month")]
        public string OpsAcctMonth { get; set; }

        [Display(Name = "Origin Code")]
        public string OriginCode { get; set; }

        [Display(Name = "Origin Name")]
        public string OriginName { get; set; }

        [Display(Name = "Payment Term")]
        public string PaymentTerm { get; set; }

        [Display(Name = "Port")]
        public string Port { get; set; }

        [Display(Name = "Post Date")]
        public DateTime PostDate { get; set; }

        [Display(Name = "Ref Invoice No.")]
        public string RefInvoiceNo { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }


        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Tank No.")]
        public string TankNo{ get; set; }

        [Display(Name = "Tax Amount Sum")]
        public double TaxAmountSum { get; set; }

        [Display(Name = "Type")]
        public string Type { get; set; }

        [Display(Name = "Universal Serial Nr")]
        public string UniversalSerialNr { get; set; }

      

        [Display(Name = "Vessel Name")]
        public string VesselName { get; set; }

        [Display(Name = "Voyage No.")]
        public string VoyageNo { get; set; }

        [Display(Name = "zgf add line item")]
        public DateTime zgfaddlineitem { get; set; }

        [Display(Name = "zgf Company  Name")]
        public string zgfCompanyName { get; set; }

        [Display(Name = "zgf HBL")]
        public string zgfHBL { get; set; }

        [Display(Name = "zgf Invoice No.")]
        public string zgfInvoiceNo { get; set; }

        [Display(Name = "zgf itemc")]
        public string zgfitemc { get; set; }

        [Display(Name = "zgf Job Ref No.")]
        public string zgfJobRefNo { get; set; }

        [Display(Name = "zgf MBL")]
        public string zgfMBL { get; set; }

        [Display(Name = "Container No")]
        public String ContainerNo { get; set; }


        public List<InvoiceDetails> InvoiceDetails { get; set; }

        public InvoiceDetails InvoiceDetailsModel { get; set; }
        public Invoice()
        {
           
            InvoiceDetailsModel = new InvoiceDetails();
        }

        
    }

}
