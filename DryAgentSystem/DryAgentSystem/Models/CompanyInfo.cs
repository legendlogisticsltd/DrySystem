using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DryAgentSystem.Models
{
    public class CompanyInfo
    {
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }
    }
}