using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DPTS.Web.Models
{
    public class SheduleViewModel
    {
        public string Sunday { get; set; }
        public string Monday { get; set; }
        public string Tuesday { get; set; }
        public string Wensday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }
        public string Saterday { get; set; }

        public DateTime Type { get; set; }
    }
}