using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DPTS.Web.Startup))]
namespace DPTS.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
