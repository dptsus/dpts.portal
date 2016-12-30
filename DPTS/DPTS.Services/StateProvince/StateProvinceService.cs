using DPTS.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPTS.Domain.Entities;

namespace DPTS.Services
{
    /// <summary>
    /// State Service
    /// </summary>
    public class StateProvinceService : IStateProvinceService
    {
        #region Fields

        private readonly IRepository<StateProvince> _stateProvinceRepository;

        #endregion

        #region Ctor
        public StateProvinceService(IRepository<StateProvince> stateProvinceRepository)
        {
            _stateProvinceRepository = stateProvinceRepository;
        }
        #endregion

        #region Methods
        /// <summary>
        /// delete state
        /// </summary>
        /// <param name="state"></param>
        public void DeleteStateProvince(StateProvince state)
        {
            if (state == null)
                throw new ArgumentNullException("StateProvince");

            _stateProvinceRepository.Delete(state);
        }
        /// <summary>
        /// Get all state
        /// </summary>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        public IList<StateProvince> GetAllStateProvince(bool showHidden = false)
        {
            var query = _stateProvinceRepository.Table;
            if (!showHidden)
                query = query.Where(c => c.Published);
            query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name);

            var countries = query.ToList();
            return countries;
        }
        /// <summary>
        /// get state by abbreviation
        /// </summary>
        /// <param name="abbreviation"></param>
        /// <returns></returns>
        public StateProvince GetStateProvinceByAbbreviation(string abbreviation)
        {
            var query = from sp in _stateProvinceRepository.Table
                        where sp.Abbreviation == abbreviation
                        select sp;
            var stateProvince = query.FirstOrDefault();
            return stateProvince;
        }

        /// <summary>
        /// get state by id
        /// </summary>
        /// <param name="stateProvinceId"></param>
        /// <returns></returns>
        public StateProvince GetStateProvinceById(int stateProvinceId)
        {
            if (stateProvinceId == 0)
                return null;

            return _stateProvinceRepository.GetById(stateProvinceId);
        }

        public IList<StateProvince> GetStateProvinceByIds(int[] stateProvinceIds)
        {
            if (stateProvinceIds == null || stateProvinceIds.Length == 0)
                return new List<StateProvince>();

            var query = from c in _stateProvinceRepository.Table
                        where stateProvinceIds.Contains(c.Id)
                        select c;
            var countries = query.ToList();
            //sort by passed identifiers
            var sortedCountries = new List<StateProvince>();
            foreach (int id in stateProvinceIds)
            {
                var country = countries.Find(x => x.Id == id);
                if (country != null)
                    sortedCountries.Add(country);
            }
            return sortedCountries;
        }

        public IList<StateProvince> GetStateProvincesByCountryId(int countryId, bool showHidden = false)
        {
            var query = from sp in _stateProvinceRepository.Table
                        orderby sp.DisplayOrder, sp.Name
                        where sp.CountryId == countryId &&
                        (showHidden || sp.Published)
                        select sp;

            return query.ToList();
        }

        public void InsertStateProvince(StateProvince state)
        {
            if (state == null)
                throw new ArgumentNullException("state");

            _stateProvinceRepository.Insert(state);
        }

        public void UpdateStateProvince(StateProvince state)
        {
            if (state == null)
                throw new ArgumentNullException("state");

            _stateProvinceRepository.Update(state);
        }
        #endregion

    }
}
