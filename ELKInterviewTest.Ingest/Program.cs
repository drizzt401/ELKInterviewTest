using ELKInterviewTest.Application.Indexer;
using ELKInterviewTest.Application.Managers;
using ELKInterviewTest.Infrastructure;
using ELKInterviewTest.Infrastructure.ElasticSearchConfig;
using ELKInterviewTest.Infrastructure.Indexer;
using ELKInterviewTest.Infrastructure.Managers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nest;
using Serilog;

namespace ELKInterviewTest.Ingest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = SerilogConfig.GetLogger();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)

                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    var env = hostContext.HostingEnvironment;
                    services.AddHostedService<Worker>();
                    services.AddSingleton<IElasticClient>(new ElasticSearchConfig(configuration).GetElasticClient());
                    services.AddSingleton<IDocumentManager, DocumentManager>();
                    services.AddSingleton<IDocumentIndexer, DocumentIndexer>();
                });
    }
}
