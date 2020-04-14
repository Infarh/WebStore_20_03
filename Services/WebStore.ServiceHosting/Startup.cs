using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using WebStore.DAL.Context;
using WebStore.Domain.Entities.Identity;
using WebStore.Interfaces.Services;
using WebStore.Logger;
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

            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "WebStore.API", Version = "v1" });

                opt.IncludeXmlComments("WebStore.ServiceHosting.xml");

                const string domain_doc_xml = "WebStore.Domain.xml";
                const string debug_path = @"bin\Debug\netcoreapp3.1";
                if(File.Exists(domain_doc_xml))
                    opt.IncludeXmlComments(domain_doc_xml);
                else if(File.Exists(Path.Combine(debug_path, domain_doc_xml)))
                    opt.IncludeXmlComments(Path.Combine(debug_path, domain_doc_xml));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, WebStoreDBInitializer db, ILoggerFactory log)
        {
            log.AddLog4Net();

            db.Initialize();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(opt =>
            {
                opt.SwaggerEndpoint("/swagger/v1/swagger.json", "WebStore.API");
                opt.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}