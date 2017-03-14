using DPTS.Common.Kendoui;
using DPTS.Domain.Core.Doctors;
using System.Web.Mvc;

namespace DPTS.Web.Controllers
{
    public class AdministrationController : Controller
    {
        #region Fields
        private readonly IDoctorService _doctorService;
        #endregion

        #region Ctr
        public AdministrationController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }
        #endregion

        // GET: Administration
        [Route("/admin")]
        public ActionResult Index()
        {
            return View();
        }

        #region Doctor
        public ActionResult Doctors()
        {
            return View();
        }
        //[HttpPost]
        //public ActionResult Doctor_Read(DataSourceRequest command)
        //{
        //    var doctor = _doctorService.GetAllDoctors(command.Page - 1, 5, );
        //    var gridModel = new DataSourceResult
        //    {
        //        Data = doctor.Select(x => new Doctor
        //        {
        //            Id = x.Id,
        //            IsActive = x.IsActive,
        //            DisplayOrder = x.DisplayOrder,
        //            DoctorId = x.DoctorId,
        //            Title = x.Title,
        //            Description = x.Description,
        //            StartDate = x.StartDate,
        //            EndDate = x.EndDate,
        //            Organization = x.Organization
        //        }),
        //        Total = experience.TotalCount
        //    };
        //    return Json(gridModel);
        //}
        #endregion
    }
}