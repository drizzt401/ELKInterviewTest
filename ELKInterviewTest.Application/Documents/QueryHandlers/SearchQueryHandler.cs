using ELKInterviewTest.Application.Documents.Queries;
using ELKInterviewTest.Application.ViewModels;
using MediatR;
using Microsoft.Extensions.Logging;
using Nest;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ELKInterviewTest.Application.Documents.QueryHandlers
{
    public class SearchQueryHandler : IRequestHandler<SearchQuery, SearchResponseViewModel>
    {
        private readonly IElasticClient client;
        private readonly ILogger logger;

        const string propertyIndex = "property";
        const string managementCompanyIndex = "managementcompany";
        public SearchQueryHandler(IElasticClient client, ILogger<SearchQueryHandler> logger)
        {
            this.client = client;
            this.logger = logger;
        }
        public async Task<SearchResponseViewModel> Handle(SearchQuery request, CancellationToken cancellationToken)
        {

            ISearchResponse<JObject> result = null;
            try
            {
                if (request.Market.Length > 0)
                {
                    result = await GetDocumentsByMarket(request);

                }
                else
                {
                    result = await GetAllDocuments(request);
                }
            }

            catch (Exception e)
            {
                logger.LogError(e.InnerException.Message ?? e.Message);
            }

            return new SearchResponseViewModel { Data = result.Documents };
        }

        private async Task<ISearchResponse<JObject>> GetDocumentsByMarket(SearchQuery request)
        {

            string markets = string.Join(" ", request.Market);
           
            return await client.SearchAsync<JObject>(s => s
            .Index($"{propertyIndex}, {managementCompanyIndex}")
            .Query(q => q
                .Bool(b => b
                .Must(mu => mu
                .MultiMatch(m => m
                .Query($"{request.SearchPhrase}")
                .Fields(f => f.Fields("property.name", "property.formerName", "mgmt.name"))
            ), mu => mu
                .MultiMatch(m => m
                .Query($"{markets}")
                    .Fields(f => f.Fields("property.market", "mgmt.market"))
                ))))
                .From(0)
                .Size(request.Size)
            );
        }

        private async Task<ISearchResponse<JObject>> GetAllDocuments(SearchQuery request)
        {
            return await client.SearchAsync<JObject>(s => s
            .Index($"{propertyIndex}, {managementCompanyIndex}")
            .Query(q => q
                 .MultiMatch(m => m
                 .Query($"{request.SearchPhrase}")
                 .Fields(f => f.Fields("property.name", "property.formerName", "mgmt.name"))
                 )
                )
                .From(0)                
                .Size(request.Size)
            );
        }
    }
}
