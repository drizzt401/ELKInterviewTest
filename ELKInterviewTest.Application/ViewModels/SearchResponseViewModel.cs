using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace ELKInterviewTest.Application.ViewModels
{
    public class SearchResponseViewModel
    {
        public IReadOnlyCollection<JObject> Data { get; set; }
    }
}
