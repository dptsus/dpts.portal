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
        private readonly IRepository<Speciality> _specialityRepository;
        private readonly IRepository<Doctor_Speciality_Mapping> _specialityMappingRepository;
        #endregion

        #region Constructor
        public DoctorService(IRepository<Doctor> doctorRepository,
            IRepository<Speciality> specialityRepository,
            IRepository<Doctor_Speciality_Mapping> specialityMappingRepository)
        {
            _doctorRepository = doctorRepository;
            _specialityRepository = specialityRepository;
            _specialityMappingRepository = specialityMappingRepository;
        }
        #endregion

        #region Methods
        public void AddDoctor(Doctor doctor)
        {
            if (doctor == null)
                throw new ArgumentNullException("doctor");

            _doctorRepository.Insert(doctor);
        }

        public void DeleteDoctor(Doctor doctor)
        {
            if (doctor == null)
                throw new ArgumentNullException("doctor");

            _doctorRepository.Delete(doctor);
        }


        public Doctor GetDoctorbyId(string DoctorId)
        {
            return _doctorRepository.Table.FirstOrDefault(d => d.DoctorId == DoctorId);
        }

        public void UpdateDoctor(Doctor data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

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
        public IList<Doctor> SearchDoctor(string keywords,int SpecialityId)
        {
                var lst = new List<Doctor>();

                var query = _doctorRepository.Table;
                    query = query.Where(p => !p.Deleted && p.IsActive);

                if (!string.IsNullOrWhiteSpace(keywords))
                {
                    query = from p in query
                            where (p.Expertise.Contains(keywords) ||
                            p.RegistrationNumber.Contains(keywords) ||
                            p.ShortProfile.Contains(keywords) ||
                            p.Subscription.Contains(keywords))
                            select p;
                }
                if(SpecialityId != 0)
                {
                     var td =
                             from s in _specialityMappingRepository.Table.Where(c => c.Speciality_Id == SpecialityId)
                             select s;

                    foreach(var a in td.ToList())
                    {
                            var doc = GetDoctorbyId(a.Doctor_Id);
                            if (doc != null)
                                lst.Add(doc);
                    }
               }

                return lst;

        }
        #endregion
    }
}
