using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebStore.DAL.Context;
using WebStore.Domain.Entities.Identity;
using WebStore.Interfaces.Services;
using WebStore.Services.Data;
using WebStore.Services.Products.InCookies;
using WebStore.Services.Products.InMemory;
using WebStore.Services.Products.InSQL;

namespace WebStore.ServiceHosting
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) => Configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            #region Сервисы бизнес-логики

            services.AddSingleton<IEmployeesData, InMemoryEmployeesData>();
            services.AddScoped<IProductData, SqlProductData>();
            services.AddScoped<IOrderService, SqlOrderService>();
            services.AddScoped<ICartService, CookiesCartService>(); 

            #endregion

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            #region База данных

            services.AddDbContext<WebStoreDB>(opt =>
                   opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<WebStoreDBInitializer>();

            #endregion

            #region Identity

            services.AddIdentity<User, Role>(opt =>
                   {
                       opt.Password.RequiredLength = 3;
                       opt.Password.RequireDigit = false;
                       opt.Password.RequireUppercase = false;
                       opt.Password.RequireLowercase = true;
                       opt.Password.RequireNonAlphanumeric = false;
                       opt.Password.RequiredUniqueChars = 3;

                       opt.User.RequireUniqueEmail = false;

                       opt.Lockout.AllowedForNewUsers = true;
                       opt.Lockout.MaxFailedAccessAttempts = 10;
                       opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                   })
                  .AddEntityFrameworkStores<WebStoreDB>()
                  .AddDefaultTokenProviders(); 

            #endregion
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, WebStoreDBInitializer db)
        {
            db.Initialize();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
