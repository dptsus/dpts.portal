﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DPTS.Domain.Entities;

namespace DPTS.Web.Models
{
    public class TempDoctorViewModel
    {
        public TempDoctorViewModel()
        {
            Doctors = new Doctor();
            Address = new Address();
        }
        public Doctor Doctors { get; set; }
        public Domain.Entities.Address Address { get; set; }
    }
}