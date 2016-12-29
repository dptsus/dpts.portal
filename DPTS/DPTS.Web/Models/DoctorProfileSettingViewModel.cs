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
            // Speciality = new List<Speciality>();
            AvailableSpeciality = new List<SelectListItem>();
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

        public DateTime DateCreated { get; set; }

        public IList<string> SelectedSpeciality { get; set; }
        //public IList<Speciality> Speciality { get; set;
        public IList<SelectListItem> AvailableSpeciality { get; set; }

        public string Tags { get; set; }


    }

}