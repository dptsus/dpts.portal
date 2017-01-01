using DPTS.Domain.Entities;
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
    }
}
