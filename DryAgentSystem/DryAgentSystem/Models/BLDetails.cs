using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DryAgentSystem.Models
{
    public class BLDetails
    {
        [Display(Name = "BL Count")]
        public string BLCount { get; set; }

        [Display(Name = "BL Type")]
        public string BLTypes { get; set; }
        
        [Display(Name = "BL Status")]
        public string BLStatus { get; set; }
        
        [Display(Name = "BL Seaway Status")]
        public string BLSeawayStatus { get; set; }

        [Display(Name = "BL Finalised Date")]
        [Required(ErrorMessage = "Please provide BL Finalised Date")]
        public DateTime BLFinalisedDate { get; set; }

        [Display(Name = "Cargo Description")]
        [Required(ErrorMessage = "Please provide Cargo Description")]
        public string CargoDescription { get; set; }

        [Display(Name = "Cargo Description2")]
        public string CargoDescription2 { get; set; }

        [Display(Name = "Consignee Address")]
        [Required(ErrorMessage = "Please provide Consignee Address")]
        public string ConsigneeAddressBL { get; set; }

        [Display(Name = "Consignee Address SI")]
        public string ConsigneeAddressSI { get; set; }

        [Display(Name = "Consignee Name")]
        [Required(ErrorMessage = "Please provide Consignee Name")]
        public string ConsigneeNameBL { get; set; }

        [Display(Name = "Consignee Name SI")]
        public string ConsigneeNameSI { get; set; }

        [Display(Name = "Country POD")]
        public string CountryPOD { get; set; }

        [Display(Name = "Country PODelivery")]
        public string CountryPODelivery { get; set; }

        [Display(Name = "Country POL")]
        public string CountryPOL { get; set; }

        [Display(Name = "Country POR")]
        public string CountryPOR { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Creation Timestamp")]
        public DateTime CreationTimestamp { get; set; }

        [Display(Name = "Date of Issue")]
        [Required(ErrorMessage = "Please provide Date of Issue")]
        public DateTime DateofIssue { get; set; }

        [Display(Name = "Disch Agent Address")]
        public string DischAgentAddress { get; set; }

        [Display(Name = "Disch Agent Name")]
        [Required(ErrorMessage = "Please provide Disch Agent Name")]
        public string DischAgentNameBL { get; set; }

        [Display(Name = "Disch Port")]
        public string DischPort { get; set; }

        [Display(Name = "Field Status")]
        public string FieldStatus { get; set; }

        [Display(Name = "Field Status Shipment")]
        public string FieldStatusShipment { get; set; }

        [Display(Name = "FPOD")]
        public string FPOD { get; set; }

        [Display(Name = "Gross Weight Unit")]
        public string GrossWeightUnit { get; set; }

        [Display(Name = "Total Gweight")]
        public string TotalGweight { get; set; }

        [Display(Name = "HBL Freight Payment")]
        [Required(ErrorMessage = "Please provide HBL Freight Payment")]
        public string HBLFreightPayment { get; set; }

        [Display(Name = "Job Ref")]
        public string JobRef { get; set; }

        [Display(Name = "Job Ref Full")]
        public string JobRefFull { get; set; }

        [Display(Name = "Laden On Board")]
        [Required(ErrorMessage = "Please provide Laden On Board Date")]
        public DateTime LadenOnBoard { get; set; }

        [Display(Name = "Load Port")]
        public string LoadPort { get; set; }

        [Display(Name = "Marks and No.")]
        [Required(ErrorMessage = "Please provide Marks and No.")]
        public string MarksandNo { get; set; }

        [Display(Name = "Marks and No2")]
        public string MarksandNo2 { get; set; }

        [Display(Name = "MBL Freight Payment")]
        [Required(ErrorMessage = "Please provide MBL Freight Payment")]
        public string MBLFreightPayment { get; set; }

        [Display(Name = "Measurement Unit")]
        public string MeasurementUnit { get; set; }

        [Display(Name = "Modification Timestamp")]
        public DateTime ModificationTimestamp { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        [Display(Name = "NetWt Unit")]
        public string NetWtUnit { get; set; }

        [Display(Name = "No. of Original BL issued")]
        [Required(ErrorMessage = "Please provide No. of Original BL issued")]
        public string NoofOriginalBLissued { get; set; }

        [Display(Name = "Notify Party Address")]
        [Required(ErrorMessage = "Please provide Notify Party Address")]
        public string NotifyPartyAddress { get; set; }

        [Display(Name = "Notify Party Name")]
        [Required(ErrorMessage = "Please provide Notify Party Name")]
        public string NotifyPartyName { get; set; }

        [Display(Name = "Place of Delivery")]
        public string PlaceofDelivery { get; set; }

        [Display(Name = "Place of Issue")]
        public string PlaceofIssue { get; set; }

        [Display(Name = "Place of Receipt")]
        public string PlaceofReceipt { get; set; }
        
        [Display(Name = "No. of Packages")]
        [Range(0.00001, int.MaxValue, ErrorMessage = "The field {0} must be greater than 0")]
        public int NoOfPkgs { get; set; }
        
        [Display(Name = "Package Type")]
        [Required(ErrorMessage = "Please provide Package Type")]
        public string PkgType { get; set; }

        [Display(Name = "PreCarriage By")]
        public string PreCarriageBy { get; set; }

        [Display(Name = "Product Der")]
        public string ProductDer { get; set; }

        [Display(Name = "Product DG Class")]
        public string ProductDGClass { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Display(Name = "Product Type")]
        public string ProductType { get; set; }

        [Display(Name = "Product UNNo")]
        public string ProductUNNo { get; set; }

        [Display(Name = "Shipper Address BL")]
        public string ShipperAddressBL { get; set; }

        [Display(Name = "Shipper Address SI")]
        public string ShipperAddressSI { get; set; }

        [Display(Name = "Shipper Name BL")]
        
        public string ShipperNameBL { get; set; }

        [Display(Name = "Shipper Name SI")]
        public string ShipperNameSI { get; set; }

        [Display(Name = "Tank List")]
        public string TankList { get; set; }

        [Display(Name = "Total Container Measurement")]
        public string TotalContainerMeasurement { get; set; }

        [Display(Name = "Total NetWt")]
        public string TotalNetWt { get; set; }

        [Display(Name = "Universal Serial Nr")]
        public string UniversalSerialNr { get; set; }

        [Display(Name = "Vessel")]
        public string Vessel { get; set; }

        [Display(Name = "VesselDetails")]
        public string VesselDetails { get; set; }        
    }
}