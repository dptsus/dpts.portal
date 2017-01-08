using DPTS.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPTS.Web.Models
{
    public class DoctorProfileSettingViewModel
    {
        public DoctorProfileSettingViewModel()
        {
            AvailableSpeciality = new List<SelectListItem>();
          //  AvailableCountry = new List<SelectListItem>();
            //AvailableStateProvince = new List<SelectListItem>();
            SelectedSpeciality = new List<string>();
        }
        public string Id { get; set; }
        /// <summary>
        /// set & Get first name
        /// </summary>
        [Display(Name ="First Name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Set & Get last name
        /// </summary>
        [Display(Name ="Last Name")]
        public string LastName { get; set; }

        /// <summary>
        /// set & get gender
        /// </summary>
        [Display(Name = "Gender")]
        public string  Gender { get; set; }

        /// <summary>
        /// set & get date of birth
        /// </summary>
        [Display(Name = "Date Of Birth")]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// set & get email
        /// </summary>
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        /// <summary>
        /// set & get phone number
        /// </summary>
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// set & get short description
        /// </summary>
        [Display(Name ="Short Description")]
        public string ShortProfile { get; set; }

        /// <summary>
        /// Qualifications
        /// </summary>
        [Display(Name = "Qualifications")]
        public string Qualifications { get; set; }

        /// <summary>
        /// Registration Number
        /// </summary>
        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; }

        /// <summary>
        /// NoOfYearExperience
        /// </summary>
        [Display(Name = "Experience [/yrs]")]
        public int NoOfYearExperience { get; set; }



        public DateTime DateCreated { get; set; }

        public IList<string> SelectedSpeciality { get; set; }
        public IList<SelectListItem> AvailableSpeciality { get; set; }

        // DOB

        [Display(Name ="Day")]
        public int? DateOfBirthDay { get; set; }
        [Display(Name = "Month")]
        public int? DateOfBirthMonth { get; set; }
        [Display(Name = "Year")]
        public int? DateOfBirthYear { get; set; }
        public DateTime? ParseDateOfBirth()
        {
            if (!DateOfBirthYear.HasValue || !DateOfBirthMonth.HasValue || !DateOfBirthDay.HasValue)
                return null;

            DateTime? dateOfBirth = null;
            try
            {
                dateOfBirth = new DateTime(DateOfBirthYear.Value, DateOfBirthMonth.Value, DateOfBirthDay.Value);
            }
            catch { }
            return dateOfBirth;
        }


    }

}