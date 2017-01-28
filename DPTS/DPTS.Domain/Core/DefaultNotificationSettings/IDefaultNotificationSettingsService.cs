using System.Collections.Generic;

namespace DPTS.Domain.Core.DefaultNotificationSettings
{
    public interface IDefaultNotificationSettingsService
    {
        /// <summary>
        /// Deletes a defaultNotificationSettings
        /// </summary>
        /// <param name="defaultNotificationSettings">defaultNotificationSettings</param>
        void DeleteDefaultNotificationSettings(Entities.DefaultNotificationSettings defaultNotificationSettings);

        /// <summary>
        /// Gets all countries
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>defaultNotificationSettings</returns>
        IList<Entities.DefaultNotificationSettings> GetAllDefaultNotificationSettings(bool showHidden = false);

        /// <summary>
        /// Gets a defaultNotificationSettings
        /// </summary>
        /// <param name="defaultNotificationSettingsId">defaultNotificationSettings identifier</param>
        /// <returns>defaultNotificationSettings</returns>
        Entities.DefaultNotificationSettings GetDefaultNotificationSettingsById(int defaultNotificationSettingsId);

        /// <summary>
        /// Get countries by identifiers
        /// </summary>
        /// <param name="defaultNotificationSettingsIds">defaultNotificationSettings identifiers</param>
        /// <returns>Countries</returns>
        IList<Entities.DefaultNotificationSettings> GetDefaultNotificationSettingsByIds(int[] defaultNotificationSettingsIds);

        /// <summary>
        /// Gets a defaultNotificationSettings/
        /// </summary>
        /// <param name="abbreviation">The defaultNotificationSettings/ abbreviation</param>
        /// <returns>defaultNotificationSettings/</returns>
        Entities.DefaultNotificationSettings GetDefaultNotificationSettingsByAbbreviation(string abbreviation);

        /// <summary>
        /// Gets a defaultNotificationSettings/ collection by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <param name="languageId">Language identifier. It's used to sort defaultNotificationSettingss by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>defaultNotificationSettingss</returns>
        IList<Entities.DefaultNotificationSettings> GetDefaultNotificationSettingssByCountryId(int countryId, bool showHidden = false);

        /// <summary>
        /// Inserts a defaultNotificationSettings
        /// </summary>
        /// <param name="defaultNotificationSettings">defaultNotificationSettings</param>
        void InsertDefaultNotificationSettings(Entities.DefaultNotificationSettings defaultNotificationSettings);

        /// <summary>
        /// Updates the defaultNotificationSettings
        /// </summary>
        /// <param name="defaultNotificationSettings">defaultNotificationSettings</param>
        void UpdateDefaultNotificationSettings(Entities.DefaultNotificationSettings state);
    }
}
