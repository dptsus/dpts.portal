using DPTS.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using DPTS.Domain;
using DPTS.Data.Context;

namespace DPTS.Services
{
    public class SpecialityService : ISpecialityService
    {
        #region Fields
        private readonly IRepository<Speciality> _specialityRepository;
        private readonly IRepository<SpecialityMapping> _specalityMappingRepos;
        #endregion

        #region Constructor
        public SpecialityService(IRepository<Speciality> specialityRepository, IRepository<SpecialityMapping> specalityMappingRepos)
        {
            _specialityRepository = specialityRepository;
            _specalityMappingRepos = specalityMappingRepos;
        }
        #endregion

        public void AddSpeciality(Speciality speciality)
        {
            if (speciality == null)
                throw new ArgumentNullException("speciality");

            _specialityRepository.Insert(speciality);

        }

        public void DeleteSpeciality(Speciality speciality)
        {
            if (speciality == null)
                throw new ArgumentNullException("speciality");

            _specialityRepository.Delete(speciality);

        }

        public Speciality GetSpecialitybyId(int Id)
        {
            if (Id == 0)
                return null;

            return _specialityRepository.GetById(Id);
        }

        public IList<Speciality> GetAllSpeciality(bool showhidden, bool enableTracking = false)
        {
            var query = _specialityRepository.Table;
            if (!showhidden)
                query = query.Where(c => c.IsActive);
            query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Title);

            var Specilities = query.ToList();
            return Specilities;
        }

        public void UpdateSpeciality(Speciality data)
        {
            if (data == null)
                throw new ArgumentNullException("data");


            _specialityRepository.Update(data);
        }

        public void AddSpecialityByDoctor(SpecialityMapping doctorSpeciality)
        {
            if (doctorSpeciality == null)
                throw new ArgumentNullException("doctorSpeciality");

            doctorSpeciality.DateCreated = DateTime.UtcNow;
            doctorSpeciality.DateUpdated = DateTime.UtcNow;
            _specalityMappingRepos.Insert(doctorSpeciality);

        }
        public bool IsDoctorSpecialityExists(SpecialityMapping doctorSpeciality)
        {
            if (doctorSpeciality == null)
                throw new ArgumentNullException("doctorSpeciality");
            try
            {

                var data = _specalityMappingRepos.Table.FirstOrDefault(c => c.Doctor_Id == doctorSpeciality.Doctor_Id && c.Speciality_Id == doctorSpeciality.Speciality_Id);
                if (data != null)
                    return true;
            }
            catch { return false; }
            return false;

        }

    }
}

