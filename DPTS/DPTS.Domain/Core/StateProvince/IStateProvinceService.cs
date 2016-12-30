using DPTS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPTS.Domain.Core
{
    public interface IStateProvinceService
    {
        /// <summary>
        /// Deletes a state
        /// </summary>
        /// <param name="state">state</param>
        void DeleteStateProvince(StateProvince state);

        /// <summary>
        /// Gets all countries
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>StateProvinces</returns>
        IList<StateProvince> GetAllStateProvince(bool showHidden = false);

        /// <summary>
        /// Gets a state
        /// </summary>
        /// <param name="stateProvinceId">state identifier</param>
        /// <returns>state</returns>
        StateProvince GetStateProvinceById(int stateProvinceId);

        /// <summary>
        /// Get countries by identifiers
        /// </summary>
        /// <param name="stateProvinceIds">state identifiers</param>
        /// <returns>Countries</returns>
        IList<StateProvince> GetStateProvinceByIds(int[] stateProvinceIds);

        /// <summary>
        /// Gets a state/province
        /// </summary>
        /// <param name="abbreviation">The state/province abbreviation</param>
        /// <returns>State/province</returns>
        StateProvince GetStateProvinceByAbbreviation(string abbreviation);

        /// <summary>
        /// Gets a state/province collection by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <param name="languageId">Language identifier. It's used to sort states by localized names (if specified); pass 0 to skip it</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>States</returns>
        IList<StateProvince> GetStateProvincesByCountryId(int countryId, bool showHidden = false);

        /// <summary>
        /// Inserts a state
        /// </summary>
        /// <param name="state">state</param>
        void InsertStateProvince(StateProvince state);

        /// <summary>
        /// Updates the state
        /// </summary>
        /// <param name="state">state</param>
        void UpdateStateProvince(StateProvince state);
    }
}
