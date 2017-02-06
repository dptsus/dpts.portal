using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DPTS.Domain.Entities;
using LinqToExcel;

namespace DPTS.Web.Controllers
{
    public class BulkDataController : Controller
    {
        // GET: BulkData
        public ActionResult OnBoardDoctors()
        {
            return View();
        }

        [HttpPost]
        public ActionResult OnBoardDoctors(HttpPostedFileBase fileUpload)
        {
            List<string> data = new List<string>();
            if (fileUpload != null)
            {
                if (fileUpload.ContentType == "application/vnd.ms-excel" ||
                    fileUpload.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    string filename = fileUpload.FileName;
                    string targetpath = Server.MapPath("~/Doc/");
                    fileUpload.SaveAs(targetpath + filename);
                    string pathToExcelFile = targetpath + filename;
                    var connectionString = "";
                    if (filename.EndsWith(".xls"))
                    {
                        connectionString =
                            $"Provider=Microsoft.Jet.OLEDB.4.0; data source={pathToExcelFile}; Extended Properties=Excel 8.0;";
                    }
                    else if (filename.EndsWith(".xlsx"))
                    {
                        connectionString =
                            $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={pathToExcelFile};Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";";
                    }

                    var adapter = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", connectionString);
                    var ds = new DataSet();

                    adapter.Fill(ds, "ExcelTable");

                    DataTable dtable = ds.Tables["ExcelTable"];

                    const string sheetName = "Sheet1";

                    var excelFile = new ExcelQueryFactory(pathToExcelFile);
                    var Doctors = from a in excelFile.Worksheet<Doctor>(sheetName) select a;

                    foreach (var a in Doctors)
                    {
                        try
                        {
                            Doctor doc = new Doctor
                            {
                                DoctorId = "",
                                Gender = ""
                            };

                            doc.Qualifications = doc.Qualifications;
                            doc.Expertise = doc.Expertise;
                            doc.RegistrationNumber = doc.RegistrationNumber;
                            doc.YearsOfExperience = doc.YearsOfExperience;
                            doc.ShortProfile = doc.ShortProfile;

                            doc.Subscription = doc.Subscription;
                            //TODO Move to a service class & Some good notifications
                            //db.Doctors.AddRange("List");
                            // db.SaveChanges();
                        }

                        catch (DbEntityValidationException ex)
                        {
                            foreach (
                                var validationError in ex.EntityValidationErrors.SelectMany(ev => ev.ValidationErrors))
                            {
                                Response.Write("Property: " + validationError.PropertyName + " Error: " +
                                               validationError.ErrorMessage);
                            }
                        }
                    }
                    //deleting excel file from folder  
                    if (System.IO.File.Exists(pathToExcelFile))
                    {
                        System.IO.File.Delete(pathToExcelFile);
                    }
                    return Json("success", JsonRequestBehavior.AllowGet);
                }
                //alert message for invalid file format  
                data.Add("<ul>");
                data.Add("<li>Only Excel file format is allowed</li>");
                data.Add("</ul>");
                var strings = data.ToArray();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            data.Add("<ul>");
            data.Add("<li>Please choose Excel file</li>");
            data.Add("</ul>");
            var array = data.ToArray();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}