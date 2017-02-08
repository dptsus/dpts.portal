using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DPTS.Domain.Core.ExportImport;
using DPTS.Domain.Entities;
using LinqToExcel;

namespace DPTS.Web.Controllers
{
    public class BulkDataController : Controller
    {
        private readonly IImportManager _importManager;
        // GET: BulkData
        public BulkDataController(IImportManager importManager)
        {
            _importManager = importManager;
        }
        public ActionResult OnBoardDoctors()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ImportFromXlsx()
        {
            try
            {
                var file = Request.Files["importexcelfile"];
                if (file != null && file.ContentLength > 0)
                {
                    _importManager.ImportDoctorsFromXlsx(file.InputStream);
                }
                else
                {
                    return null;
                }
                return null;
            }
            catch (Exception exc)
            {
                return null;
            }
        }
    }
}