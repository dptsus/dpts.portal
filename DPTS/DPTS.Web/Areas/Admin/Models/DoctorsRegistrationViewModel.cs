using System.ComponentModel.DataAnnotations;

namespace DPTS.Web.Areas.Admin.Models
{
    public class DoctorsRegistrationViewModel
    { 
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "LastName")]
        public string LastName { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        [Required]
        [Display(Name = "EmailId")]
        [EmailAddress]
        public string EmailId { get; set; }

        [Required]
        [Display(Name = "PhoneNumber")]
        [Phone]
        public string PhoneNumber { get; set; }


        [Display(Name = "Gender")]
        [DataType(DataType.Text)]
        public string Gender { get; set; }

        [Required]
        [Display(Name = "ShortProfile")]
        [DataType(DataType.Text)]
        public string ShortProfile { get; set; }


        [Display(Name = "RegistratioNumber")]
        [DataType(DataType.Text)]
        public string RegistrationNumber { get; set; }

        [Required]
        [Display(Name = "DateOfBirth")]
        [DataType(DataType.Date)]
        public string DateOfBirth { get; set; }

        [Required]
        [Display(Name = "Specality")]
        [DataType(DataType.Text)]
        public string Specality { get; set; }

        [Required]
        [Display(Name = "YearsOfExperience")]
        [DataType(DataType.Text)]
        public string YearsOfExperience { get; set; } 
    }
}