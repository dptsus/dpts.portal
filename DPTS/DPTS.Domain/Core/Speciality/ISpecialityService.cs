using DPTS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPTS.Domain.Core
{
    public partial interface ISpecialityService
    {
        /// <summary>
        /// Inserts an Speciality
        /// </summary>
        void AddSpecialityAsync(Speciality speciality);

        /// <summary>
        /// update Speciality
        /// </summary>
        void UpdateSpecialityAsync(Speciality data);

        /// <summary>
        /// Get all Speciality
        /// </summary>
        Task<IEnumerable<Speciality>> GetAllSpecialityAsync(bool showhidden, bool enableTracking = false);
        /// <summary>
        /// Get Speciality by Id
        /// </summary>
        Task<Speciality> GetSpecialitybyIdAsync(int Id);
        /// <summary>
        /// Delete Doctor by Id
        /// </summary>
        Task DeleteSpecialityAsync(Speciality Doctor);
    }
}
