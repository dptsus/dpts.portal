using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPTS.Web.Models
{
    public class AddressViewModel
    {
        public AddressViewModel()
        {
            AvailableCountry = new List<SelectListItem>();
            AvailableStateProvince = new List<SelectListItem>();
        }
        public IList<SelectListItem> AvailableCountry { get; set; }
        public IList<SelectListItem> AvailableStateProvince { get; set; }

        public int Id { get; set; }
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
    }
}