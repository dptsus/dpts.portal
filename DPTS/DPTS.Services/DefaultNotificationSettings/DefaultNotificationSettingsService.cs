using System;
using System.Collections.Generic;
using System.Linq;
using DPTS.Domain.Core;
using DPTS.Domain.Core.DefaultNotificationSettings;
using DPTS.Domain.Entities;

namespace DPTS.Services.DefaultNotificationSettings
{
    /// <summary>
    /// DefaultNotificationSettings Service
    /// </summary>
    public class DefaultNotificationSettingsService : IDefaultNotificationSettingsService
    {
        #region Fields

        private readonly IRepository<Domain.Entities.DefaultNotificationSettings> _stateProvinceRepository;

        #endregion

        #region Ctor
        public DefaultNotificationSettingsService(IRepository<Domain.Entities.DefaultNotificationSettings> stateProvinceRepository)
        {
            _stateProvinceRepository = stateProvinceRepository;
        }
        #endregion

        #region Methods
        /// <summary>
        /// delete state
        /// </summary>
        /// <param name="state"></param>
        public void DeleteStateProvince(Domain.Entities.DefaultNotificationSettings state)
        {
            if (state == null)
                throw new ArgumentNullException("defaultNotificationSettings");

            _stateProvinceRepository.Delete(state);
        }
        /// <summary>
        /// Get all state
        /// </summary>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        public IList<Domain.Entities.DefaultNotificationSettings> GetAllStateProvince(bool showHidden = false)
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
        public Domain.Entities.DefaultNotificationSettings GetStateProvinceByAbbreviation(string abbreviation)
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
        public Domain.Entities.DefaultNotificationSettings GetStateProvinceById(int stateProvinceId)
        {
            if (stateProvinceId == 0)
                return null;

            return _stateProvinceRepository.GetById(stateProvinceId);
        }

        public IList<Domain.Entities.DefaultNotificationSettings> GetStateProvinceByIds(int[] stateProvinceIds)
        {
            if (stateProvinceIds == null || stateProvinceIds.Length == 0)
                return new List<Domain.Entities.DefaultNotificationSettings>();

            var query = from c in _stateProvinceRepository.Table
                        where stateProvinceIds.Contains(c.Id)
                        select c;
            var countries = query.ToList();
            //sort by passed identifiers
            var sortedCountries = new List<Domain.Entities.DefaultNotificationSettings>();
            foreach (int id in stateProvinceIds)
            {
                var country = countries.Find(x => x.Id == id);
                if (country != null)
                    sortedCountries.Add(country);
            }
            return sortedCountries;
        }

        public IList<Domain.Entities.DefaultNotificationSettings> GetStateProvincesByCountryId(int countryId, bool showHidden = false)
        {
            var query = from sp in _stateProvinceRepository.Table
                        orderby sp.DisplayOrder, sp.Name
                        where sp.CountryId == countryId &&
                        (showHidden || sp.Published)
                        select sp;

            return query.ToList();
        }

        public void InsertStateProvince(Domain.Entities.DefaultNotificationSettings state)
        {
            if (state == null)
                throw new ArgumentNullException("DefaultNotificationSettings");

            _stateProvinceRepository.Insert(state);
        }

        public void UpdateStateProvince(Domain.Entities.DefaultNotificationSettings state)
        {
            if (state == null)
                throw new ArgumentNullException("DefaultNotificationSettings");

            _stateProvinceRepository.Update(state);
        }
         
        #endregion

    }
}
