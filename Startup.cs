using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ets.Startup))]  // Specifies this class as the OWIN Startup class

namespace ets
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Adding a Welcome Page Middleware (this should now work after installing the necessary package)
            app.UseWelcomePage("/welcome");

            // You can configure more middleware components like authentication, logging, etc., here.
        }
    }
}
