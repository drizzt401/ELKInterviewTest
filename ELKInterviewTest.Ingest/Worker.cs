using ELKInterviewTest.Application.Indexer;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ELKInterviewTest.Ingest
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> logger;
        private readonly IDocumentIndexer indexer;

        public Worker(ILogger<Worker> logger, IDocumentIndexer indexer)
        {
            this.logger = logger;
            this.indexer = indexer;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Worker stopped at: {time}", DateTimeOffset.Now);
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await indexer.IndexDocumentsAsync();
                await StopAsync(stoppingToken);
            }
        }
    }
}
