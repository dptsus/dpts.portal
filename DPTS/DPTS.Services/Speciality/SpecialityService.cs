using DPTS.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using DPTS.Domain.Entities;
using DPTS.Data.Context;

namespace DPTS.Services
{
    public class SpecialityService : ISpecialityService
    {
        #region Fields
        private DPTSDbContext _specialityRepository;
        #endregion

        #region Constructor
        public SpecialityService(DPTSDbContext specialityRepository)
        {
            _specialityRepository = specialityRepository;
        }
        #endregion

        public void AddSpeciality(Speciality speciality)
        {
            if (speciality == null)
                throw new ArgumentNullException(nameof(speciality));

            _specialityRepository.Speciality.Add(speciality);

            _specialityRepository.SaveChanges();
        }

        public void DeleteSpeciality(Speciality speciality)
        {
            if (speciality == null)
                throw new ArgumentNullException(nameof(speciality));

            _specialityRepository.Speciality.Remove(speciality);

        }

        public Speciality GetSpecialitybyId(int Id)
        {
            return _specialityRepository.Speciality.Find(Id);
        }

        public IList<Speciality> GetAllSpeciality(bool showhidden, bool enableTracking = false)
        {
            //var query = enableTracking ? _specialityRepository.Table : _specialityRepository.TableNoTracking;

            //if (!showhidden)
            //    query = query.Where(c => c.IsActive);

            //query = query.OrderBy(c => c.DisplayOrder);

            return _specialityRepository.Speciality.ToList();
        }

        public void UpdateSpeciality(Speciality data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            _specialityRepository.SaveChanges();
        }

    }
}

