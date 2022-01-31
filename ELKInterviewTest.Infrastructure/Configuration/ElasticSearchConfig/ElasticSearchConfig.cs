using Elasticsearch.Net;
using Microsoft.Extensions.Configuration;
using Nest;
using Nest.JsonNetSerializer;
using System.Text;

namespace ELKInterviewTest.Infrastructure.ElasticSearchConfig
{
    public class ElasticSearchConfig
    {
        private readonly ConnectionSettings settings;

        public ElasticClient GetElasticClient() => new ElasticClient(settings);

        public ElasticSearchConfig(IConfiguration configuration)
        {
            var pool = new CloudConnectionPool(configuration["ELKService:cloudId"], new BasicAuthenticationCredentials(
                    "elastic", configuration["ELKService:password"]));

            settings = new ConnectionSettings(pool, JsonNetSerializer.Default)
                       .DefaultIndex("example-index")
                       .EnableDebugMode()
                       .OnRequestCompleted(apiCallDetails =>
                        {
                            if (apiCallDetails.RequestBodyInBytes != null)
                                System.Diagnostics.Debug.WriteLine("RequestBody" + Encoding.UTF8.GetString(apiCallDetails.RequestBodyInBytes));
                        });
        }


    }
}
