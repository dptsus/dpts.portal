namespace DPTS.Domain
{
    using System.ComponentModel.DataAnnotations;

    public partial class AddressMapping
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        [Required]
        public int AddressId { get; set; }

        public virtual Address Address { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }
    }
}
