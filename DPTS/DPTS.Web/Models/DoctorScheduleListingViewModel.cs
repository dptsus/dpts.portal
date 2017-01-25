using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DPTS.Domain.Entities;

namespace DPTS.Web.Models
{
    public class DoctorScheduleListingViewModel
    {
        public DoctorScheduleListingViewModel()
        {
            AppointmentSchedule = new List<AppointmentSchedule>();
        }

        public IList<AppointmentSchedule> AppointmentSchedule { get; set; }

    }
}