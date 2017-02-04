using DPTS.Domain.Core.Address;
using DPTS.Domain.Core.Doctors;
using DPTS.Domain.Core.Notification;
using DPTS.Domain.Entities;
using DPTS.EmailSmsNotifications.IServices;
using DPTS.EmailSmsNotifications.ServiceModels;
using System.Linq;

namespace DPTS.Domain.Notifications
{
    class AppointmentNotificationService : IAppointmentNotificationService
    {

        #region Fields
        private readonly IDoctorService _doctorService;
        private readonly IAddressService _addressService;
        private readonly ISmsNotificationService _smsService;
        private readonly IEmailNotificationService _emailService;
        private readonly IDefaultNotificationSettingsService _defaultNotificationSettingsService;

        #endregion

        #region Constructor
        public AppointmentNotificationService(IDoctorService doctorService, 
            ISmsNotificationService smsService, IEmailNotificationService emailService, 
            IAddressService addressService, IDefaultNotificationSettingsService defaultNotificationSettingsService)
        {
            _doctorService = doctorService;
            _addressService = addressService;
            _smsService = smsService;
            _emailService = emailService;
            _defaultNotificationSettingsService = defaultNotificationSettingsService;
        }
        #endregion

        /// <summary>
        /// Send Email and sms notification on apponitment scheduled
        /// </summary>
        /// <param name="bookingDoctorId"></param>
        /// <param name="bookingAppointmentDate"></param>
        /// <param name="bookingAppointmentTime"></param>
        /// <param name="userEmail"></param>
        /// <param name="userMobileNumber"></param>
        public void SendAppointmentSchedule(string bookingDoctorId, string bookingAppointmentDate,
            string bookingAppointmentTime, string userEmail, string userMobileNumber)
        {
            string content = CreateNotificationContent(bookingDoctorId, bookingAppointmentDate, bookingAppointmentTime, "Doc Direct Appointment Schedule");

            SmsNotificationModel sms = new SmsNotificationModel();
            sms.numbers = userMobileNumber;
            sms.route = 4; //route 4 is for transactional sms
            sms.senderId = "DOCPTS";
            sms.message = content;
            _smsService.SendSms(sms);

            EmailNotificationModel email = new EmailNotificationModel();
            email.from = "support@dpts.com";
            email.to = userEmail;
            //email.subject = "Doc Direct Appointment Schedule";
            email.subject = "Doc Direct Appointment Schedule";
            email.content = content;
            _emailService.SendEmail(email);

        }

        /// <summary>
        /// Create message/email content
        /// </summary>
        /// <param name="bookingDoctorId"></param>
        /// <param name="bookingAppointmentDate"></param>
        /// <param name="bookingAppointmentTime"></param>
        /// <returns></returns>
        private string CreateNotificationContent(string bookingDoctorId, string bookingAppointmentDate, string bookingAppointmentTime, string contentName)
        {
            string content = "";
            if (bookingDoctorId.Trim() == "")
            {
                content = _defaultNotificationSettingsService.GetDefaultNotificationSettingsByName(contentName).Message;
            }
            else
            {
                Doctor doctorDetails = _doctorService.GetDoctorbyId(bookingDoctorId);
                string doctorName = "Dr. " + doctorDetails.AspNetUser.FirstName + doctorDetails.AspNetUser.LastName;
                string appointmemtSchedule = bookingAppointmentDate + " " + bookingAppointmentTime;
                string appomitmentAddress = _addressService.GetAllAddressByUser(bookingDoctorId).FirstOrDefault().Address1 + " " +
                    _addressService.GetAllAddressByUser(bookingDoctorId).FirstOrDefault().Address2 + " " +
                    _addressService.GetAllAddressByUser(bookingDoctorId).FirstOrDefault().City + " " +
                    _addressService.GetAllAddressByUser(bookingDoctorId).FirstOrDefault().ZipPostalCode;
                string contactNumber = doctorDetails.AspNetUser.PhoneNumber;

                content = "Your appointment with " + doctorName + " is scheduled for " +
                    appointmemtSchedule + ", " + appomitmentAddress + ", " + contactNumber;
            }

            return content;
        }



        //public class AppointmentScheduleDetails
        //{
        //    public string userMobileNumber;
        //    public string userEmail;
        //    public string bookingDoctorId;
        //    public string bookingAppointmentDate;
        //    public string bookingAppointmentTime;
        //    public string emailFrom = doctorDetails.AspNetUser.Email;
        //    public string emailTo = scheduleDetails.userEmail;
        //    public string emailSubject;
        //    //"Doc Direct Appointment Schedule.";
        //    public string emailContent;
        //    //"Your appointment with "
        //    public string doctorName;
        //    //" is scheduled for " +
        //    public string appointmemtSchedule;
        //    //+ ", " + 
        //    public string appointmentAddress;
        //    //+ ", " + 
        //    public string appointmentContactNumber;
        //}

        private string CreateEmailContent(string bookingDoctorId, string bookingAppointmentDate, string bookingAppointmentTime)
        {
            Doctor doctorDetails = _doctorService.GetDoctorbyId(bookingDoctorId);
            string doctorName = "Dr. " + doctorDetails.AspNetUser.FirstName + doctorDetails.AspNetUser.LastName;
            string appointmemtSchedule = bookingAppointmentDate + " " + bookingAppointmentTime;
            string appomitmentAddress = _addressService.GetAllAddressByUser(bookingDoctorId).FirstOrDefault().Address1 + " " +
                _addressService.GetAllAddressByUser(bookingDoctorId).FirstOrDefault().Address2 + " " +
                _addressService.GetAllAddressByUser(bookingDoctorId).FirstOrDefault().City + " " +
                _addressService.GetAllAddressByUser(bookingDoctorId).FirstOrDefault().ZipPostalCode;
            string contactNumber = doctorDetails.AspNetUser.PhoneNumber;

            string content = "Your appointment with " + doctorName + " is scheduled for " +
                appointmemtSchedule + ", " + appomitmentAddress + ", " + contactNumber;
            return content;
        }

    }
}