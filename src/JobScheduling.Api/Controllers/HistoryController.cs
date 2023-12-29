using JobScheduling.Domain.Histories.Services.Contracts;
using JobScheduling.Messages.Responses;
using LM.Responses;
using Microsoft.AspNetCore.Mvc;

namespace JobScheduling.Api.Controllers
{
    [Route("api/[controller]"), ApiController]
    public class HistoryController : Controller
    {
        IHistoryService HistoryService { get; }

        public HistoryController(IHistoryService historyService)
        {
            HistoryService = historyService;
        }

        [HttpGet, Route("job/{code}")]
        [Produces(typeof(Response<List<HistoryResponseMessage>>))]
        public async Task<IActionResult> GetAllAsync([FromRoute] Guid code)
            => await WithResponseAsync(() => HistoryService.GetByJobCodeAsync(code));
    }
}