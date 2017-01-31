using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ElmahLogging.Startup))]
namespace ElmahLogging
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
