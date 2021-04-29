using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DryAgentSystem.Models
{
    public class ShipmentDetails
    {
        [Display(Name = "Acct Month")]
        public string AcctMonth { get; set; }

        [Display(Name = "Booking No")]
        public string BookingNo { get; set; }

        [Display(Name = "Cancel Reason")]
        public string CancelReason { get; set; }

        [Display(Name = "Carrier Service Priority FRT")]
        public int CarrierServicePriorityFRT { get; set; }

        [Display(Name = "Carrier Service Priority ID")]
        public string CarrierServicePriorityID { get; set; }

        [Display(Name = "Carrier Service Priority Label")]
        public string CarrierServicePriorityLabel { get; set; }

        [Display(Name = "Closing Date")]
        public DateTime ClosingDate { get; set; }

        [Display(Name = "Closing Time")]
        public DateTime ClosingTime { get; set; }

        [Display(Name = "Cost Agency")]
        public string CostAgency { get; set; }

        [Display(Name = "Cost Depot")]
        public string CostDepot { get; set; }

        [Display(Name = "Cost Lease")]
        public int CostLease { get; set; }

        [Display(Name = "Cost Ocean")]
        public string CostOcean { get; set; }

        [Display(Name = "Cost Repo")]
        public string CostRepo { get; set; }

        [Display(Name = "count")]
        public string count { get; set; }

        [Display(Name = "Country")]
        public string Country { get; set; }

        [Display(Name = "Create Date")]
        public DateTime CreateDate { get; set; }

        [Display(Name = "Creation Timestamp")]
        public string CreationTimestamp { get; set; }

        [Display(Name = "Customer Ref")]
        public string CustomerRef { get; set; }

        [Display(Name = "Date ATA")]
        public string DateATA { get; set; }

        [Display(Name = "Date SOB")]
        public string DateSOB { get; set; }

        [Display(Name = "DDetention")]
        public string DDetention { get; set; }

        [Display(Name = "Det Amount Status")]
        public string DetAmountStatus { get; set; }

        [Display(Name = "Detention Month")]
        public string DetentionMonth { get; set; }

        [Display(Name = "Detention Status")]
        public string DetentionStatus { get; set; }

        [Display(Name = "Det Rate Disch")]
        public int DetRateDisch { get; set; }

        [Display(Name = "Det Rate Load")]
        public int DetRateLoad { get; set; }

        [Display(Name = "Disch Agent Address")]
        public string DischAgentAddress { get; set; }

        [Display(Name = "Disch FT")]
        public int DischFT { get; set; }

        [Display(Name = "Disch Port")]
        public string DischPort { get; set; }

        [Display(Name = "DTotal Detention")]
        public string DTotalDetention { get; set; }

        [Display(Name = "ETA")]
        public string ETA { get; set; }

        [Display(Name = "ETA IGM")]
        public string ETAIGM { get; set; }

        [Display(Name = "ETD")]
        public string ETD { get; set; }

        [Display(Name = "Feeder Service Priority FRT")]
        public int FeederServicePriorityFRT { get; set; }

        [Display(Name = "Feeder Service Priority ID")]
        public string FeederServicePriorityID { get; set; }

        [Display(Name = "Feeder Service Priority Label")]
        public string FeederServicePriorityLabel { get; set; }

        [Display(Name = "Fixed Cost")]
        public string FixedCost { get; set; }

        [Display(Name = "FMC Type")]
        public string FMC_Type { get; set; }

        [Display(Name = "Grand Total Cost")]
        public string GrandTotalCost { get; set; }

        [Display(Name = "Grand Total Sales")]
        public string GrandTotalSales { get; set; }

        [Display(Name = "House BL No.")]
        public string HBLHAWB { get; set; }

        [Display(Name = "ID Agent Disch")]
        public string IDAgentDisch { get; set; }

        [Display(Name = "ID Agent Load")]
        public string IDAgentLoad { get; set; }

        [Display(Name = "ID Charge Party")]
        public string IDChargeParty { get; set; }

        [Display(Name = "ID FFreight")]
        public string IDFFreight { get; set; }

        [Display(Name = "ID OFreight")]
        public string IDOFreight { get; set; }

        [Display(Name = "ID Product")]
        public string IDProduct { get; set; }

        [Display(Name = "ID QuoteRef")]
        public string IDQuoteRef { get; set; }

        [Display(Name = "ID Shipper")]
        public string IDShipper { get; set; }

        [Display(Name = "Import")]
        public int import { get; set; }

        [Display(Name = "Job Ref")]
        public string JobRef { get; set; }

        [Display(Name = "Job Ref Serial No")]
        public int JobRefSerialNo { get; set; }

        [Display(Name = "k ID PO")]
        public int k_ID_PO { get; set; }

        [Display(Name = "Last 3 Cargo List")]
        public string Last3CargoList { get; set; }

        [Display(Name = "L Detention")]
        public string LDetention { get; set; }

        [Display(Name = "Lease Period")]
        public int LeasePeriod { get; set; }

        [Display(Name = "Lease Period Type")]
        public string LeasePeriodType { get; set; }

        [Display(Name = "Lease Rate")]
        public int LeaseRate { get; set; }

        [Display(Name = "Lease Rate Type")]
        public string LeaseRateType { get; set; }

        [Display(Name = "Load Agent Address")]
        public string LoadAgentAddress { get; set; }

        [Display(Name = "Load FT")]
        public int LoadFT { get; set; }

        [Display(Name = "Load Port")]
        public string LoadPort { get; set; }

        [Display(Name = "Log")]
        public string Log { get; set; }

        [Display(Name = "Master BL No")]
        public string MBLMAWB { get; set; }

        [Display(Name = "Modify Date")]
        public DateTime ModifyDate { get; set; }

        [Display(Name = "Note Billing Instruction")]
        public string NoteBillingInstruction { get; set; }

        [Display(Name = "Note MNR")]
        public string NoteMNR { get; set; }

        [Display(Name = "Note Operation")]
        public string NoteOperation { get; set; }

        [Display(Name = "Place Of Delivery")]
        public string PlaceOfDelivery { get; set; }

        [Display(Name = "Place Of Receipt")]
        public string PlaceOfReceipt { get; set; }

        [Display(Name = "PNL")]
        public string PNL { get; set; }

        [Display(Name = "PNLPCT")]
        public string PNLPCT { get; set; }

        [Display(Name = "POD")]
        public string POD { get; set; }

        [Display(Name = "POL")]
        public string POL { get; set; }

        [Display(Name = "Port Pair")]
        public string PortPair { get; set; }

        [Display(Name = "Print Approval")]
        public string PrintApproval { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Display(Name = "Charge Party")]
        public string pzChargeParty { get; set; }

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Quantity Lifting")]
        public string QuantityLifting { get; set; }

        [Display(Name = "Quote Type")]
        public string QuoteType { get; set; }

        [Display(Name = "Remark")]
        public string Remark { get; set; }

        [Display(Name = "Release Date")]
        public DateTime ReleaseDate { get; set; }

        [Display(Name = "Remark Print")]
        public string RemarkPrint { get; set; }

        [Display(Name = "REPO Port")]
        public string REPOPort { get; set; }

        [Display(Name = "Search Status")]
        public string SearchStatus { get; set; }

        [Display(Name = "Shipment Term")]
        public string ShipmentTerm { get; set; }

        [Display(Name = "Shipper Address")]
        public string ShipperAddress { get; set; }

        [Display(Name = "Status Cancel")]
        public string StatusCancel { get; set; }

        [Display(Name = "Status Locked")]
        public string StatusLocked { get; set; }

        [Display(Name = "Status Shipment")]
        public string StatusShipment { get; set; }

        [Display(Name = "Status UNLocked")]
        public string StatusUNLocked { get; set; }

        [Display(Name = "Test")]
        public string test { get; set; }

        [Display(Name = "Test Final")]
        public string testfinal { get; set; }

        [Display(Name = "Transit Day")]
        public int TransitDay { get; set; }

        [Display(Name = "Transit Repo")]
        public int TransitRepo { get; set; }

        [Display(Name = "Universal Serial Nr")]
        public string UniversalSerialNr { get; set; }

        [Display(Name = "User")]
        public string User { get; set; }

        [Display(Name = "User Modify")]
        public string UserModify { get; set; }

        [Display(Name = "vessel Label")]
        public string vesselLabel { get; set; }

        [Display(Name = "Voy No IGM")]
        public string VoyNoIGM { get; set; }

        [Display(Name = "VS Name IGM")]
        public string VSNameIGM { get; set; }

        [Display(Name = "Week Nr")]
        public string WeekNr { get; set; }

        public IEnumerable<SelectListItem> ContainerList { get; set; }

        public List<string> SelectedContainerList { get; set; }

        public ShipmentDetails()
        {
            SelectedContainerList = new List<string>();
        }
    }
}