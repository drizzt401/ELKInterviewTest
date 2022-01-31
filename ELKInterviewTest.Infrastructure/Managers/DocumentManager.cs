using ELKInterviewTest.Application.Managers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;

namespace ELKInterviewTest.Infrastructure.Managers
{
    public class DocumentManager : IDocumentManager
    {
        private readonly IHostEnvironment env;
        private readonly ILogger<DocumentManager> logger;
        string contentRootPath;
        public DocumentManager(IHostEnvironment env, ILogger<DocumentManager> logger)
        {
            this.env = env;
            this.logger = logger;
        }

        public async Task<IEnumerable<T>> GetDocuments<T>(string fileName) where T : class
        {

            contentRootPath = Path.Combine(env.ContentRootPath, @"TestFiles\");
            using (FileStream fs = new FileStream(Path.Combine(contentRootPath, fileName), FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    string objectListJson = await reader.ReadToEndAsync();

                    List<T> objectList = JsonConvert.DeserializeObject<List<T>>(objectListJson.Trim(), new JsonSerializerSettings
                    {
                        Error = HandleDeserializationError
                    });

                    return objectList;
                }
            }
        }

        private void HandleDeserializationError(object sender, ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
            logger.LogError(currentError);
        }
    }
}
