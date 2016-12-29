using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domain.Entities
{
    public class Doctor : BaseEntityWithDateTime
    {
        private ICollection<Address> _addresses;
        private ICollection<Speciality> _speciality;
        public Doctor()
        {
            this.DoctorGuid = Guid.NewGuid();
        }

        [Required]
        public string DoctorId { get; set; }

        public Guid DoctorGuid { get; set; }

        public string Gender { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Qualifications { get; set; }

        public string RegistrationNumber { get; set; }

        public int YearsOfExperience { get; set; }

        public string ShortProfile { get; set; }

        public string Expertise { get; set; }

        public string Subscription { get; set; }

        public bool IsActive { get; set; }

        public bool Deleted { get; set; }

        public int DisplayOrder { get; set; }

        public virtual ICollection<Address> Addresses
        {
            get { return _addresses ?? (_addresses = new List<Address>()); }
            protected set { _addresses = value; }
        }
        public virtual ICollection<Speciality> Speciality
        {
            get { return _speciality ?? (_speciality = new List<Speciality>()); }
            protected set { _speciality = value; }
        }
    }
}
