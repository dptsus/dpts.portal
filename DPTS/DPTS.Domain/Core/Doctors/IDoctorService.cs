using System.Collections.Generic;
using DPTS.Domain.Entities;

namespace DPTS.Domain.Core.Doctors
{
    public interface IDoctorService
    {
        /// <summary>
        /// Inserts an Doctor account
        /// </summary>
        void AddDoctor(Doctor doctor);

        /// <summary>
        /// Get Doctor by Id
        /// </summary>
        Doctor GetDoctorbyId(string doctorId);

        /// <summary>
        /// Delete Doctor by Id
        /// </summary>
        void DeleteDoctor(Doctor Doctor);

        /// <summary>
        /// update catDoctoregory
        /// </summary>
        void UpdateDoctor(Doctor data);

        /// <summary>
        /// get list of Doctor Name
        /// </summary>
        IList<string> GetDoctorsName(bool showhidden);
        /// <summary>
        /// Search
        /// </summary>
        /// <param name="zipcode"></param>
        /// <returns></returns>
        IList<Doctor> SearchDoctor(string zipcode = null);

        /// <summary>
        /// Paging with get all doctors
        /// </summary>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        IList<Doctor> GetAllDoctors(int page, int itemsPerPage, out int totalCount);
    }
}
