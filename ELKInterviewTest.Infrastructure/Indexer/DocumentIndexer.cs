using ELKInterviewTest.Application.Indexer;
using ELKInterviewTest.Application.Managers;
using ELKInterviewTest.Domain;
using Microsoft.Extensions.Configuration;
using Nest;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using ELKInterviewTest.Domain.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace ELKInterviewTest.Infrastructure.Indexer
{
    public class DocumentIndexer : IDocumentIndexer
    {
        private readonly IDocumentManager documentManager;
        private readonly IConfiguration configuration;
        private readonly IElasticClient client;
        private readonly ILogger logger;

        public DocumentIndexer(IDocumentManager documentManager, IConfiguration configuration, IElasticClient client, ILogger<DocumentIndexer> logger)
        {
            this.documentManager = documentManager;
            this.configuration = configuration;
            this.client = client;
            this.logger = logger;
        }

        public async Task IndexDocumentsAsync(CancellationToken cancellationToken = default)
        {

            //Get FileNames
            string propertiesJSONfileName = configuration["propertiesFileName"];
            string mgmtJSONfileName = configuration["mgmtFileName"];

            //Serialize Documents
            var propertyTestData = await documentManager.GetDocuments<JObject>(propertiesJSONfileName);
            var mgmtTestData = await documentManager.GetDocuments<JObject>(mgmtJSONfileName);

            // Create Indices
            await CreateIndexAsync<Property>(cancellationToken);
            await CreateIndexAsync<ManagementCompany>(cancellationToken);

            //Index Documents
            IndexBulk("Property", propertyTestData, cancellationToken);
            IndexBulk("ManagementCompany", mgmtTestData, cancellationToken);

        }

        private void IndexBulk<T>(string indexName, IEnumerable<T> documents, CancellationToken stoppingToken) where T : class
        {
            #region IndexMultipleRecords
            var bulkAllObservable = client.BulkAll(documents, b => b
                .Index(indexName.ToLower())
                .BackOffTime("30s")
                .BackOffRetries(2)
                .RefreshOnCompleted()
                .MaxDegreeOfParallelism(Environment.ProcessorCount)
                .Size(1000)
            );

            var waitHandle = new CountdownEvent(1);

            ExceptionDispatchInfo dispatchInfo = null;
            var subscription = bulkAllObservable.Subscribe(new BulkAllObserver(
                onNext: b => logger.LogInformation("Data has been indexed"),
                onError: e =>
                {
                    dispatchInfo = ExceptionDispatchInfo.Capture(e);
                    waitHandle.Signal();
                },
                onCompleted: () => waitHandle.Signal()
            ));

            waitHandle.Wait(TimeSpan.FromMinutes(30), stoppingToken);

            if (dispatchInfo != null && !(dispatchInfo.SourceException is OperationCanceledException))
                logger.LogError(dispatchInfo.SourceException.InnerException?.Message ?? dispatchInfo.SourceException.Message);
            #endregion
        }

        private async Task CreateIndexAsync<T>(CancellationToken cancellationToken) where T: class
        {
            try
            {
                string indexName = typeof(T).Name;
                DeleteIndexIfItExists(indexName);
                await client.Indices.CreateAsync(indexName, i => i
                    .Settings(s => s
                        .NumberOfShards(2)
                        .NumberOfReplicas(0)
                        .Analysis(InitCommonAnalyzers)                      
                ), cancellationToken);

                await client.Indices.PutAliasAsync(indexName, $"{indexName}List", ct: cancellationToken);
                logger.LogInformation("Index '{index}' has been created", indexName);
            }
            catch (Exception e)
            {

                logger.LogError(e.InnerException?.Message ?? e.Message);
            }
        }

        protected static IAnalysis InitCommonAnalyzers(AnalysisDescriptor analysis)
        {
            return analysis.Analyzers(a => a
                .Custom("autocomplete", cc => cc
                    .Filters("eng_stopwords", "trim")
                    .Tokenizer("autocomplete")
                )
            )
            .TokenFilters(f => f
                .Stop("eng_stopwords", lang => lang
                    .StopWords("_english_")
                )
            );
        }

        public void DeleteIndexIfItExists(string name)
        {
            if (client.Indices.Get(name).Indices.Count > 0)
                client.Indices.Delete(name);
        }
    }
}
