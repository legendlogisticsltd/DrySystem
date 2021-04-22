using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DryAgentSystem.Models
{
    public class ShipmentBL
    {
        public ShipmentDetails ShipmentDetailsModel { get; set; }
        public BLDetails BLDetailsModel { get; set; }
        public Vessel VesselModel { get; set; }

        public ShipmentBL()
        {
            ShipmentDetailsModel = new ShipmentDetails();
            BLDetailsModel = new BLDetails();
            VesselModel = new Vessel();
        }

    }
}