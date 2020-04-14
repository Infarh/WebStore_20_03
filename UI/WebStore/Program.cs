using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace WebStore
{
    public class Program
    {
        public static void Main(string[] args) => 
            CreateHostBuilder(args)
               .Build()
               .Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder =>
                {
                    builder.UseStartup<Startup>()
                       .ConfigureLogging((host, log) =>
                        {
                            //log.ClearProviders();
                            //log.AddConsole(o => o.IncludeScopes = true);
                            //log.AddDebug()
                            //log.AddEventLog()
                            //log.AddFilter("WebStore.Controllers.AccountController", LogLevel.Warning);
                            //log.AddFilter<ConsoleLoggerProvider>((category, level) => category.StartsWith("WebStore") && level > LogLevel.Warning);
                        })
                       .UseSerilog((host, log) => log.ReadFrom.Configuration(host.Configuration)
                           .MinimumLevel.Debug()
                           .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                           .Enrich.FromLogContext()
                           .WriteTo.Console(
                                outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}]{SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}"
                                )
                           .WriteTo.RollingFile($@".\Logs\WebStore[{DateTime.Now:yyyy-MM-ddTHH-mm-ss}].log")
                           .WriteTo.File(new JsonFormatter(",", true), $@".\Logs\WebStore[{DateTime.Now:yyyy-MM-ddTHH-mm-ss}].log.json")
                           .WriteTo.Seq("http://localhost:5341/")
                            );
                });
    }
}