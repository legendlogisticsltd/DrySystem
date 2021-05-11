using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DryAgentSystem.Models
{
    public class SearchParameters
    {
        [Display(Name = "Quote Ref ID")]
        public string QuoteRefID { get; set; }

        [Display(Name = "Rate ID")]
        public string RateID { get; set; } 
        
        [Display(Name = "Booking ID")]
        public string BookingID { get; set; }

        [Display(Name = "Booking No")]
        public string BookingNo { get; set; }

        [Display(Name = "Agency Name")]
        public string CompanyName { get; set; }

        [Display(Name = "Load Port")]
        public string LoadPort { get; set; }

        [Display(Name = "Discharge Port")]
        public string DischargePort { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Accounting Month")]
        public string AcctMonth { get; set; }
        
        [Display(Name = "Job Reference")]
        public string JobRef { get; set; }

        [Display(Name = "Charge Party")]
        public string ChargeParty { get; set; }

        [Display(Name = "Invoice No.")]
        public string InvoiceNo { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Quantity")]
        public string Quantity { get; set; }

        [Display(Name = "Currency")]
        public string Currency { get; set; }

        [Display(Name = "UnitRate")]
        public string UnitRate { get; set; }

        [Display(Name = "AmountUSD")]
        public string AmountUSD { get; set; }

        [Display(Name = "ExRate")]
        public string ExRate { get; set; }



    }
}