using System;
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
        /// Get Doctor by id
        /// </summary>
        Doctor GetDoctorbyId(string doctorId);

        /// <summary>
        /// Delete Doctor by id
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
        /// search doctor
        /// </summary>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="totalCount"></param>
        /// <param name="zipcode"></param>
        /// <returns></returns>
        IList<Doctor> SearchDoctor(int page, int itemsPerPage, out int totalCount, string zipcode = null,int specialityId = 0, double Geo_Distance = 50);

        /// <summary>
        /// Paging with get all doctors
        /// </summary>
        /// <param name="page"></param>
        /// <param name="itemsPerPage"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        IList<Doctor> GetAllDoctors(int page, int itemsPerPage, out int totalCount);

        #region Social links

        void InsertSocialLink(SocialLinkInformation link);

        SocialLinkInformation GetSocialLinkbyId(int id);

        void DeleteSocialLink(SocialLinkInformation link);

        void UpdateSocialLink(SocialLinkInformation link);

        IPagedList<SocialLinkInformation> GetAllLinksByDoctor(string doctorId,int pageIndex = 0,
            int pageSize = Int32.MaxValue, bool showHidden = false);

        #endregion

        #region HonorsAwards
        void InsertHonorsAwards(HonorsAwards award);

        HonorsAwards GetHonorsAwardsbyId(int id);

        void DeleteHonorsAwards(HonorsAwards award);

        void UpdateHonorsAwards(HonorsAwards award);

        IPagedList<HonorsAwards> GetAllHonorsAwards(string doctorId, int pageIndex = 0,
            int pageSize = Int32.MaxValue, bool showHidden = false);
        #endregion
    }
}
