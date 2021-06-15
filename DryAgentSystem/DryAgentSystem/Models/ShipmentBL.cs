﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DryAgentSystem.Models
{
    public class ShipmentBL
    {
        public ShipmentDetails ShipmentDetailsModel { get; set; }
        public BLDetails BLDetailsModel { get; set; }
        
        public List<DevAllocateEquipment> DevAllocateEquipmentModel { get; set; }
        public Booking BookingModel { get; set; }
        public QuoteRef QuoteRefModel { get; set; }
        public List<DryShipmentSellLi> DryShipmentSellLiModel { get; set; }
        public List<Vessel> TransVessel { get; set; }

        public ShipmentBL()
        {
            ShipmentDetailsModel = new ShipmentDetails();
            BLDetailsModel = new BLDetails();
            
            DevAllocateEquipmentModel = new List<DevAllocateEquipment>();
            BookingModel = new Booking();
            QuoteRefModel = new QuoteRef();
            DryShipmentSellLiModel = new List<DryShipmentSellLi>();
            TransVessel = new List<Vessel>();
        }

    }
}