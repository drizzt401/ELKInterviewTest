using System.Threading;
using System.Threading.Tasks;

namespace ELKInterviewTest.Application.Indexer
{
    public interface IDocumentIndexer
    {
        Task IndexDocumentsAsync(CancellationToken cancellationToken = default);
    }
}