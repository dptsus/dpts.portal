using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DPTS.Web.Models
{
    public class VisitorContactDeatilsModel
    {
        public string Subject { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Note { get; set; }
        public string doctorId { get; set; }
    }
}