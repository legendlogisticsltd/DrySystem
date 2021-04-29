using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DryAgentSystem.Models
{
    public class Inventory
    {
        public DateTime ActiveDDate { get; set; }
        public string ActiveDDepotTerminal { get; set; }
        public string ActiveDPort { get; set; }
        public DateTime ActiveLDate { get; set; }
        public string ActiveLDepotTerminal { get; set; }
        public string ActiveLPort { get; set; }
        public string AVStatus { get; set; }
        public string Capacity { get; set; }
        public DateTime CreateDate { get; set; }
        public string CROBookingStatus { get; set; }
        public string CRONO { get; set; }
        public DateTime ERequestDate { get; set; }
        public string ERequester { get; set; }
        public string ERequestLog { get; set; }
        public DateTime ETA { get; set; }
        public DateTime ETD { get; set; }
        public string LastCargo { get; set; }
        public string ModifyDate { get; set; }
        public string Remark { get; set; }
        public string StatusTrack { get; set; }
        public DateTime SurveyDate { get; set; }
        public string TankAVStatus { get; set; }
        public string TankCOStatus { get; set; }
        public string TankNo { get; set; }
        public string TankRepoStatus { get; set; }
        public string UniversalSerialNr { get; set; }
        public string User { get; set; }
        public string UserModify { get; set; }
        public string WeekNrDisch { get; set; }
        public string WeekNrLoad { get; set; }
    }
}