using ELKInterviewTest.Domain;
using Moq;
using Nest;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Xunit;

namespace ELKInterviewTest.UnitTests.HandlerTests
{
    public class SearchQueryHandlerTests
    {

        private Mock<IElasticClient> elasticClientMock;
        private Mock<ISearchResponse<JObject>> mockSearchResponse;
        public SearchQueryHandlerTests()
        {
            elasticClientMock = new Mock<IElasticClient>();
            mockSearchResponse = new Mock<ISearchResponse<JObject>>();
        }

        [Fact]
        public void ShouldReturnSearchedDocuments()
        {
            var documents = new List<JObject>
            {
                JObject.FromObject(new ManagementCompany{ market = "Test", mgmtID = 2, name ="Test", state ="Test"}),
                JObject.FromObject(new ManagementCompany{ market = "Test", mgmtID = 2, name ="Test", state ="Test"}),
            };

            mockSearchResponse.Setup(x => x.Documents).Returns(documents);

            elasticClientMock.Setup(x => x
            .Search(It.IsAny<Func<SearchDescriptor<JObject>, ISearchRequest>>()))
            .Returns(mockSearchResponse.Object);

            var result = elasticClientMock.Object.Search<JObject>(s => s);

            Assert.Equal(2, result.Documents.Count);
        }
    }
}
