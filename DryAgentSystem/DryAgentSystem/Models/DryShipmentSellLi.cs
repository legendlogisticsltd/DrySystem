using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DryAgentSystem.Models
{
    public class DryShipmentSellLi
    {
        public int AmountUSD { get; set; }
        public string CompanyName { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string ExRate { get; set; }
        public string ID { get; set; }
        public string IDCompany { get; set; }
        public string PayMode { get; set; }
        public int Quantity { get; set; }
        public string UnitRate { get; set; }
        public int UnitRateUSD { get; set; }
        public string UniversalSerialNr { get; set; }
        public string User { get; set; }
        public DateTime UserCreateDate { get; set; }
        public string UserModify { get; set; }
        public DateTime UserModifyDate { get; set; }
        public string InvoiceNo { get; set; }
        public string CompanyAddress { get; set; }
        public string PaymentTerm { get; set; }

    }
}