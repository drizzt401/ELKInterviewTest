using ELKInterviewTest.Application.ViewModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ELKInterviewTest.Application.Documents.Queries
{
    public class SearchQuery : IRequest<SearchResponseViewModel>
    {
        public string SearchPhrase { get; set; }
        public string[] Market { get; set; }
        public int Size { get; set; } = 25;
    }
}
