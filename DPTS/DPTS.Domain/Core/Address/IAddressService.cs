using DPTS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPTS.Domain.Core
{
    public interface IAddressService
    {
        /// <summary>
        /// Inserts an Address
        /// </summary>
        void AddAddress(Address address);

        /// <summary>
        /// Get Address by Id
        /// </summary>
        Address GetAddressbyId(int Id);

        /// <summary>
        /// Delete Addressr by Id
        /// </summary>
        void DeleteAddress(Address address);

        /// <summary>
        /// update Address
        /// </summary>
        void UpdateAddress(Address address);

        /// <summary>
        /// get list of Address
        /// </summary>
        IList<Address> GetAllAddress();

        /// <summary>
        /// Get all address by userId
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        IList<Address> GetAllAddressByUser(string UserId);

        /// <summary>
        /// Inserts an Address Mapping
        /// </summary>
        void AddAddressMapping(AddressMapping addressMapping);

        void DeleteAddressMapping(AddressMapping addressMapping);

        AddressMapping GetAddressMappingbuUserIdAddrId(string UserId, int AddressId);

        IList<Address> GetAllAddressByZipcode(string zipcode);

        IList<AddressMapping> GetAllAddressMapping();

    }
}
