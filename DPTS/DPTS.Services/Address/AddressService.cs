using DPTS.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPTS.Domain.Entities;

namespace DPTS.Services
{
    public class AddressService : IAddressService
    {
        #region Fields
        private readonly IRepository<Address> _addressRepository;
        #endregion

        #region Constructor
        public AddressService(IRepository<Address> addressRepository)
        {
            _addressRepository = addressRepository;
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

        //public IList<Address> GetAllAddressByUser(int Id)
        //{
        //    if (Id == 0)
        //        return null;

        //    var query = _addressRepository.Table;
        //        query = query.Where(c => c.);
        //    query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Title);

        //    var Specilities = query.ToList();
        //    return Specilities;
        //}

        public void UpdateAddress(Address address)
        {
            if (address == null)
                throw new ArgumentNullException("address");

            _addressRepository.Update(address);
        }
        #endregion

    }
}
