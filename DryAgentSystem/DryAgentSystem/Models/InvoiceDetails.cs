using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace DryAgentSystem.Models
{
    public class InvoiceDetails
    {
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }

        [Display(Name = "Acct Month")]
        public string AcctMonth { get; set; }

        [Display(Name = "Amount in USD Delete")]
        public string AmountinUSDDelete { get; set; }

        [Display(Name = "Amount in USD WTax")]
        public string AmountinUSDWTax { get; set; }

        [Display(Name = "Amount in USD WTax Delete")]
        public string AmountinUSDWTaxDelete { get; set; } 

        [Display(Name = "COA")]
        public string COA { get; set; }

        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Display(Name = "Company Address")]
        public string CompanyAddress{ get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Creation Time Stamp")]
        public DateTime CreationTimestamp { get; set; }

        [Display(Name = "Currency")]
        public string Currency { get; set; }

        [Display(Name = "Currency Cost Delete")]
        public string CurrencyCostDelete { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Ex Rate")]
        public int ExRate { get; set; }

        [Display(Name = "ID")]
        public string ID { get; set; }

        [Display(Name = "AmountUSD")]
        public string AmountUSD { get; set; }

        [Display(Name = "ID Company")]
        public string IDcompany { get; set; }

        [Display(Name = "ID Invoice Nr")]
        public string IDInvoiceNr { get; set; }

        [Display(Name = "ID Shipment Sales LI")]
        public string IDShipmentSalesLI { get; set; }

        [Display(Name = "Invoice No.")]
        public string InvoiceNo { get; set; }

        [Display(Name = "Grossweight")]
        public string Grossweight { get; set; }

        [Display(Name = "InvoiceDate")]
        public string InvoiceDate { get; set; }

        [Display(Name = "GrossweightUnit")]
        public string GrossweightUnit { get; set; }

        [Display(Name = "DueDate")]
        public string DueDate { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "InvoiceMonth")]
        public string InvoiceMonth { get; set; }

        [Display(Name = "Item Code")]
        public string ItemCode { get; set; }

        [Display(Name = "Label")]
        public string Label { get; set; }

        [Display(Name = "Label2")]
        public string Label2 { get; set; }

        [Display(Name = "Line Item Remarks")]
        public string LineItemRemarks { get; set; }

        [Display(Name = "Modification Timestamp")]
        public DateTime ModificationTimestamp { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "Ops Acct Month")]
        public string OpsAcctMonth { get; set; }

        [Display(Name = "Payment Term")]
        public string PaymentTerm { get; set; }

        [Display(Name = "Port")]
        public string Port { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Ref Invoice No.")]
        public string RefInvoiceNo { get; set; }

        [Display(Name = "SI Total Amount")]
        public string SITotalAmount { get; set; }

        [Display(Name = "Tank No.")]
        public string TankNo { get; set; }

        [Display(Name = "Tank No. 1")]
        public string TankNo1 { get; set; }

        [Display(Name = "Tax Amount")]
        public string TaxAmount { get; set; }

        [Display(Name = "Tax Percent")]
        public int TaxPercent { get; set; }

        [Display(Name = "Unit Rate")]
        public int UnitRate { get; set; }

        [Display(Name = "Unit Rate Cost Delete")]
        public int UnitRateCostDelete { get; set; }

        [Display(Name = "Unit Rate USD")]
        public string UnitRateUSD { get; set; }

        [Display(Name = "Universal Serial Nr")]
        public string UniversalSerialNr { get; set; }

        [Display(Name = "zgf Item code")]
        public string zgfItemcode { get; set; }

        [Display(Name = "zgf Sort Order")]
        public string zgfSortOrder { get; set; }

        
    }
}