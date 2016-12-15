using DPTS.Domain.Core;
using DPTS.Domain.Core.Doctors;
using DPTS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DPTS.Services.Doctors
{
    public class DoctorService : IDoctorService
    {
        #region Fields
        private readonly IRepository<Doctor> _doctorRepository;
        #endregion

        #region Constructor
        public DoctorService(IRepository<Doctor> doctorRepository)
        {
            _doctorRepository = doctorRepository;
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


        public Doctor GetDoctorbyId(int Id)
        {
            return _doctorRepository.GetById(Id);
        }

        public void UpdateDoctor(Doctor data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

             _doctorRepository.Update(data);
        }
        public IList<string> GetDoctorsName(bool showhidden)
        {
            var query = _doctorRepository.Table;
            if (!showhidden)
                query = query.Where(c => c.IsActive);
            return query.Select(c => c.Name).ToList();
        }

        #endregion
    }
}
