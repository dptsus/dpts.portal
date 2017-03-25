using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPTS.Web.Models
{
    public class JoinUsViewModel
    {
        public JoinUsViewModel()
        {
            SpecialityList = new List<SelectListItem>();
            SubSpecialityList = new List<SelectListItem>();
            AddressModel = new AddressViewModel();

        }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Middle Name")]
        public string MiddleName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Your must provide a PhoneNumber")]
        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
            ErrorMessage = "Not a valid Phone number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; }

        public IList<SelectListItem> SpecialityList { get; set; }

        public IList<SelectListItem> SubSpecialityList { get; set; }

        public int Speciality { get; set; }

        public int SubSpeciality { get; set; }

        public string Gender { get; set; }

        [Required(ErrorMessage = "select at least one qualification")]
        public string[] Expertise { get; set; }
        // DOB

        [Display(Name = "Day")]
        public int? DateOfBirthDay { get; set; }

        [Display(Name = "Month")]
        public int? DateOfBirthMonth { get; set; }

        [Display(Name = "Year")]
        public int? DateOfBirthYear { get; set; }


        public string DateOfBirth { get; set; }

        public DateTime? ParseDateOfBirth()
        {
            if (!DateOfBirthYear.HasValue || !DateOfBirthMonth.HasValue || !DateOfBirthDay.HasValue)
                return null;

            DateTime? dateOfBirth = null;
            try
            {
                dateOfBirth = new DateTime(DateOfBirthYear.Value, DateOfBirthMonth.Value, DateOfBirthDay.Value);
            }
            catch (Exception)
            {
                //todo log exceptions
            }
            return dateOfBirth;
        }

        public AddressViewModel AddressModel { get; set; }

    }
}