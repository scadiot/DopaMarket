using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DopaMarket.Startup))]
namespace DopaMarket
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
