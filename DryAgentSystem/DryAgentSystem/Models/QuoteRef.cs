using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DryAgentSystem.Models
{
    public class QuoteRef
    {
        [Display(Name = "Agent Name")]
        public string AgentName { get; set; }

        [Display(Name = "Company ID")]
        public string IDCompany { get; set; }

        [Display(Name = "Agency Name")]
        //[Required(ErrorMessage = "Please provide Company Name")]
        public string CompanyName { get; set; }

        [Display(Name = "Shipper Name")]
        //[Required(ErrorMessage = "Please provide Shipper Name")]
        public string ShipperName3 { get; set; }

        [Display(Name = "Gross Weight")]
        //[Range(0.00001, int.MaxValue, ErrorMessage = "The field {0} must be greater than 0")]
        public decimal GrossWt { get; set; }

        [Display(Name = "Gross Weight Unit")]
        public string GrossWtUnit { get; set; }

        [Display(Name = "Agency Type")]
        //[Required(ErrorMessage = "Please provide Agency Type")]
        public string AgencyType { get; set; }

        [Display(Name = "Rate Type")]
        //[Required(ErrorMessage = "Please provide Rate Type")]
        public string RateType { get; set; }

        [Display(Name = "Humidity")]
        //[Required(ErrorMessage = "Please provide Humidity")]
        public string Humidity { get; set; }

        [Display(Name = "Temperature")]
        //[Required(ErrorMessage = "Please provide Temperature")]
        public string Temperature { get; set; }

        [Display(Name = "Ventilation")]
        //[Required(ErrorMessage = "Please provide Ventilation")]
        public string Ventilation { get; set; }

        [Display(Name = "Shipment Term")]
        //[Required(ErrorMessage = "Please provide Shipment Term")]
        public string ShipmentTerm { get; set; }

        [Display(Name = "Load Port")]
        //[Required(ErrorMessage = "Please provide Load Port")]
        public string LoadPort { get; set; }

        [Display(Name = "Discharge Port")]
        //[Required(ErrorMessage = "Please provide Discharge Port")]
        public string DischargePort { get; set; }

        [Display(Name = "Trans-Shipment Port")]
        //[Required(ErrorMessage = "Please provide Trans-Shipment Port")]
        public string TransshipmentPort { get; set; }

        [Display(Name = "Trans-Shipment Type")]
        //[Required(ErrorMessage = "Please provide Trans-Shipment Type")]
        public string TransshipmentType { get; set; }

        [Display(Name = "Place Of Receipt")]
        //[Required(ErrorMessage = "Please provide Place of Receipt")]
        public string PlaceOfReceipt { get; set; }

        [Display(Name = "Place Of Delivery")]
        //[Required(ErrorMessage = "Please provide Place of Delivery")]
        public string PlaceOfDelivery { get; set; }

        [Display(Name = "FreeDays POL")]
        //[Range(1, int.MaxValue, ErrorMessage = "The field {0} must be greater than 0")]
        public int POLFreeDays { get; set; }

        [Display(Name = "FreeDays POD")]
        //[Range(1, int.MaxValue, ErrorMessage = "The field {0} must be greater than 0")]
        public int PODFreeDays { get; set; }

        [Display(Name = "Creation Date")]
        public DateTime CreationDate { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime ModifiedDate { get; set; }


        [Display(Name = "Valid From")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}")] //ApplyFormatInEditMode = true
        public DateTime EffectiveDate { get; set; }

        [Display(Name = "Valid Till")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}")]
        public DateTime Validity { get; set; }

        [Display(Name = "QuoteRef ID")]
        public string QuoteRefID { get; set; }

        [Display(Name = "Status")]
        public string StatusDIS { get; set; }

        [Display(Name = "Grand Total Sales")]
        public decimal GTotalSalesCal { get; set; }

        [Display(Name = "Quantity")]
        //[Range(1, int.MaxValue, ErrorMessage = "The field {0} must be greater than 0")]
        public int Quantity { get; set; }

        [Display(Name = "Equipment Type")]
        public string EquipmentType { get; set; }

        [Display(Name = "Cargo Type")]
       // [Required(ErrorMessage = "Please specify Cargo Type", AllowEmptyStrings = false)]
        public string CargoType { get; set; }

        [Display(Name = "UN (For DG)")]
        //[Required(ErrorMessage = "Please specify Cargo Type", AllowEmptyStrings = false)]
        public string UNNo { get; set; }

        [Display(Name = "IMO (For DG)")]
        //[Required(ErrorMessage = "Please specify Cargo Type", AllowEmptyStrings = false)]
        public string IMO { get; set; }

        [Display(Name = "Rate (USD)")]
        //[Range(0.00001,int.MaxValue, ErrorMessage = "The field {0} must be greater than 0")]
        public decimal Rate { get; set; }

        [Display(Name = "Rate Countered")]
        public decimal RateCountered { get; set; }

        [Display(Name = "Rate ID")]
        public string RateID { get; set; }

        [Display(Name = "Remarks")]
        public string Remark { get; set; }

        public List<DQuoteSales> QuoteLocalCharges { get; set; }

        public QuoteRef()
        {
            QuoteLocalCharges = new List<DQuoteSales>();            
        }
    }

    public class DQuoteSales
    {
        public string ID { get; set; }
        public string RateID { get; set; }
        public string Description { get; set; }
        public decimal UnitRate { get; set; }
        public string Currency { get; set; }
        public decimal Exrate { get; set; }
        public string PayBy { get; set; }
        public string PaymentTerm { get; set; }
        public string PayMode { get; set; }

    }
}