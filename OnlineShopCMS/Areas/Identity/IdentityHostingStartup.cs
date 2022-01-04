using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OnlineShopCMS.Areas.Identity.Data;
using OnlineShopCMS.Data;

[assembly: HostingStartup(typeof(OnlineShopCMS.Areas.Identity.IdentityHostingStartup))]
namespace OnlineShopCMS.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<OnlineShopUserContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("OnlineShopUserContextConnection")));
                
                services.AddRazorPages();

                services.AddDefaultIdentity<OnlineShopUser>(options =>
                {
                    options.Password.RequiredLength = 4;             //密碼長度
                    options.Password.RequireLowercase = false;       //包含小寫英文
                    options.Password.RequireUppercase = false;       //包含大寫英文
                    options.Password.RequireNonAlphanumeric = false; //包含符號
                    options.Password.RequireDigit = false;           //包含數字
                })
                .AddRoles<IdentityRole>() //角色
                .AddEntityFrameworkStores<OnlineShopUserContext>();
                //services.AddDefaultIdentity<OnlineShopUser>(options => options.SignIn.RequireConfirmedAccount = true)
                //    .AddEntityFrameworkStores<OnlineShopUserContext>();
            });
        }
    }
}