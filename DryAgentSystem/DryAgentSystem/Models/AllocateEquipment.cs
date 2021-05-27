using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DryAgentSystem.Models
{
    public class AllocateEquipment
    {
        [Display(Name = "Container No")]
        public string ContainerNo { get; set; }

        [Display(Name = "ID")]
        public string ID { get; set; }

        [Display(Name = "Seal No")]
        public string SealNo { get; set; }

        [Display(Name = "Gross Weight")]
        public string GrossWeight { get; set; }
        public string GrossWeightUnit { get; set; }

        [Display(Name = "Net Weight")]
        public string NettWeight { get; set; }
        public string NetWeightUnit { get; set; }

        [Display(Name = "Universal Serial Nr")]
        public string UniversalSerialNr { get; set; }

        [Display(Name = "Measurement")]
        public string Measurement { get; set; }
        public string MeasurementUnit { get; set; }

        public IEnumerable<SelectListItem> ContainerList { get; set; }
    }
}