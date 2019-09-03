using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(testApp.Startup))]
namespace testApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
