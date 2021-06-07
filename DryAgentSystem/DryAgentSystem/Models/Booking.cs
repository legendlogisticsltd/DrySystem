using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DryAgentSystem.Models
{
    public class Booking
    {
        [Display(Name = "Agent Name")]
        public string AgentName { get; set; }

        [Display(Name = "Company ID")]
        public string IDCompany { get; set; }

        [Display(Name = "Agency Name")]
        //[Required(ErrorMessage = "Please provide Agency Name")]
        public string CompanyName { get; set; }

        [Display(Name = "Rate Type")]
        [Required(ErrorMessage = "Please provide Rate Type")]
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
       // [Required(ErrorMessage = "Please provide Load Port")]
        public string LoadPort { get; set; }

        [Display(Name = "Discharge Port")]
      //  [Required(ErrorMessage = "Please provide Discharge Port")]
        public string DischargePort { get; set; }

        [Display(Name = "Place Of Receipt")]
       // [Required(ErrorMessage = "Please provide Place of Receipt")]
        public string PlaceOfReceipt { get; set; }

        [Display(Name = "Place Of Delivery")]
      //  [Required(ErrorMessage = "Please provide Place of Delivery")]
        public string PlaceOfDelivery { get; set; }

        [Display(Name = "Trans-Shipment Port")]
        //[Required(ErrorMessage = "Please provide Trans-Shipment Port")]
        public string TransshipmentPort { get; set; }

        [Display(Name = "Trans-Shipment Type")]
        //[Required(ErrorMessage = "Please provide Trans-Shipment Type")]
        public string TransshipmentType { get; set; }

        [Display(Name = "FreeDays POL")]
        //[Range(1, int.MaxValue, ErrorMessage = "The field {0} must be greater than 0")]
        public int POLFreeDays { get; set; }

        [Display(Name = "FreeDays POD")]
       // [Range(1, int.MaxValue, ErrorMessage = "The field {0} must be greater than 0")]
        public int PODFreeDays { get; set; }

        [Display(Name = "Creation Date")]
        public DateTime CreationDate { get; set; }

        [Display(Name = "Modified Date")]
        public DateTime ModifiedDate { get; set; }

        [Display(Name = "Grand Total Sales")]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public double GTotalSalesCal { get; set; }

        [Display(Name = "Grand Total Cost")]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public double GTotalCostCal { get; set; }

        [Display(Name = "Total Selling")]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public double TotalSalesItemCal { get; set; }

        [Display(Name = "Profit & Loss Calculation")]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public double PNLCal { get; set; }

        [Display(Name = "Total Other Revenue")]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public double TotalXRevItemCal { get; set; }

        //[Display(Name = "Valid From")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}")] //ApplyFormatInEditMode = true
        //public DateTime EffectiveDate { get; set; }

        //[Display(Name = "Valid Till")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}")]
        //public DateTime Validity { get; set; }

        [Display(Name = "QuoteRef ID")]
        public string QuoteRefID { get; set; }

        [Display(Name = "Booking No")]
        public string BookingNo { get; set; }

        [Display(Name = "Booking Status")]
        public string BookingStatus { get; set; }

        [Display(Name = "Quantity")]
        //[Range(1, int.MaxValue, ErrorMessage = "The field {0} must be greater than 0")]
        public int Quantity { get; set; }

        [Display(Name = "Equipment Type")]
        public string EquipmentType { get; set; }

        [Display(Name = "Cargo Type")]
        //[Required(ErrorMessage = "Please specify Cargo Type", AllowEmptyStrings = false)]
        public string CargoType { get; set; }

        [Display(Name = "UN (For DG)")]
        //[Required(ErrorMessage = "Please specify Cargo Type", AllowEmptyStrings = false)]
        public string UNNo { get; set; }

        [Display(Name = "IMO (For DG)")]
        //[Required(ErrorMessage = "Please specify Cargo Type", AllowEmptyStrings = false)]
        public string IMO { get; set; }

        [Display(Name = "Rate (USD)")]
        //[Range(0.00001,int.MaxValue, ErrorMessage = "The field {0} must be greater than 0")]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public double Rate { get; set; }

        [Display(Name = "Rate Countered")]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public double RateCountered { get; set; }

        [Display(Name = "Rate ID")]
        public string RateID { get; set; }

        [Display(Name = "Remarks")]
        public string Remark { get; set; }
        
        [Display(Name = "CRO Remarks")]
        public string CRORemarks { get; set; }

        [Display(Name = "Booking ID")]
        public string BookingID { get; set; }

        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; }

        [Display(Name = "Accounting Month")]
        public string AccMonth { get; set; }

        [Display(Name = "Universal Serial Number")]
        public string UniversalSerialNr { get; set; }

        [Display(Name = "Commodity")]
        [Required(ErrorMessage = "Please specify Commodity", AllowEmptyStrings = false)]
        public string Commodity { get; set; }

        [Display(Name = "Commodity Group")]
        [Required(ErrorMessage = "Please specify Commodity Group", AllowEmptyStrings = false)]
        public string CommodityGroup { get; set; }

        [Display(Name = "Cut-off Date & Time")]
        public DateTime CutoffDate { get; set; }

        [Display(Name = "Cut-off Date & Time")]
        //[DataType(DataType.Time)]
        //[DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public TimeSpan CutoffDateandTime { get; set; }

        [Display(Name = "Service Mode")]
        [Required(ErrorMessage = "Please specify Service Mode", AllowEmptyStrings = false)]
        public string ServiceMode { get; set; }

        [Display(Name = "Gross Weight")]
        [Range(0.01, int.MaxValue, ErrorMessage = "The field {0} must be greater than 0")]
        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Please enter Valid Weight upto 2 decimal place")]
        [DisplayFormat(DataFormatString = "{0:0.00}", ApplyFormatInEditMode = true)]
        public double Grossweight { get; set; }

        [Display(Name = "Gross Weight Measurement")]
        public string GrossweightMeasurement { get; set; }

        //[Display(Name = "Load Terminal")]
        //public string LoadTerminal { get; set; }

        //[Display(Name = "Discharge Terminal")]
        //public string DischargeTerminal { get; set; }

        //[Display(Name = "Load Agent")]
        //public string LoadAgent { get; set; }

        //[Display(Name = "Discharge Agent")]
        //public string DischAgent { get; set; }



        [Display(Name = "Payment Terms")]
        public string PaymentTerms { get; set; }

        [Display(Name = "Volume")]
        //[Required(ErrorMessage = "Please specify Volume", AllowEmptyStrings = false)]
        public string Volume { get; set; }

        //[Display(Name = "SOC")]
        //public string SOC { get; set; }

        public List<Vessel> Vessels { get; set; }

        public Vessel VesselModel { get; set; }

        public Booking()
        {
            Vessels = new List<Vessel>();
            VesselModel = new Vessel();
        }

        [Display(Name = "Attn")]
        [Required(ErrorMessage = "Please specify Attn Address", AllowEmptyStrings = false)]
        public string AddressAttn { get; set; }

        [Display(Name = "Fax")]
        [Required(ErrorMessage = "Please specify Fax", AllowEmptyStrings = false)]
        public string AddressFax { get; set; }

        [Display(Name = "Telephone No.")]
        [Required(ErrorMessage = "Please specify Telephone No.", AllowEmptyStrings = false)]
        public string AddressTel { get; set; }

        [Display(Name = "Address To")]
        [Required(ErrorMessage = "Please specify Address To", AllowEmptyStrings = false)]
        public string AddressTo { get; set; }

        [Display(Name = "Collection Yard")]
        [Required(ErrorMessage = "Please specify Collection Yard", AllowEmptyStrings = false)]
        public string CollectionYard { get; set; }

        [Display(Name = "CRO No")]
        public string ContainerReleaseOrderNo { get; set; }

        [Display(Name = "Voyage No")]
        public string VoyageNo { get; set; }

        [Display(Name = "Job Ref")]
        public string JobRef { get; set; }
        
        [Display(Name = "Depot Address")]
        public string Address { get; set; }

        [Display(Name = "Depo Phone No")]
        public string PhoneNo { get; set; }

        [Display(Name = "Depo Email")]
        public string Email { get; set; }

        [Display(Name = "CRO PickUp Date")]
        public DateTime CROPickUpDate { get; set; } 
        
        [Display(Name = "Shipper")]
        public string Shipper { get; set; }

        public bool hasshipment { get; set; }
    }
}