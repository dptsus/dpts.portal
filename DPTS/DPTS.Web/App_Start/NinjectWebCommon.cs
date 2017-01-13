using DPTS.Domain.Core.Address;
using DPTS.Domain.Core.Country;
using DPTS.Domain.Core.DefaultNotificationSettings;
using DPTS.Domain.Core.Doctors;
using DPTS.Domain.Core.EmailCategory;
using DPTS.Domain.Core.Speciality;
using DPTS.Domain.Core.StateProvince;
using DPTS.Domain.Core.SubSpeciality;
using DPTS.Services.Address;
using DPTS.Services.Country;
using DPTS.Services.DefaultNotificationSettings;
using DPTS.Services.Doctors;
using DPTS.Services.EmailCategory;
using DPTS.Services.Speciality;
using DPTS.Services.StateProvince;
using DPTS.Services.SubSpeciality;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(DPTS.Web.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(DPTS.Web.App_Start.NinjectWebCommon), "Stop")]

namespace DPTS.Web.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using Domain.Core;
    using Domain.Entities;
    using Services;
    using Domain.Core.Appointment;
    using Services.Appointment;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind(typeof(IRepository<>)).To(typeof(Repository<>)).InRequestScope();
            kernel.Bind<IDoctorService>().To<DoctorService>();
            kernel.Bind<ISpecialityService>().To<SpecialityService>();
            kernel.Bind<ICountryService>().To<CountryService>();
            kernel.Bind<IStateProvinceService>().To<StateProvinceService>();
            kernel.Bind<ISubSpecialityService>().To<SubSpecialityService>();
            kernel.Bind<IAddressService>().To<AddressService>();
            kernel.Bind<IEmailCategoryService>().To<EmailCategoryService>();
            kernel.Bind<IAppointmentService>().To<AppointmentService>();
            kernel.Bind<IDefaultNotificationSettingsService>().To<DefaultNotificationSettingsService>();

        }
    }
}
