using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

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
                        });
                });
    }
}