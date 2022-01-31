using ELKInterviewTest.Application.Indexer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ELKInterviewTest.Ingest
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IDocumentIndexer indexer;
        private readonly IHostApplicationLifetime lifetime;

        public Worker(ILogger<Worker> logger, IDocumentIndexer indexer, IHostApplicationLifetime lifetime)
        {
            this.logger = logger;
            this.indexer = indexer;
            this.lifetime = lifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //var testData = new Management { market = "Austin", mgmtID = 1, name = "Test Name", state = "Texas" };
                //var response = await client.IndexDocumentAsync(testData);

                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await indexer.IndexDocumentsAsync();
                lifetime.StopApplication();
                //await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
