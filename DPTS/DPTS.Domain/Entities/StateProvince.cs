namespace DPTS.Domain.Entities
{
    public class StateProvince : BaseEntityWithDateTime
    {
        public int CountryId { get; set; }

        public string Name { get; set; }

        public string Abbreviation { get; set; }

        public bool Published { get; set; }

        public int DisplayOrder { get; set; }

        public virtual Country Country { get; set; }
    }
}
