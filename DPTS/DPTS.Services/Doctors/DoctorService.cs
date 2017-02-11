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
    public class DoctorService : IDoctorService
    {
        #region Fields
        private readonly IRepository<Doctor> _doctorRepository;
        private readonly IRepository<Domain.Entities.Speciality> _specialityRepository;
        private readonly IRepository<SpecialityMapping> _specialityMappingRepository;
        private readonly IRepository<AddressMapping> _addressMapping;
        private readonly IRepository<Domain.Entities.Address> _address;
        private readonly IRepository<AppointmentSchedule> _appointmentScheduleRepository;
        private readonly IAddressService _addressService;
        private readonly DPTSDbContext _context;

        #endregion

        #region Constructor
        public DoctorService(IRepository<Doctor> doctorRepository,
            IRepository<Domain.Entities.Speciality> specialityRepository,
            IRepository<SpecialityMapping> specialityMappingRepository,
            IAddressService addressService,
            IRepository<AddressMapping> addressMapping,
            IRepository<Domain.Entities.Address> address,
            IRepository<AppointmentSchedule> appointmentScheduleRepository)
        {
            _doctorRepository = doctorRepository;
            _specialityRepository = specialityRepository;
            _specialityMappingRepository = specialityMappingRepository;
            _addressService = addressService;
            _addressMapping = addressMapping;
            _address = address;
            _appointmentScheduleRepository = appointmentScheduleRepository;
            _context = new DPTSDbContext();
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
                query = query.SelectMany(d => d.SpecialityMapping.Where(s => s.Speciality_Id.Equals(specialityId)), (d, s) => d);
            }

            var pageQuery =  query.OrderBy(d => d.DateUpdated)
                .Skip(itemsPerPage * page).Take(itemsPerPage)
                         .ToList();


            totalCount = query.Count();
            return pageQuery;
        }

        #endregion
    }
}
