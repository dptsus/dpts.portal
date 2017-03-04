﻿using System;
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
    public class DoctorService : IDoctorService
    {
        #region Fields
        private readonly IRepository<Doctor> _doctorRepository;
        private readonly IRepository<Domain.Entities.Speciality> _specialityRepository;
        private readonly IRepository<SpecialityMapping> _specialityMappingRepository;
        private readonly IRepository<AddressMapping> _addressMapping;
        private readonly IRepository<Domain.Entities.Address> _address;
        private readonly IRepository<AppointmentSchedule> _appointmentScheduleRepository;
        private readonly IRepository<SocialLinkInformation> _socialLinksRepository;
        private readonly IRepository<HonorsAwards> _honorsAwardsRepository;
        private readonly IRepository<Education> _educationRepository;
        private readonly IRepository<Experience> _experienceRepository;
        private readonly IAddressService _addressService;
        private readonly DPTSDbContext _context;
        private readonly IRepository<PictureMapping> _pictureMapRepository;

        #endregion

        #region Constructor
        public DoctorService(IRepository<Doctor> doctorRepository,
            IRepository<Domain.Entities.Speciality> specialityRepository,
            IRepository<SpecialityMapping> specialityMappingRepository,
            IAddressService addressService,
            IRepository<AddressMapping> addressMapping,
            IRepository<Domain.Entities.Address> address,
            IRepository<AppointmentSchedule> appointmentScheduleRepository,
            IRepository<SocialLinkInformation> socialLinksRepository,
            IRepository<HonorsAwards> honorsAwardsRepository,
            IRepository<Education> educationRepository,
            IRepository<Experience> experienceRepository,
            IRepository<PictureMapping> pictureMapRepository)
        {
            _doctorRepository = doctorRepository;
            _specialityRepository = specialityRepository;
            _specialityMappingRepository = specialityMappingRepository;
            _addressService = addressService;
            _addressMapping = addressMapping;
            _address = address;
            _appointmentScheduleRepository = appointmentScheduleRepository;
            _socialLinksRepository = socialLinksRepository;
            _honorsAwardsRepository = honorsAwardsRepository;
            _educationRepository = educationRepository;
            _experienceRepository = experienceRepository;
            _context = new DPTSDbContext();
            _pictureMapRepository = pictureMapRepository;
        }
        #endregion

        #region Utilities
        private IList<ZipCodes> GetAllZipCodes()
        {
            return _addressService.GetAllZipCodes();
        }

        private ZipCodes GetByZipCode(string zipCode)
        {
            var zipCodes = GetAllZipCodes();
            return zipCodes.FirstOrDefault(c => c.ZipCode == zipCode);

        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(Deg2Rad(lat1)) * Math.Sin(Deg2Rad(lat2)) + Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) * Math.Cos(Deg2Rad(theta));
            dist = Math.Acos(dist);
            dist = Rad2Deg(dist);
            dist = CalculateDistanceByUnitType("Miles", dist);
            return dist;
        }

        private static double CalculateDistanceByUnitType(string unitType, double dist)
        {
            dist = dist * 60 * 1.1515;
            //if (unitType == DistanceUnitType.KiloMeters)
            //{
            //    dist *= 1.609344;
            //}
            return dist;
        }

        private static double Deg2Rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        private static double Rad2Deg(double rad)
        {
            return rad / Math.PI * 180.0;
        }

        private Dictionary<string, double> GetGeoCoordinate(string address)
        {
            Dictionary<string, double> dictionary = new Dictionary<string, double>();
            try
            {
                string requestUri = string.Format("http://maps.google.com/maps/api/geocode/xml?address={0}&sensor=false", address);
                var request = System.Net.WebRequest.Create(requestUri);
                var response = request.GetResponse();
                var xdoc = XDocument.Load(response.GetResponseStream());
                var result = xdoc.Element("GeocodeResponse").Element("result");
                if (result != null)
                {
                    var locationElement = result.Element("geometry").Element("location");
                    dictionary.Add("lat", Double.Parse(locationElement.Element("lat").Value));
                    dictionary.Add("lng", Double.Parse(locationElement.Element("lng").Value));
                }
            }
            catch (Exception ex)
            {
            }
            return dictionary;
        }

        private ZipCodes CalculateLatLngForZipCode(string zipcode)
        {
            var zipCodeLatLng = GetGeoCoordinate(zipcode);
            if (zipCodeLatLng.Count == 2)
            {
                //insert zipcode
                var zipCodeLocation = new ZipCodes
                {
                    ZipCode = zipcode,
                    Latitude = zipCodeLatLng["lat"],
                    Longitude = zipCodeLatLng["lng"]
                };

                _addressService
                    .InsertZipCode(zipCodeLocation);

                return zipCodeLocation;
            }

            return null;
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

        public Doctor GetDoctorbyId(string doctorId)
        {
            return _doctorRepository.Table.FirstOrDefault(d => d.DoctorId == doctorId);
        }

        public void UpdateDoctor(Doctor data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            _doctorRepository.Update(data);
        }

        public IList<string> GetDoctorsName(bool showhidden)
        {
            return null;
        }

        //public IList<Doctor> GetAllDoctors()
        //{
        //    var query = from d in _doctorRepository.Table
        //                select d;
        //    return query.ToList();
        //}

        public IList<Doctor> GetAllDoctors(int page, int itemsPerPage, out int totalCount)
        {

            var query = from d in _doctorRepository.Table
                select d;

            var doctors = (from d in _doctorRepository.Table
                         orderby d.DateUpdated descending
                         select d)
                        .Skip(itemsPerPage * page).Take(itemsPerPage)
                          .ToList();
            //foreach (var doc in docLst.Doctors)
            //{
            //    var address = from d in _address.Table
            //             join m in _addressMapping.Table on d.Id equals m.AddressId
            //             where m.UserId == doc.DoctorId
            //             select d;
            //    docLst.Address = address.FirstOrDefault();
            //}

            totalCount = query.Count();//return the number of pages
            return doctors;//the query is now already executed, it is a subset of all the orders.
        }

        public class TempAddressViewModel
        {
            public Address Address { get; set; }
            public double Distance { get; set; }
        }

        public IList<Doctor> SearchDoctor(int page, int itemsPerPage, out int totalCount,
            string zipcode = null,
            int specialityId = 0,
            double Geo_Distance = 50)
        {
            var query = from d in _doctorRepository.Table
                        select d;

            //  query = query.Where(p => !p.Deleted && p.IsActive);
            //  if (string.IsNullOrWhiteSpace(directoryType) && directoryType != "doctor")
            //  return null;

            //decimal myDec;
            //var result = decimal.TryParse(zipcode, out myDec);

           // if (!result)
            //{
                // zipcode = GetGeoZip(zipcode);
                var addrList = new List<TempAddressViewModel>();
                double lat = 0, lng = 0;
                #region (with zipcode)

                var firstZipCodeLocation = GetByZipCode(zipcode) ?? CalculateLatLngForZipCode(zipcode);

                var data = _addressService.GetAllAddress();
                foreach (var addr in data)
                {

                    if (addr.Latitude == 0 && addr.Longitude == 0)
                    {
                        var geoCoodrinateDealer = GetGeoCoordinate(addr.ZipPostalCode.Trim());
                        if (geoCoodrinateDealer.Count == 2)
                        {
                            lat = addr.Latitude = geoCoodrinateDealer["lat"];
                            lng = addr.Longitude = geoCoodrinateDealer["lng"];

                            _addressService.UpdateAddress(addr);
                        }
                    }
                    else
                    {
                        lat = addr.Latitude;
                        lng = addr.Longitude;
                    }

                    if (firstZipCodeLocation != null && lat != 0 && lng != 0)
                    {
                        var addrModel = new TempAddressViewModel
                        {
                            Address = addr,
                            Distance = CalculateDistance(firstZipCodeLocation.Latitude, firstZipCodeLocation.Longitude,
                                lat, lng)
                        };
                        addrList.Add(addrModel);
                    }
                }
                addrList = addrList.Where(c => c.Distance <= Convert.ToDouble(Geo_Distance)).OrderBy(c => c.Distance).ToList();

                if (addrList.Any())
                {
                    List<int> addrIds = addrList.Select(_address => _address.Address.Id).ToList();

                    query = from d in _context.Doctors
                            join a in _context.AddressMappings on d.DoctorId equals a.UserId
                            where addrIds.Contains(a.AddressId)//a.AddressId == zipcode
                            select d;
                }

                #endregion
           // }
            if (specialityId > 0)
            {
                //var spec =
                //        from s in _specialityRepository.Table
                //        join r in _specialityMappingRepository.Table on s.Id equals r.Speciality_Id
                //        where s.Title == searchTerm
                //        select s.Id;

                query = query.SelectMany(d => d.SpecialityMapping.Where(s => s.Speciality_Id.Equals(specialityId)), (d, s) => d);
            }

            var pageQuery =  query.OrderBy(d => d.DateUpdated)
                .Skip(itemsPerPage * page).Take(itemsPerPage)
                         .ToList();


            totalCount = query.Count();
            return pageQuery;
        }

        #region Social links
        public void InsertSocialLink(SocialLinkInformation link)
        {
            if (link == null)
                throw new ArgumentNullException(nameof(link));

            _socialLinksRepository.Insert(link);
        }

        public SocialLinkInformation GetSocialLinkbyId(int id)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(id));

           return  _socialLinksRepository.GetById(id);
        }

        public void DeleteSocialLink(SocialLinkInformation link)
        {
            if (link == null)
                throw new ArgumentNullException(nameof(link));

            _socialLinksRepository.Delete(link);
        }

        public void UpdateSocialLink(SocialLinkInformation link)
        {
            if (link == null)
                throw new ArgumentNullException(nameof(link));

            _socialLinksRepository.Update(link);
        }

        public IPagedList<SocialLinkInformation> GetAllLinksByDoctor(string doctorId, int pageIndex = 0,
            int pageSize = Int32.MaxValue, bool showHidden = false)
        {
           // return _socialLinksRepository.Table.Where(d => d.DoctorId == doctorId).ToList();
            var query = _socialLinksRepository.Table;
            if (!showHidden)
                query = query.Where(c => c.DoctorId == doctorId);
            query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.SocialLink);

            query = query.OrderBy(c => c.DisplayOrder);
            return new PagedList<SocialLinkInformation>(query, pageIndex, pageSize);
        }
        #endregion

        #region HonorsAwards

        public void InsertHonorsAwards(HonorsAwards award)
        {
            if (award == null)
                throw new ArgumentNullException(nameof(award));

            _honorsAwardsRepository.Insert(award);
        }

        public HonorsAwards GetHonorsAwardsbyId(int id)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(id));

            return _honorsAwardsRepository.GetById(id);
        }

        public void DeleteHonorsAwards(HonorsAwards award)
        {
            if (award == null)
                throw new ArgumentNullException(nameof(award));

            _honorsAwardsRepository.Delete(award);
        }

        public void UpdateHonorsAwards(HonorsAwards award)
        {
            if (award == null)
                throw new ArgumentNullException(nameof(award));

            _honorsAwardsRepository.Update(award);
        }

        public IPagedList<HonorsAwards> GetAllHonorsAwards(string doctorId, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _honorsAwardsRepository.Table;
            if (!showHidden)
                query = query.Where(c => c.DoctorId == doctorId);
            query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name);

            query = query.OrderBy(c => c.DisplayOrder);
            return new PagedList<HonorsAwards>(query, pageIndex, pageSize);
        }


        #endregion

        #region Education
        public void InsertEducation(Education education)
        {
            if (education == null)
                throw new ArgumentNullException(nameof(education));

            _educationRepository.Insert(education);
        }

        public Education GetEducationbyId(int id)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(id));

            return _educationRepository.GetById(id);
        }

        public void DeleteEducation(Education education)
        {
            if (education == null)
                throw new ArgumentNullException(nameof(education));

            _educationRepository.Delete(education);
        }

        public void UpdateEducation(Education education)
        {
            if (education == null)
                throw new ArgumentNullException(nameof(education));

            _educationRepository.Update(education);
        }

        public IPagedList<Education> GetAllEducation(string doctorId, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _educationRepository.Table;
            if (!showHidden)
                query = query.Where(c => c.DoctorId == doctorId);
            query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Title);

            query = query.OrderBy(c => c.DisplayOrder);
            return new PagedList<Education>(query, pageIndex, pageSize);
        }
        #endregion

        #region Experience

        public void InsertExperience(Experience experience)
        {
            if (experience == null)
                throw new ArgumentNullException(nameof(experience));

            _experienceRepository.Insert(experience);
        }

        public Experience GetExperiencebyId(int id)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(id));

            return _experienceRepository.GetById(id);
        }

        public void DeleteExperience(Experience experience)
        {
            if (experience == null)
                throw new ArgumentNullException(nameof(experience));

            _experienceRepository.Delete(experience);
        }

        public void UpdateExperience(Experience experience)
        {
            if (experience == null)
                throw new ArgumentNullException(nameof(experience));

            _experienceRepository.Update(experience);
        }

        public IPagedList<Experience> GetAllExperience(string doctorId, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
        {
            var query = _experienceRepository.Table;
            if (!showHidden)
                query = query.Where(c => c.DoctorId == doctorId);
            query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Title);

            query = query.OrderBy(c => c.DisplayOrder);
            return new PagedList<Experience>(query, pageIndex, pageSize);
        }
        #endregion

        #region Picture
        public virtual void InsertDoctorPicture(PictureMapping docPicture)
        {
            if (docPicture == null)
                throw new ArgumentNullException("productPicture");

            _pictureMapRepository.Insert(docPicture);
        }

        public virtual IList<PictureMapping> GetDoctorPicturesByDoctorId(string doctorId)
        {
            var query = from pp in _pictureMapRepository.Table
                        where pp.UserId == doctorId
                        orderby pp.DisplayOrder
                        select pp;
            var docPictures = query.ToList();
            return docPictures;
        }

        public virtual PictureMapping GetDoctorPictureById(int doctorPictureId)
        {
            if (doctorPictureId == 0)
                return null;

            return _pictureMapRepository.GetById(doctorPictureId);
        }

        public virtual void UpdateDoctorPicture(PictureMapping docPicture)
        {
            if (docPicture == null)
                throw new ArgumentNullException("docPicture");

            _pictureMapRepository.Update(docPicture);
        }
        public virtual void DeleteDoctorPicture(PictureMapping docPicture)
        {
            if (docPicture == null)
                throw new ArgumentNullException("docPicture");

            _pictureMapRepository.Delete(docPicture);
        }
        #endregion

        #endregion
    }
}
