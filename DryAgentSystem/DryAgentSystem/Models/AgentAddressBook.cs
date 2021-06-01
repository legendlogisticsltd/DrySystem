using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DryAgentSystem.Models
{
    public class AgentAddressBook
    {
        public string Address { get; set; }
        public string CompanyName { get; set; }
        public string Country { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationTimestamp { get; set; }
        public string Email { get; set; }
        public string ID { get; set; }
        public string IDCompany { get; set; }
        public DateTime ModificationTimestamp { get; set; }
        public string PhoneNo { get; set; }
        public string Remark { get; set; }
        public string Website { get; set; }
        public string Zipcode { get; set; }
        public string CustomAddress { get; set; }
    }
}