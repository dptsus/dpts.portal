using System;
using System.Collections.Generic;
using System.Linq;
using DPTS.Domain.Core;
using DPTS.Domain.Core.DoctorNotificationSettingsService;

namespace DPTS.Services.DoctorNotificationSettingsService
{
    /// <summary>
    /// DoctorNotificationSettings Service
    /// </summary>
    public class DoctorNotificationSettingsService : IDoctorNotificationSettingsService
    {
        #region Fields

        private readonly IRepository<Domain.Entities.DoctorNotificationSettings> _doctorNotificationSettingsRepository;

        #endregion

        #region Ctor
        public DoctorNotificationSettingsService(IRepository<Domain.Entities.DoctorNotificationSettings> doctorNotificationSettingsRepository)
        {
            _doctorNotificationSettingsRepository = doctorNotificationSettingsRepository;
        }
        #endregion

        #region Methods
        /// <summary>
        /// delete doctorNotificationSettings
        /// </summary>
        /// <param name="doctorNotificationSettings"></param>
        public void DeleteDoctorNotificationSettings(Domain.Entities.DoctorNotificationSettings doctorNotificationSettings)
        {
            if (doctorNotificationSettings == null)
                throw new ArgumentNullException("doctorNotificationSettings");

            _doctorNotificationSettingsRepository.Delete(doctorNotificationSettings);
        }
        /// <summary>
        /// Get all doctorNotificationSettings
        /// </summary>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        public IList<Domain.Entities.DoctorNotificationSettings> GetAllDoctorNotificationSettings(bool showHidden = false)
        {
            var query = _doctorNotificationSettingsRepository.Table;
            if (!showHidden)
                query = query.Where(c => c.Published);
            query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name);

            var countries = query.ToList();
            return countries;
        }
        /// <summary>
        /// get doctorNotificationSettings by abbreviation
        /// </summary>
        /// <param name="abbreviation"></param>
        /// <returns></returns>
        public Domain.Entities.DoctorNotificationSettings GetDoctorNotificationSettingsByAbbreviation(string abbreviation)
        {
            var query = from sp in _doctorNotificationSettingsRepository.Table
                        where sp.Message == abbreviation
                        select sp;
            var doctorNotificationSettings = query.FirstOrDefault();
            return doctorNotificationSettings;
        }

        /// <summary>
        /// get doctorNotificationSettings by id
        /// </summary>
        /// <param name="doctorNotificationSettingsId"></param>
        /// <returns></returns>
        public Domain.Entities.DoctorNotificationSettings GetDoctorNotificationSettingsById(int doctorNotificationSettingsId)
        {
            if (doctorNotificationSettingsId == 0)
                return null;

            return _doctorNotificationSettingsRepository.GetById(doctorNotificationSettingsId);
        }

        public IList<Domain.Entities.DoctorNotificationSettings> GetDoctorNotificationSettingsByIds(int[] doctorNotificationSettingsIds)
        {
            if (doctorNotificationSettingsIds == null || doctorNotificationSettingsIds.Length == 0)
                return new List<Domain.Entities.DoctorNotificationSettings>();

            var query = from c in _doctorNotificationSettingsRepository.Table
                        where doctorNotificationSettingsIds.Contains(c.Id)
                        select c;
            var countries = query.ToList();
            //sort by passed identifiers
            var sortedCountries = new List<Domain.Entities.DoctorNotificationSettings>();
            foreach (int id in doctorNotificationSettingsIds)
            {
                var country = countries.Find(x => x.Id == id);
                if (country != null)
                    sortedCountries.Add(country);
            }
            return sortedCountries;
        }

        public IList<Domain.Entities.DoctorNotificationSettings> GetDoctorNotificationSettingssByCountryId(int countryId, bool showHidden = false)
        {
            var query = from sp in _doctorNotificationSettingsRepository.Table
                        orderby sp.DisplayOrder, sp.Name
                        where sp.CategoryId == countryId &&
                        (showHidden || sp.Published)
                        select sp;

            return query.ToList();
        }

        public void InsertDoctorNotificationSettings(Domain.Entities.DoctorNotificationSettings doctorNotificationSettings)
        {
            if (doctorNotificationSettings == null)
                throw new ArgumentNullException("doctorNotificationSettings");

            _doctorNotificationSettingsRepository.Insert(doctorNotificationSettings);
        }

        public void UpdateDoctorNotificationSettings(Domain.Entities.DoctorNotificationSettings doctorNotificationSettings)
        {
            if (doctorNotificationSettings == null)
                throw new ArgumentNullException("doctorNotificationSettings");

            _doctorNotificationSettingsRepository.Update(doctorNotificationSettings);
        }
         
        #endregion

    }
}
