using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MvcBoard.Startup))]
namespace MvcBoard
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
