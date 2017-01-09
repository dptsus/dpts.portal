using DPTS.Domain;
using System.Collections.Generic;

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

        void AddSpecialityByDoctor(SpecialityMapping doctorSpecilities);

        bool IsDoctorSpecialityExists(SpecialityMapping doctorSpecilities);
        /// <summary>
        /// Get specilities by doctor
        /// </summary>
        /// <param name="doctorId"></param>
        /// <returns></returns>
        IList<Speciality> GetDoctorSpecilities(string doctorId);
    }
}
