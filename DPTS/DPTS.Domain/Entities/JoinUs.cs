using System;
using System.ComponentModel.DataAnnotations;

namespace DPTS.Domain.Entities
{
    public partial class JoinUs : BaseEntityWithDateTime
    {
        public string FirstName { get; set; } 
        public string LastName { get; set; }         
        public string EmailId { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string ProfessionalStatements { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Specality { get; set; }
        public double YearsOfExperience { get; set; }
        public Guid DoctorGuid { get; set; }
        public string Language { get; set; }
        public string Expertise { get; set; }
        public string Subscription { get; set; }
        public bool IsActive { get; set; }
        public bool Deleted { get; set; } 
        [Key]
        public string DoctorId { get; set; }
         
            }
}
