using DPTS.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPTS.Domain.Entities;

namespace DPTS.Services
{
    /// <summary>
    /// Sub Speci service
    /// </summary>
    public class SubSpecialityService : ISubSpecialityService
    {
        #region Fields
        private readonly IRepository<SubSpeciality> _subSpecialityRepository;
        #endregion

        #region Constructor
        public SubSpecialityService(IRepository<SubSpeciality> subSpecialityRepository)
        {
            _subSpecialityRepository = subSpecialityRepository;
        }
        #endregion
        public void AddSubSpeciality(SubSpeciality subSpeciality)
        {
            if (subSpeciality == null)
                throw new ArgumentNullException("subSpeciality");

            _subSpecialityRepository.Insert(subSpeciality);
        }

        public void DeleteSubSpeciality(SubSpeciality subSpeciality)
        {
            if (subSpeciality == null)
                throw new ArgumentNullException("subSpeciality");

            _subSpecialityRepository.Delete(subSpeciality);
        }

        public SubSpeciality GetSubSpecialitybyId(int Id)
        {
            if (Id == 0)
                return null;

            return _subSpecialityRepository.GetById(Id);
        }

        public void UpdateSubSpeciality(SubSpeciality subSpeciality)
        {
            if (subSpeciality == null)
                throw new ArgumentNullException("subSpeciality");


            _subSpecialityRepository.Update(subSpeciality);
        }

        public IList<SubSpeciality> GetAllSubSpeciality(bool showhidden, bool enableTracking = false)
        {
            var query = _subSpecialityRepository.Table;
            if (!showhidden)
                query = query.Where(c => c.IsActive);
            query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name);

            var subSpecilities = query.ToList();
            return subSpecilities;
        }
    }
}
