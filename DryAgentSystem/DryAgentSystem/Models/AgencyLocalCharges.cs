using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DryAgentSystem.Models
{
    public class AgencyLocalCharges
    {

        public string ID { get; set; }

        [Display(Name = "Import Or Export")]
        public string ImportOrExport { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Charge Description")]
        public string ChargeDescription { get; set; }

        [Display(Name = "Cost")]
        public string Cost { get; set; }

        [Display(Name = "Currency")]
        public string Currency { get; set; }

        [Display(Name = "Equipment Type")]
        public string EquipmentType { get; set; }

    }
}