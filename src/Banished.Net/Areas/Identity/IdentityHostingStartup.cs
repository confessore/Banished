using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Banished.Net.Areas.Identity.IdentityHostingStartup))]
namespace Banished.Net.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
            });
        }
    }
}