using DPTS.Data.Context;
using DPTS.Domain.Core;
using DPTS.Domain;
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
        private readonly IRepository<SpecialityMapping> _specialityMappingRepository;
        private readonly IRepository<AddressMapping> _addressMapping;
        private readonly IRepository<Address> _address;
        private readonly IAddressService _addressService;
        private readonly DPTSDbContext _context;

        #endregion

        #region Constructor
        public DoctorService(IRepository<Doctor> doctorRepository,
            IRepository<Speciality> specialityRepository,
            IRepository<SpecialityMapping> specialityMappingRepository,
            IAddressService addressService,
            IRepository<AddressMapping> addressMapping,
            IRepository<Address> address)
        {
            _doctorRepository = doctorRepository;
            _specialityRepository = specialityRepository;
            _specialityMappingRepository = specialityMappingRepository;
            _addressService = addressService;
            _addressMapping = addressMapping;
            _address = address;
            _context = new DPTSDbContext();
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
            return null;
        }
        public IList<Doctor> SearchDoctor(string keywords = null, int SpecialityId = 0, string directory_type = null,string zipcode = null)
        {
            var query = from d in _doctorRepository.Table
                        select d;

          //  query = query.Where(p => !p.Deleted && p.IsActive);

            if (string.IsNullOrWhiteSpace(directory_type) && directory_type != "doctor")
                return null;

            
            if(!string.IsNullOrWhiteSpace(zipcode))
            {
                query = from d in _context.Doctors
                        join a in _context.AddressMappings on d.DoctorId equals a.UserId
                        join m in _context.Addresses on a.AddressId equals m.Id
                        where m.ZipPostalCode == zipcode
                        select d;
             }
            if (SpecialityId > 0)
            {
                query = query.SelectMany(d => d.SpecialityMapping.Where(s => s.Speciality_Id.Equals(SpecialityId)), (d, s) => d);
            }
            if (!string.IsNullOrWhiteSpace(keywords))
            {
                query = query.Where(d => d.ShortProfile.Contains(keywords)
                         || d.Subscription.Contains(keywords) || d.Qualifications.Contains(keywords));
            }

            return query.ToList();

        }
        #endregion
    }
}
