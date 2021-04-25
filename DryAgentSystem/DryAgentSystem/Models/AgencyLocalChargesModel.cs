using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DryAgentSystem.Models
{
    public class AgencyLocalChargesModel
    {
        public List<int> Ids { get; set; }
        public List<AgencyLocalCharges> LocalCharges { get; set; }
       
    }
}