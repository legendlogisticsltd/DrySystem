using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DryAgentSystem.Data
{
    public class Constant
    {
        //Status
        public const string Draft = "DRAFT";
        public const string ApprovalRequired = "APPROVAL REQUIRED";
        public const string CounterOffer = "COUNTER OFFER";
        public const string Rejected = "REJECTED";
        public const string Approved = "APPROVED";
        public const string Confirmed = "CONFIRMED";
        public const string Issued = "ISSUED";

        //Invoice Constants
        public const string Dry = "DRY";
        public const string Reefer = "Reefer";
        public const string DryInitial = "DB";
        public const string ReeferInitial = "RF";
        public const string Export = "EXPORT";
        public const string Import = "IMPORT";
        public const string ExportInitial = "EX";
        public const string ImportInitial = "IM";
        public const string ProformaInitial = "PR";
    }
}