using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DryAgentSystem.Models
{
    public class SalesInvoiceLineItemDry
    {
        public string AccountCode { get; set; }
        public string CompanyName { get; set; }
        public string ContainerNo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationTimestamp { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public double ExRate { get; set; }
        public string ID { get; set; }
        public string IDcompany { get; set; }
        public string IDInvoiceNr { get; set; }
        public string IDShipmentSalesLI { get; set; }
        public string InvoiceNo { get; set; }
        public string ItemCode { get; set; }
        public DateTime ModificationTimestamp { get; set; }
        public string ModifiedBy { get; set; }
        public string PaymentTerm { get; set; }
        public string Port { get; set; }
        public int Quantity { get; set; }
        public int TaxPercent { get; set; }
        public double UnitRate { get; set; }
        public string UniversalSerialNr { get; set; }
        public double AmountUSD { get; set; }
        public double TaxAmount { get; set; }
    }
}