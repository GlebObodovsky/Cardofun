using System;
using System.Net;
using System.Runtime.Loader;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;

namespace Cardofun.Microservices.Repositories.Location
{
    public class Program
    {
        public static void Main(string[] args)
        {
           var _quitEvent = new ManualResetEvent(false);
            AssemblyLoadContext.Default.Unloading += ctx =>
            {
                try
                {
                    Console.WriteLine("Disposing resources...");
                    // ToDo: Dispose shit here
                }
                catch(Exception e)
                {
                    Console.WriteLine($"Shit disposal failed: {e.Message}");
                }
                _quitEvent.Set();
            };

            CreateHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args) =>
            new WebHostBuilder()
                .UseKestrel(o =>
                    {
                        try
                        {
                            var config = ContextProvider.GetConfiguration();
                            var MicroserviceConfiguration = new MicroserviceConfiguration(config);

                            o.Listen(IPAddress.Any, MicroserviceConfiguration.GrpcPort, listenOptions =>
                            {
                                listenOptions.Protocols = HttpProtocols.Http2;
                            });
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Starting Location Server Failed, Error: {e.Message}");
                        }
                    }
                )
                .UseStartup<Startup>();
    }

    public class MicroserviceConfiguration
    {
        private IConfigurationRoot _config;

        public int GrpcPort
        {
            get
            {
                int grpcPort = 0;
                if (!int.TryParse(_config["GrpcPort"], out grpcPort))
                {
                    throw new Exception("Invalid gRPC Port");
                }

                return grpcPort;
            }
        }

        public MicroserviceConfiguration(IConfigurationRoot config)
        {
            _config = config;
        }
    }
}
