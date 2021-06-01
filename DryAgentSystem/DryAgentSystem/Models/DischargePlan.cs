using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DryAgentSystem.Models
{
    public class DischargePlan
    {
        [Display(Name = "Discharge Plan ID")]
        public int IDNo { get; set; }

        [Display(Name = "Closing Date")]
        public DateTime ClosingDate { get; set; }

        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "Create Date")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Disch Agent Address")]
        public string DischAgentAddress { get; set; }

        [Display(Name = "Disch FT")]
        public int DischFT { get; set; }

        [Display(Name = "Disch Port")]
        public string DischPort { get; set; }

        [Display(Name = "ETA")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ETA { get; set; }

        [Display(Name = "Load Agent Address")]
        public string LoadAgentAddress { get; set; }

        [Display(Name = "ETD")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ETD { get; set; }

        [Display(Name = "ATA")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ATA { get; set; }

        [Display(Name = "ID Agent Disch")]
        public string IDAgentDisch { get; set; }

        [Display(Name = "ID Agent Load")]
        public string IDAgentLoad { get; set; }

        [Display(Name = "Job Ref")]
        public string JobRef { get; set; }

        [Display(Name = "Load FT")]
        public int LoadFT { get; set; }

        [Display(Name = "Load Port")]
        public string LoadPort { get; set; }

        [Display(Name = "Modify Date")]
        public DateTime ModifyDate { get; set; }

        [Display(Name = "Place Of Delivery")]
        public string PlaceOfDelivery { get; set; }

        [Display(Name = "Place Of Receipt")]
        public string PlaceOfReceipt { get; set; }

        [Display(Name = "Port Pair")]
        public string PortPair { get; set; }

        [Display(Name = "Disch Agent Name BL")]
        public string DischAgentNameBL { get; set; }

        [Display(Name = "Load Agent")]
        public string LoadAgent { get; set; }

        [Display(Name = "Shipper Name BL")]
        public string ShipperNameBL { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Quantity Lifting")]
        public int QuantityLifting { get; set; }

        [Display(Name = "Quote Type")]
        public string QuoteType { get; set; }

        [Display(Name = "Universal Serial Nr")]
        public string UniversalSerialNr { get; set; }

        [Display(Name = "User")]
        public string User { get; set; }

        [Display(Name = "Status")]
        public string DischargePlanStatus { get; set; }

        [Display(Name = "Vessel Name")]
        public string VesselName { get; set; }
        public string Voyage { get; set; }

        [Display(Name = "Invoice Status")]
        public string Status { get; set; }

        [Display(Name = "To")]
        public string DishTo { get; set; }

        [Display(Name = "Attention")]
        public string DishAttn { get; set; }
    }
}