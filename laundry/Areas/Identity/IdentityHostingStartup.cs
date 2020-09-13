using System;
using laundry.Areas.Identity.Data;
using laundry.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(laundry.Areas.Identity.IdentityHostingStartup))]
namespace laundry.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<laundryDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("laundryDbContextConnection")));

                services.AddDefaultIdentity<laundryUser>(options =>
                {
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.SignIn.RequireConfirmedAccount = false;
                })
                     .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<laundryDbContext>();
            });
        }
    }
}