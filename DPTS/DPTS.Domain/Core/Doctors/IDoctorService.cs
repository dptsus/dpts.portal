using DPTS.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DPTS.Domain.Core
{
    public interface IDoctorService
    {
        void AddDoctorAsync(Doctor Doctor);
        /// <summary>
        /// Get all Doctor
        /// </summary>
        Task<IEnumerable<Doctor>> GetAllDoctorAsync(bool showhidden, bool enableTracking = false);
        /// <summary>
        /// Get Doctor by Id
        /// </summary>
        Task<Doctor> GetDoctorbyIdAsync(int Id);
        /// <summary>
        /// Delete Doctor by Id
        /// </summary>
        Task DeleteDoctorAsync(Doctor Doctor);
        /// <summary>
        /// update Doctor
        /// </summary>
        void UpdateDoctorAsync(Doctor data);
    }
}
