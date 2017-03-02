using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DPTS.Web.Models
{
    public class DoctorPictureModel
    {
        public string DoctorId { get; set; }

        [UIHint("Picture")]
        [Display(Name = "Picture")]
        public int PictureId { get; set; }

        [Display(Name = "Picture Url")]
        public string PictureUrl { get; set; }

        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }
    }
}