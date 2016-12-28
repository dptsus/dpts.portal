using DPTS.Data.Context;
using DPTS.Domain.Core;
using DPTS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DPTS.Services
{
    public class DoctorService : IDoctorService
    {
        #region Fields
        private readonly IRepository<Doctor> _doctorRepository;
        private DPTSDbContext _specialityRepository;
        #endregion

        #region Constructor
        public DoctorService(IRepository<Doctor> doctorRepository, DPTSDbContext specialityRepository)
        {
            _doctorRepository = doctorRepository;
            _specialityRepository = specialityRepository;
        }
        #endregion

        #region Methods
        public void AddDoctor(Doctor doctor)
        {
            if (doctor == null)
                throw new ArgumentNullException(nameof(doctor));

            _doctorRepository.Insert(doctor);
        }

        public void DeleteDoctor(Doctor doctor)
        {
            if (doctor == null)
                throw new ArgumentNullException(nameof(doctor));

            _doctorRepository.Delete(doctor);
        }


        public Doctor GetDoctorbyId(string DoctorId)
        {
            return _doctorRepository.Table.FirstOrDefault(d => d.DoctorId == DoctorId);
        }

        public void UpdateDoctor(Doctor data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            _doctorRepository.Update(data);
        }
        public IList<string> GetDoctorsName(bool showhidden)
        {
            //var query = _doctorRepository.Table;
            //if (!showhidden)
            //    query = query.Where(c => c.IsActive);
            //return query.Select(c => c.Name).ToList();
            return null;
        }

        #endregion
    }
}
