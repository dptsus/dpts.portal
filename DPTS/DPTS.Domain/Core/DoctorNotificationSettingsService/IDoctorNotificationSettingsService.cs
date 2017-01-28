using System.Collections.Generic;

namespace DPTS.Domain.Core.DoctorNotificationSettingsService
{
    public interface IDoctorNotificationSettingsService
    {
        /// <summary>
        /// Deletes a doctorNotificationSettings
        /// </summary>
        /// <param name="doctorNotificationSettings">doctorNotificationSettings</param>
        void DeleteDoctorNotificationSettings(Entities.DoctorNotificationSettings doctorNotificationSettings);

        /// <summary>
        /// Gets all countries
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>doctorNotificationSettings</returns>
        IList<Entities.DoctorNotificationSettings> GetAllDoctorNotificationSettings(bool showHidden = false);

        /// <summary>
        /// Gets a doctorNotificationSettings
        /// </summary>
        /// <param name="doctorNotificationSettingsId">doctorNotificationSettings identifier</param>
        /// <returns>doctorNotificationSettings</returns>
        Entities.DoctorNotificationSettings GetDoctorNotificationSettingsById(int doctorNotificationSettingsId);

        /// <summary>
        /// Get countries by identifiers
        /// </summary>
        /// <param name="doctorNotificationSettingsIds">doctorNotificationSettings identifiers</param>
        /// <returns>Countries</returns>
        IList<Entities.DoctorNotificationSettings> GetDoctorNotificationSettingsByIds(int[] doctorNotificationSettingsIds);

        /// <summary>
        /// Gets a doctorNotificationSettings/
        /// </summary>
        /// <param name="abbreviation">The doctorNotificationSettings/ abbreviation</param>
        /// <returns>doctorNotificationSettings/</returns>
        Entities.DoctorNotificationSettings GetDoctorNotificationSettingsByAbbreviation(string abbreviation);

        /// <summary>
        /// Gets a doctorNotificationSettings/ collection by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <param name="languageId">Language identifier. It's used to sort doctorNotificationSettingss by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>doctorNotificationSettingss</returns>
        IList<Entities.DoctorNotificationSettings> GetDoctorNotificationSettingssByCountryId(int countryId, bool showHidden = false);

        /// <summary>
        /// Inserts a doctorNotificationSettings
        /// </summary>
        /// <param name="doctorNotificationSettings">doctorNotificationSettings</param>
        void InsertDoctorNotificationSettings(Entities.DoctorNotificationSettings doctorNotificationSettings);

        /// <summary>
        /// Updates the doctorNotificationSettings
        /// </summary>
        void UpdateDoctorNotificationSettings(Entities.DoctorNotificationSettings settings);
    }
}
