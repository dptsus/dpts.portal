using System;
using System.ComponentModel.DataAnnotations;

namespace DPTS.Web.Areas.Admin.Models
{
    public class JoinUsViewModel
    { 
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } 

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string EmailId { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        [Phone]
        public string PhoneNumber { get; set; }


        [Display(Name = "Gender")]
        [DataType(DataType.Text)]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "Short Profile")]
        [DataType(DataType.Text)]
        public string ProfessionalStatements { get; set; }

        [Required]
        [Display(Name = "Registratio Number")]
        [DataType(DataType.Text)]
        public string RegistrationNumber { get; set; }

        [Required]
        [Display(Name = "Date Of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Display(Name = "Specality")]
        [DataType(DataType.Text)]
        public string Specality { get; set; }
         
        [Display(Name = "Years Of Experience")] 
        public double YearsOfExperience { get; set; } 
    }
}