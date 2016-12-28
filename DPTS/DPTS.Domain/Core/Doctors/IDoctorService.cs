using DPTS.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DPTS.Domain.Core
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
        Doctor GetDoctorbyId(string DoctorId);

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
    }
}
