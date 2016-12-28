namespace DPTS.Domain.Entities
{
    public class Doctor_Speciality_Mapping : BaseEntityWithDateTime
    {
        public string Doctor_Id { get; set; }
        public int Speciality_Id { get; set; }
    }
}
