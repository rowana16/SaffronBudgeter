using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Saffron.Startup))]
namespace Saffron
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
