using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DPTS.Data.Context;
using DPTS.Domain.Core;
using DPTS.Domain.Core.Address;
using DPTS.Domain.Core.Doctors;
using DPTS.Domain.Entities;

namespace DPTS.Services.Doctors
{
    public class JoinUsService : IJoinUsService
    {
        #region Fields
        private readonly IRepository<JoinUs> _joinUsRepository;
        
        #endregion

        #region Constructor
        public JoinUsService(IRepository<JoinUs> JoinUsRepository )
        {
            _joinUsRepository = JoinUsRepository; 
        }
        #endregion 
        #region Methods
        public void AddDoctor(JoinUs doctor)
        {
            if (doctor == null)
                throw new ArgumentNullException(nameof(doctor));

            _joinUsRepository.Insert(doctor);
        } 
        public IList<JoinUs> GetAllDoctors(int page, int itemsPerPage, out int totalCount)
        {

            //var query = from d in _doctorRepository.Table
            //    select d;

            //var doctors = (from d in _doctorRepository.Table
            //             orderby d.DateUpdated descending
            //             select d)
            //            .Skip(itemsPerPage * page).Take(itemsPerPage)
            //              .ToList();

            //totalCount = query.Count();
            //return doctors; 
            totalCount = 0;
            return null;
        }
  
        #endregion
    }
}
