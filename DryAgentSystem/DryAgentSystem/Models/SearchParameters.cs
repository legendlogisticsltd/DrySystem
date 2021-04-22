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
        public string pzChargeParty { get; set; }

    }
}