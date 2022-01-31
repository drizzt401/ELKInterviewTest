using Elasticsearch.Net;
using ELKInterviewTest.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using ELKInterviewTest.Infrastructure;
using ELKInterviewTest.Application.Managers;
using ELKInterviewTest.Infrastructure.Managers;
using ELKInterviewTest.Application.Indexer;
using ELKInterviewTest.Infrastructure.Indexer;
using ELKInterviewTest.Infrastructure.ElasticSearchConfig;

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
