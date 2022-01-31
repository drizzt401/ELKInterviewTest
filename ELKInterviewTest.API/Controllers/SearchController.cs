using ELKInterviewTest.Application.Documents.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ELKInterviewTest.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SearchController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Used to search for a document on Elasticsearch
        /// </summary>
        /// <remarks>Search results can be filtered based on "market"</remarks>
        [HttpGet("{searchPhrase}")]
        public async Task<IActionResult> Search(string searchPhrase, [FromQuery] string[] market, [FromQuery] int limit = 25)
        {
            var responseViewModel = await _mediator.Send(new SearchQuery { SearchPhrase = searchPhrase, Market = market, Size = limit });
            if (responseViewModel.Data is null)
                return StatusCode(StatusCodes.Status500InternalServerError, "An error has occured");
            return Ok(responseViewModel.Data);
        }
    }
}
