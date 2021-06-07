using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DryAgentSystem.Models
{
    public class Vessel
    {
        public string ID { get; set; }
        public string BookingStatus { get; set; }
        
        [Display(Name = "Carrier Booking Ref No")]
        public string CarrierBookingRefNo { get; set; }

        [Display(Name = "Carrier")]
        public string Carrier { get; set; }
        public string UniversalSerialNr { get; set; }

        [Display(Name = "Vessel Name")]
        [Required(ErrorMessage = "Please provide Vessel Name")]
        public string VesselName { get; set; }

        [Display(Name = "Voy No.")]
        public string VoyNo { get; set; }

        [Display(Name = "Load Port")]
        [Required(ErrorMessage = "Please provide Load Port")]
        public string LoadPort { get; set; }

        [Display(Name = "Discharge Port")]
        [Required(ErrorMessage = "Please provide Discharge Port")]
        public string DischPort { get; set; }

        [Display(Name = "ETD")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}")]
        public DateTime ETD { get; set; }

        [Display(Name = "ETA")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}")]
        public DateTime ETA { get; set; }

        [Display(Name = "Mode of Transport")]
        [Required(ErrorMessage = "Please provide Mode of Transport")]
        public string TransportMode { get; set; }


        //[Display(Name = "Date SOB")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}")]
        //public DateTime? DateSOB { get; set; }

        //[Display(Name = "Date ATA")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}")]
        //public DateTime? DateATA { get; set; }
    }
}