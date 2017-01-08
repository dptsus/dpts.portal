namespace DPTS.Domain
{
    using System.ComponentModel.DataAnnotations;

    public partial class SpecialityMapping :BaseEntityWithDateTime
    {
        [Required]
        [StringLength(128)]
        public string Doctor_Id { get; set; }

        [Required]
        public int Speciality_Id { get; set; }

        public virtual Doctor Doctor { get; set; }

        public virtual Speciality Speciality { get; set; }
    }
}
