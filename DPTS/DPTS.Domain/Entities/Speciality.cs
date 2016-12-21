using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPTS.Domain.Entities
{
    public class Speciality : BaseEntityWithDateTime
    {
        [Required(ErrorMessage = "Please enter {0}")]
        [MaxLength(256, ErrorMessage = "Maximum allowed character length for {0} is {1}")]
        public string Title { get; set; }

        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; } = 0;

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
