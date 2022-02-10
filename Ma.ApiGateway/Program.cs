using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace Ma.ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseKestrel()
                        .UseIIS()
                        .ConfigureAppConfiguration((host, config) => config
                        .AddJsonFile($"ocelot.{host.HostingEnvironment.EnvironmentName}.json"))
                        .UseStartup<Startup>();
                })
            .Build();

            host.Run();
        }
    }
}
