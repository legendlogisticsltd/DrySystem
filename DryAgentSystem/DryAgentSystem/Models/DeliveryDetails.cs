using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DryAgentSystem.Models
{
    public class DeliveryDetails
    {
        public List<DevAllocateEquipment> DevAllocateEquipmentModel { get; set; }
        public DischargePlan DischargePlanModel { get; set; }
        public ShipmentDetails ShipmentDetailsModel { get; set; }
        public BLDetails BLDetailsModel { get; set; }

        public QuoteRef QuoteRefModel { get; set; }
        public DeliveryDetails()
        {
            DevAllocateEquipmentModel = new List<DevAllocateEquipment>();
            DischargePlanModel = new DischargePlan();
            ShipmentDetailsModel = new ShipmentDetails();
            BLDetailsModel = new BLDetails();
            QuoteRefModel = new QuoteRef();
        }
    }
}