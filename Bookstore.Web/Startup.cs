using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BabyStore.Web.Startup))]
namespace BabyStore.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
