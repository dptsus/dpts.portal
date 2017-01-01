using DPTS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPTS.Domain.Core
{
    /// <summary>
    /// Sub Speciality Service
    /// </summary>
    public interface ISubSpecialityService
    {
        /// <summary>
        /// Inserts an sub Speciality
        /// </summary>
        void AddSubSpeciality(SubSpeciality subSpeciality);

        /// <summary>
        /// Get sub Speciality by Id
        /// </summary>
        SubSpeciality GetSubSpecialitybyId(int Id);

        /// <summary>
        /// Delete sub Speciality by Id
        /// </summary>
        void DeleteSubSpeciality(SubSpeciality subSpeciality);

        /// <summary>
        /// update sub Speciality
        /// </summary>
        void UpdateSubSpeciality(SubSpeciality subSpeciality);

        /// <summary>
        /// get list of sub Speciality
        /// </summary>
        IList<SubSpeciality> GetAllSubSpeciality(bool showhidden, bool enableTracking = false);

    }
}
