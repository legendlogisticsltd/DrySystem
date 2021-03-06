using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DryAgentSystem.Models
{
    public class AllocateEquipment
    {
        [Display(Name = "Tank No")]
        public string TankNo { get; set; } 
        
        [Display(Name = "ID")]
        public string ID { get; set; }

        [Display(Name = "Seal No")]
        public string SealNo { get; set; }

        [Display(Name = "Gross Weight")]
        public string GrossWeight { get; set; }

        [Display(Name = "Net Weight")]
        public string NettWeight { get; set; }

        [Display(Name = "Universal Serial Nr")]
        public string UniversalSerialNr { get; set; }

        [Display(Name = "Measurement")]
        public string Measurement { get; set; }
    }
}