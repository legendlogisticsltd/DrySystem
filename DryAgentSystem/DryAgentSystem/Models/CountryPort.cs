using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DryAgentSystem.Models
{
    public class CountryPort
    {
        [Display(Name = "Port")]
        public string Port { get; set; }

        [Display(Name = "Port Code")]
        public string PortCode { get; set; }

        [Display(Name = "Country")]
        public string Country { get; set; }
        
        [Display(Name = "PortCountry")]
        public string PortCountry { get; set; }

    }
}