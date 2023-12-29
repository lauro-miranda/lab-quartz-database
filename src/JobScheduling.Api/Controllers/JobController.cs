using JobScheduling.Domain.Jobs.Services.Contracts;
using JobScheduling.Messages.Requests;
using JobScheduling.Messages.Responses;
using LM.Responses;
using Microsoft.AspNetCore.Mvc;

namespace JobScheduling.Api.Controllers
{
    [Route("api/[controller]"), ApiController]
    public class JobController : Controller
    {
        IJobService JobService { get; }

        public JobController(IJobService jobService)
        {
            JobService = jobService;
        }

        [HttpGet, Route("")]
        [Produces(typeof(Response<List<JobResponseMessage>>))]
        public async Task<IActionResult> GetAllAsync()
            => await WithResponseAsync(() => JobService.GetAllAsync());

        [HttpGet, Route("{code}")]
        [Produces(typeof(Response<JobResponseMessage>))]
        public async Task<IActionResult> GetAsync([FromRoute] Guid code)
            => await WithResponseAsync(() => JobService.GetAsync(code));

        [HttpPost, Route("")]
        [Produces(typeof(Response<JobResponseMessage>))]
        public async Task<IActionResult> CreateAsync([FromBody] JobRequestMessage message)
            => await WithResponseAsync(() => JobService.CreateAsync(message));

        [HttpPut, Route("{code}")]
        [Produces(typeof(Response<JobResponseMessage>))]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid code, [FromBody] JobRequestMessage message)
            => await WithResponseAsync(() => JobService.UpdateAsync(code, message));

        [HttpDelete, Route("{code}")]
        [Produces(typeof(Response))]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid code)
            => await WithResponseAsync(() => JobService.DeleteAsync(code));
    }
}