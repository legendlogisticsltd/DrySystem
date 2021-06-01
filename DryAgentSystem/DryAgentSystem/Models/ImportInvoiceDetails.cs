using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DryAgentSystem.Models
{
    public class ImportInvoiceDetails
    {
        public SalesInvoiceDry SalesInvoiceDryModel { get; set; }
        // public SalesInvoiceLineItemDry salesInvoiceLineItemDryModel { get; set; }
        public List<SalesInvoiceLineItemDry> salesInvoiceLineItemDryModel { get; set; }

        //public BLDetails BLDetailsModel { get; set; }
        public ShipmentDetails ShipmentDetailsModel { get; set; }
        public ImportInvoiceDetails()
        {
            SalesInvoiceDryModel = new SalesInvoiceDry();
            salesInvoiceLineItemDryModel = new List<SalesInvoiceLineItemDry>();
            //BLDetailsModel = new BLDetails();
            ShipmentDetailsModel = new ShipmentDetails();
        }
    }
}