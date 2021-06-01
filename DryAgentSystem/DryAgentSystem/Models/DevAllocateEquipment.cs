using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DryAgentSystem.Models
{
    public class DevAllocateEquipment
    {
        public DateTime CreateDate { get; set; }
        public DateTime EstPickUp { get; set; }
        public DateTime EstReturn { get; set; }
        public DateTime ETD { get; set; }
        public double GrossWeight { get; set; }
        public string GrossWeightUnit { get; set; }
        public string ID { get; set; }
        public string JobRefSerialNo { get; set; }
        public double Measurement { get; set; }
        public string MeasurementUnit { get; set; }
        public DateTime ModifyDate { get; set; }
        public string ModifyUser { get; set; }
        public string NettWeight { get; set; }
        public string SealNo { get; set; }
        public string ContainerNo { get; set; }
        public string UniversalSerialNr { get; set; }
        public string User { get; set; }
        public string FPOD { get; set; }
        public string Remarks { get; set; }
        public string DONumber { get; set; }
        public DateTime DOIssueDate { get; set; }
    }
}