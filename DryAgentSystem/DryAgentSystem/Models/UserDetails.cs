using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CompareAttribute = System.ComponentModel.DataAnnotations.CompareAttribute;

namespace DryAgentSystem.Models
{
    public class UserDetails
    {

        public string UserID { get; set; }

        [Display(Name = "User Name")]
        public string Name { get; set; }

        //[Required(ErrorMessage = "Please provide Email ID", AllowEmptyStrings = false)]
        //[RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        [Display(Name = "Email ID")]
        public string EmailID { get; set; }

        [Required(ErrorMessage = "Please provide Password", AllowEmptyStrings = false)]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must contain minimum 6 digits")]
        public string Password { get; set; }

        [Display(Name = "Company ID")]
        public string IDCompany { get; set; }


        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Access Log")]
        public DateTime AccessLog { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; }

        [Display(Name = "Role Type")]
        public string RoleType { get; set; }
    }
}