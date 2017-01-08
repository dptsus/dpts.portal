using DPTS.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using DPTS.Domain;

namespace DPTS.Services
{
    public class AddressService : IAddressService
    {
        #region Fields
        private readonly IRepository<Address> _addressRepository;
        private readonly IRepository<AddressMapping> _addressMappingRepository;
        #endregion

        #region Constructor
        public AddressService(IRepository<Address> addressRepository, IRepository<AddressMapping> addressMappingRepository)
        {
            _addressRepository = addressRepository;
            _addressMappingRepository = addressMappingRepository;
        }
        #endregion

        #region Methods

        public void AddAddress(Address address)
        {
            if (address == null)
                throw new ArgumentNullException("address");

            _addressRepository.Insert(address);
        }

        public void DeleteAddress(Address address)
        {
            if (address == null)
                throw new ArgumentNullException("address");

            _addressRepository.Delete(address);
        }

        public Address GetAddressbyId(int Id)
        {
            if (Id == 0)
                return null;

            return _addressRepository.GetById(Id);
        }

        public IList<Address> GetAllAddress()
        {
            var query = _addressRepository.Table;

            return query.ToList(); ;
        }

        public IList<Address> GetAllAddressByUser(string UserId)
        {
            if (string.IsNullOrWhiteSpace(UserId))
                return null;

            var query = _addressMappingRepository.Table;
                query = query.Where(c => c.UserId == UserId);

            var lstAddr = new List<Address>();
            foreach (var addrMap in query.ToList())
            {
                var addr = _addressRepository.GetById(addrMap.AddressId);
                if (addr != null)
                    lstAddr.Add(addr);
            }
            return lstAddr;
        }
        public IList<Address> GetAllAddressByZipcode(string zipcode)
        {
            if (string.IsNullOrWhiteSpace(zipcode))
                return null;

            var addrs = _addressRepository.Table.Where(a => a.ZipPostalCode == zipcode).ToList();
            return addrs;
        }

        public void UpdateAddress(Address address)
        {
            if (address == null)
                throw new ArgumentNullException("address");

            _addressRepository.Update(address);
        }

        public void AddAddressMapping(AddressMapping addressMapping)
        {
            if (addressMapping == null)
                throw new ArgumentNullException("addressMapping");

            _addressMappingRepository.Insert(addressMapping);
        }

        public void DeleteAddressMapping(AddressMapping addressMapping)
        {
            if (addressMapping == null)
                throw new ArgumentNullException("addressMapping");

            _addressMappingRepository.Delete(addressMapping); //AddressMapping GetAddressMappingbId(int Id)
        }

        public AddressMapping GetAddressMappingbuUserIdAddrId(string UserId,int AddressId)
        {
            if (string.IsNullOrWhiteSpace(UserId) && AddressId == 0)
                return null;

            var query = _addressMappingRepository.Table;
                query = query.Where(c => c.UserId == UserId && c.AddressId == AddressId);

            return query.FirstOrDefault();
        }

        #endregion

    }
}
