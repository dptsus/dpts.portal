using DPTS.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPTS.Domain.Entities;
using DPTS.Data.Context;

namespace DPTS.Services
{
    public class SpecialityService : ISpecialityService
    {
        #region Fields
        private IRepository<Speciality> _specialityRepository;
        #endregion

        #region Constructor
        public SpecialityService(IRepository<Speciality> specialityRepository)
        {
            _specialityRepository = specialityRepository;
        }
        #endregion

        public void AddSpecialityAsync(Speciality speciality)
        {
            if (speciality == null)
                throw new ArgumentNullException(nameof(speciality));

            _specialityRepository.AddAsync(speciality);
        }

        public async Task DeleteSpecialityAsync(Speciality Speciality)
        {
            if (Speciality == null)
                throw new ArgumentNullException(nameof(Speciality));

            await _specialityRepository.RemoveAsync(Speciality);
        }

        public async Task<IEnumerable<Speciality>> GetAllSpecialityAsync(bool showhidden, bool enableTracking = false)
        {
            return showhidden ? await _specialityRepository.GetAllAsync(enableTracking)
                : await _specialityRepository.FindAsync(c => c.IsActive);
        }

        public async Task<Speciality> GetSpecialitybyIdAsync(int Id)
        {
            return await _specialityRepository.GetByIdAsync(Id);
        }

        public void UpdateSpecialityAsync(Speciality data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            _specialityRepository.UpdateAsync(data);
        }

    }
}

