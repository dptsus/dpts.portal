using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DPTS.Web.Models
{
    public class AppointmentScheduleViewModel
    {
        public AppointmentScheduleViewModel()
        {
            Morning = new List<MorningSlotModel>(); 
            Afternoon =new List<AfternoonSlotModel>();
            Evening =new List<EveningSlotModel>();
        }

        public IList<MorningSlotModel> Morning { get; set; }

        public IList<AfternoonSlotModel> Afternoon { get; set; }

        public IList<EveningSlotModel> Evening { get; set; }
        
    }
    public class MorningSlotModel
    {
        public string Slot { get; set; }
    }
    public class AfternoonSlotModel
    {
        public string Slot { get; set; }
    }
    public class EveningSlotModel
    {
        public string Slot { get; set; }
    }

}