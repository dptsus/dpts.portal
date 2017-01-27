using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DPTS.Domain.Entities
{
    public partial class Doctor :BaseEntityWithDateTime
    {
        public Doctor()
        {
            SpecialityMapping = new HashSet<SpecialityMapping>();
            AddresseMapping = new HashSet<AddressMapping>();
            AppointmentSchedules = new HashSet<AppointmentSchedule>();
            Schedules = new HashSet<Schedule>();
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

        public decimal Rating { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }

        public virtual ICollection<SpecialityMapping> SpecialityMapping { get; set; }

        public virtual ICollection<AddressMapping> AddresseMapping { get; set; }

        public virtual ICollection<AppointmentSchedule> AppointmentSchedules { get; set; }

        public virtual ICollection<Schedule> Schedules { get; set; }

    }
}
