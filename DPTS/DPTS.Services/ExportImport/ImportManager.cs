using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPTS.Domain.Core.Country;
using DPTS.Domain.Core.ExportImport;
using DPTS.Domain.Core.StateProvince;
using DPTS.Domain.Entities;
using System.Web;
using DPTS.Data.Context;
using DPTS.Domain.Core.Doctors;
using DPTS.Services.ExportImport.Help;
using OfficeOpenXml;

namespace DPTS.Services.ExportImport
{
    /// <summary>
    /// Import manager
    /// </summary>
    public partial class ImportManager : IImportManager
    {
        #region Fields
        private readonly IDoctorService _doctorService;
        private readonly DPTSDbContext _context;

        #endregion

        #region Ctor

        public ImportManager(IDoctorService doctorService)
        {
            this._doctorService = doctorService;
            _context =new DPTSDbContext();
        }

        #endregion

        #region Utilities

        protected virtual int GetColumnIndex(string[] properties, string columnName)
        {
            if (properties == null)
                throw new ArgumentNullException("properties");

            if (columnName == null)
                throw new ArgumentNullException("columnName");

            for (int i = 0; i < properties.Length; i++)
                if (properties[i].Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return i + 1; //excel indexes start from 1
            return 0;
        }

        protected virtual string ConvertColumnToString(object columnValue)
        {
            if (columnValue == null)
                return null;

            return Convert.ToString(columnValue);
        }


        #endregion

        #region Methods

        /// <summary>
        /// Import doctors from XLSX file
        /// </summary>
        /// <param name="stream">Stream</param>
        public virtual void ImportDoctorsFromXlsx(Stream stream)
        {
            //property array
            var properties = new[]
            {
                new PropertyByName<AspNetUser>("FirstName"),
                new PropertyByName<AspNetUser>("LastName"),
                new PropertyByName<AspNetUser>("Email"),
                new PropertyByName<AspNetUser>("IsEmailUnsubscribed"),
                new PropertyByName<AspNetUser>("IsPhoneNumberUnsubscribed"),
                new PropertyByName<AspNetUser>("IsEmailUnsubscribed"),
              //  new PropertyByName<Doctor>("LastLoginDateUtc"),
              //  new PropertyByName<Doctor>("CreatedOnUtc"),
                new PropertyByName<AspNetUser>("PhoneNumber"),
                new PropertyByName<AspNetUser>("TwoFactorEnabled"),
            };

            var manager = new PropertyManager<AspNetUser>(properties);

            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    throw new DptsException("No worksheet found");

                var iRow = 2;

                while (true)
                {
                    var allColumnsAreEmpty = manager.GetProperties
                        .Select(property => worksheet.Cells[iRow, property.PropertyOrderPosition])
                        .All(cell => cell == null || cell.Value == null || String.IsNullOrEmpty(cell.Value.ToString()));

                    if (allColumnsAreEmpty)
                        break;

                    manager.ReadFromXlsx(worksheet, iRow);

                    //manager.GetProperty("Email").ToString()

                    var doctors =
                        _context.AspNetUsers
                            .FirstOrDefault(d => d.Email.Equals(manager.GetProperty("Email").ToString()));

                    var isNew = doctors == null;

                    doctors = doctors ?? new AspNetUser();

                    if (isNew)
                        doctors.CreatedOnUtc = DateTime.UtcNow;

                    doctors.FirstName = manager.GetProperty("FirstName").StringValue;
                    doctors.LastName = manager.GetProperty("LastName").StringValue;
                    doctors.Email = manager.GetProperty("Email").StringValue;
                    doctors.IsEmailUnsubscribed = manager.GetProperty("IsEmailUnsubscribed").BooleanValue;
                    doctors.IsPhoneNumberUnsubscribed = manager.GetProperty("IsPhoneNumberUnsubscribed").BooleanValue;
                    doctors.TwoFactorEnabled = manager.GetProperty("TwoFactorEnabled").BooleanValue;
                    doctors.LastLoginDateUtc = DateTime.UtcNow;
                    doctors.Email = manager.GetProperty("PhoneNumber").StringValue;
                    
                    if (isNew)
                    {
                        _context.AspNetUsers.Add(doctors);
                        _context.SaveChanges();
                    }
                    else
                    {
                        _context.Entry(doctors).State = System.Data.Entity.EntityState.Modified;
                        _context.SaveChanges();
                    }

                    iRow++;
                }
            }
        }
        
        #endregion
    }
}
