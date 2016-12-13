using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(dpts.portal.Startup))]
namespace dpts.portal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
