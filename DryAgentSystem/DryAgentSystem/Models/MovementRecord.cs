using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DryAgentSystem.Models
{
    public class MovementRecord
    {
		[Display(Name = "Container No")]
		public string ContainerNo { get; set; }
		public string ID { get; set; }
		public string ModifyUser { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime ModifyDate { get; set; }
		public DateTime LGateOutDate { get; set; }
		public DateTime LGateInDate { get; set; }
		public string LDepotTerminal { get; set; }
		public string UniversalSerialNr { get; set; }
		public DateTime DGateInDate { get; set; }
		public string DDepotTerminal { get; set; }
		public string ProductName { get; set; }
		public string CreatedBy { get; set; }

		[Display(Name = "Discharge Port")]
		public string DischargePort { get; set; }

		[Display(Name = "Load Port")]
		public string LoadPort { get; set; }

		[Display(Name = "Job Ref")]
		public string JobRefSerialNo { get; set; }
		public DateTime AVDate { get; set; }

		[Display(Name = "Vessel Schedule")]
		public string VesselSchedule { get; set; }
		public DateTime RepoETA { get; set; }
		public DateTime ETAPostDP { get; set; }
		public DateTime ETAPrevDP { get; set; }
		public DateTime ETDPostLP { get; set; }
		public DateTime ETDPrevLP { get; set; }

		[Display(Name = "DO Number")]
		public string DONumber { get; set; }

		[Display(Name = "DO Issue Date")]
		public DateTime DOIssueDate { get; set; }

		[Display(Name = "ATA")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
		public DateTime ATA { get; set; }

		[Display(Name = "ETA")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
		public DateTime ETA { get; set; }

		[Display(Name = "ETD")]
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
		public DateTime ETD { get; set; }

		[Display(Name = "Vessel Name")]
		public string VesselName { get; set; }

		[Display(Name = "Seal No")]
		public string SealNo { get; set; }

		[Display(Name = "Voyage")]
		public string Voyage { get; set; }
	}
}