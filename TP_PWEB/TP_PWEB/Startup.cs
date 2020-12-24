using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TP_PWEB.Startup))]
namespace TP_PWEB
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
