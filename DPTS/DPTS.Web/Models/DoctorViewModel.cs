using DPTS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DPTS.Domain.Entities;

namespace DPTS.Web.Models
{
    public class DoctorViewModel
    {
        public DoctorViewModel()
        {
            doctor = new Doctor();
            Addresses = new List<Address>();
            Specialitys = new List<Speciality>();

        }
        public string Id { get; set; }

        public virtual Doctor doctor { get; set; }

        public virtual IList<Address> Addresses { get; set; }

        public virtual IList<Speciality> Specialitys { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string MobileNumber { get; set; }


    }
}