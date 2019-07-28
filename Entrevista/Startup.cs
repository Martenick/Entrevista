using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Entrevista.Startup))]
namespace Entrevista
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
