using DPTS.Domain.Entities;
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
            AvailableCountry = new List<SelectListItem>();
            AvailableStateProvince = new List<SelectListItem>();
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

        //Address
        [Display(Name = "Hospital Name")]
        public string Hospital { get; set; }

        [Display(Name = "Country")]
        public int CountryId { get; set; }

        [Display(Name = "State")]
        public int StateProvinceId { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }

        [Display(Name = "Address Line 1")]
        public string Address1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string Address2 { get; set; }

        [Display(Name = "Zip /Postal Code")]
        public string ZipPostalCode { get; set; }

        [Display(Name = "Landline Number")]
        public string LandlineNumber { get; set; }

        [Display(Name = "Website")]
        public string Website { get; set; }

        [Display(Name = "Fax")]
        public string FaxNumber { get; set; }
        //end of address

        public DateTime DateCreated { get; set; }

        public IList<string> SelectedSpeciality { get; set; }
        public IList<SelectListItem> AvailableSpeciality { get; set; }
        public IList<SelectListItem> AvailableCountry { get; set; }
        public IList<SelectListItem> AvailableStateProvince { get; set; }


    }

}