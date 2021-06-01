using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DryAgentSystem.Models
{
        public class SalesInvoiceDry
        {
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string Address3 { get; set; }
            public string Address4 { get; set; }
            public double AmountinUSDSUM { get; set; }
            public double AmountinUSDSUMWTax { get; set; }
            // public string Amountinwords { get; set; }
            public int BillingItems { get; set; }
            public string BillingPartyAddress { get; set; }
            public string BillingParty { get; set; }
            public string CreatedBy { get; set; }
            public string CreationTimestamp { get; set; }
            public int CreditTerms { get; set; }
            public string Description { get; set; }
            public string DestCode { get; set; }
            public string DestName { get; set; }
            public string DischargePort { get; set; }
            public string DischportCode { get; set; }
            public DateTime DueDate { get; set; }
            public DateTime ETA { get; set; }
            public DateTime ETD { get; set; }
            public int Grossweight { get; set; }
            public string GrossweightMeasurement { get; set; }
            public string HBLHAWB { get; set; }
            public string ID { get; set; }
            public string IDCompany { get; set; }
            public string IdshipmentsalesLi { get; set; }
            public int InvoiceAmt { get; set; }
            public DateTime InvoiceDate { get; set; }
            public string InvoiceNo { get; set; }
            public string InvoiceType { get; set; }
            public string JobRefNo { get; set; }
            public string LoadPort { get; set; }
            public string LoadportCode { get; set; }
            public string MBLHAWB { get; set; }
            public DateTime ModificationTimestamp { get; set; }
            public string ModifiedBy { get; set; }
            public string OriginCode { get; set; }
            public string OriginName { get; set; }
            public DateTime PostDate { get; set; }
            public string RefInvoiceNo { get; set; }
            public string Remarks { get; set; }
            public string Status { get; set; }
            public string ContainerNo { get; set; }
            public double TaxAmountSum { get; set; }
            public string Type { get; set; }
            public string UniversalSerialNr { get; set; }
            public string VesselName { get; set; }
            public string VoyageNo { get; set; }
            public string AmountInWords { get; set; }
        }
    }