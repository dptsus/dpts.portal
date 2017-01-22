using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;

namespace DPTS.Web.Models
{
    public class AppointmentScheduleViewModel
    {
        public AppointmentScheduleViewModel()
        {
            ScheduleSlotModel = new List<ScheduleSlotModel>();
        }

        public IList<ScheduleSlotModel> ScheduleSlotModel { get; set; }

    }
    public class ScheduleSlotModel
    {
        public string Slot { get; set; }
        public string BookedSlot { get; set; }
    }
}