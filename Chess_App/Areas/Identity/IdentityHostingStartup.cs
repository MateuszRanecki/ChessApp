using System;
using Chess_App.Areas.Identity.Data;
using Chess_App.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Chess_App.Areas.Identity.IdentityHostingStartup))]
namespace Chess_App.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<Chess_AppContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("Chess_AppContextConnection")));

                services.AddDefaultIdentity<Chess_AppUser>(options => {
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.SignIn.RequireConfirmedAccount = false;
                })
                    .AddEntityFrameworkStores<Chess_AppContext>();
            });
        }
    }
}