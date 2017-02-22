using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Bookstore.Web.Startup))]
namespace Bookstore.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
