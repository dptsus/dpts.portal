using DPTS.Domain.Core;
using DPTS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DPTS.Services
{
    public class DoctorService : IDoctorService
    {
        #region Fields
        private readonly IRepository<Doctor> _DoctorRepository;
        #endregion

        #region Constructor
        public DoctorService(IRepository<Doctor> DoctorRepository)
        {
            _DoctorRepository = DoctorRepository;
        }
        #endregion

        #region Methods
        public void AddDoctorAsync(Doctor Doctor)
        {
            if (Doctor == null)
                throw new ArgumentNullException(nameof(Doctor));

             _DoctorRepository.AddAsync(Doctor);
        }

        public async Task DeleteDoctorAsync(Doctor Doctor)
        {
            if (Doctor == null)
                throw new ArgumentNullException(nameof(Doctor));

            await _DoctorRepository.RemoveAsync(Doctor);
        }

        public async Task<IEnumerable<Doctor>> GetAllDoctorAsync(bool showhidden, bool enableTracking = false)
        {
            return showhidden ? await _DoctorRepository.GetAllAsync(enableTracking)
                : await _DoctorRepository.FindAsync(c => c.IsActive);
        }

        public async Task<Doctor> GetDoctorbyIdAsync(int Id)
        {
            return await _DoctorRepository.GetByIdAsync(Id);
        }

        public void UpdateDoctorAsync(Doctor data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

             _DoctorRepository.UpdateAsync(data);
        }
        #endregion
    }
}
