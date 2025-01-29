using ElasticFileLoad.Models;
using ElasticFileLoad.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ElasticFileLoad
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Build config
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            // Configure services
            var serviceProvider = new ServiceCollection()
                .AddLogging(opt =>
                {
                    opt.AddConsole();
                    opt.SetMinimumLevel(LogLevel.Debug);
                })
                .Configure<ElasticSettings>(config.GetSection("ElasticSettings"))
                .AddSingleton<IElasticService, ElasticService>()
                .AddTransient<IImportService, ImportService>()
                .BuildServiceProvider();

            // Run data importer
            var importService = serviceProvider.GetService<IImportService>();
            importService.ImportAsync(args).Wait();
        }
    }
}
