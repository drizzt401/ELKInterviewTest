using ELKInterviewTest.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ELKInterviewTest.Application.Managers
{
    public interface IDocumentManager
    {
        Task<IEnumerable<T>> GetDocuments<T>(string fileName) where T : class;
    }
}