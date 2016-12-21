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
        void AddSpeciality(Speciality speciality);

        /// <summary>
        /// Get Speciality by Id
        /// </summary>
        Speciality GetSpecialitybyId(int Id);

        /// <summary>
        /// Delete Specialityr by Id
        /// </summary>
        void DeleteSpeciality(Speciality speciality);

        /// <summary>
        /// update Speciality
        /// </summary>
        void UpdateSpeciality(Speciality data);

        /// <summary>
        /// get list of Speciality
        /// </summary>
        IList<Speciality> GetAllSpeciality(bool showhidden, bool enableTracking = false);
    }
}
