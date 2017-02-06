using System.Collections.Generic;

namespace DPTS.Web.Models
{
    public class AppointmentScheduleViewModel
    {
        public AppointmentScheduleViewModel()
        {
            ScheduleSlotModel = new List<ScheduleSlotModel>();
        }

        public IList<ScheduleSlotModel> ScheduleSlotModel { get; set; }

        public string doctorId { get; set; }

    }
    public class ScheduleSlotModel
    {
        public string Slot { get; set; }
        public bool IsBooked { get; set; }
    }
}