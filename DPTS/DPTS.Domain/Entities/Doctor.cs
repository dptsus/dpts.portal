using System.ComponentModel.DataAnnotations;

namespace DPTS.Domain.Entities
{
    public class Doctor : BaseEntityWithDateTime
    {
        /// <summary>
        /// Get or set category name
        /// </summary>
        [Required, MaxLength(256)]
        public string Name { get; set; }

        /// <summary>
        /// Get or set IsActive
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Get or Set Display Order
        /// </summary>
        public int DisplayOrder { get; set; }

    }
}
