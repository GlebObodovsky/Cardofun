using System.IO;
using Cardofun.Core.NameConstants;
using Cardofun.DataContext.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Cardofun.Microservices.Repositories.Location
{
    public static class ContextProvider
    {
         static IConfigurationRoot _configuration = null;
        public static ServiceProvider ServiceProvider { get; set; }

        public static IConfigurationRoot GetConfiguration()
        {
            _configuration = _configuration ?? new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile($"appsettings.json")
               .AddEnvironmentVariables()
               .Build();

            return _configuration;
        }

        public static void AddServicesTo(IServiceCollection services)
        {
            services.AddGrpc();
            services.AddDbContext<CardofunContext>(x => x.UseSqlServer(_configuration.GetConnectionString(ConnectionStringConstants.CardofunSqlServerConnection)));

            services.AddLogging(builder =>
            {
                builder
                    .AddConsole(options =>
                    {
                        options.DisableColors = true;
                        options.Format = ConsoleLoggerFormat.Systemd;
                    });
            });

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}