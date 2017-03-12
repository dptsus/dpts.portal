using System;
using System.Collections.Generic;
using DPTS.Domain.Entities;

namespace DPTS.Domain.Core.Doctors
{
    public interface IJoinUsService
    {
        void AddDoctor(JoinUs doctor);
        IList<JoinUs> GetAllDoctors(int page, int itemsPerPage, out int totalCount);
    }
}
