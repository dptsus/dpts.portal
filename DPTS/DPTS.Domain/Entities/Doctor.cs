namespace DPTS.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Doctor :BaseEntityWithDateTime
    {
        public Doctor()
        {
            SpecialityMapping = new HashSet<SpecialityMapping>();
            AddresseMapping = new HashSet<AddressMapping>();
        }

        public Guid DoctorGuid { get; set; }

        public string Gender { get; set; }

        public string Qualifications { get; set; }

        public string RegistrationNumber { get; set; }

        public int? YearsOfExperience { get; set; }

        public string ShortProfile { get; set; }

        public string Expertise { get; set; }

        public string Subscription { get; set; }

        public bool IsActive { get; set; }

        public bool Deleted { get; set; }

        public int DisplayOrder { get; set; }

        [Key]
        public string DoctorId { get; set; }

        public string DateOfBirth { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual ICollection<SpecialityMapping> SpecialityMapping { get; set; }

        public virtual ICollection<AddressMapping> AddresseMapping { get; set; }

    }
}
