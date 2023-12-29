using JobScheduling.Domain.Groups.Services.Contracts;
using JobScheduling.Messages.Requests;
using JobScheduling.Messages.Responses;
using LM.Responses;
using Microsoft.AspNetCore.Mvc;

namespace JobScheduling.Api.Controllers
{
    [Route("api/[controller]"), ApiController]
    public class GroupController : Controller
    {
        IGroupService GroupService { get; }

        public GroupController(IGroupService groupService)
        {
            GroupService = groupService;
        }

        [HttpGet, Route("")]
        [Produces(typeof(Response<List<GroupResponseMessage>>))]
        public async Task<IActionResult> GetGroupAllAsync()
            => await WithResponseAsync(() => GroupService.GetAllAsync());

        [HttpGet, Route("{code}")]
        [Produces(typeof(Response<GroupResponseMessage>))]
        public async Task<IActionResult> GetGroupAsync([FromRoute] Guid code)
            => await WithResponseAsync(() => GroupService.GetAsync(code));

        [HttpPost, Route("")]
        [Produces(typeof(Response<GroupResponseMessage>))]
        public async Task<IActionResult> CreateAsync([FromBody] GroupRequestMessage message)
            => await WithResponseAsync(() => GroupService.CreateAsync(message));

        [HttpPut, Route("{code}")]
        [Produces(typeof(Response<GroupResponseMessage>))]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid code, [FromBody] GroupRequestMessage message)
            => await WithResponseAsync(() => GroupService.UpdateAsync(code, message));

        [HttpDelete, Route("{code}")]
        [Produces(typeof(Response))]
        public async Task<IActionResult> DeleteGroupAsync([FromRoute] Guid code)
            => await WithResponseAsync(() => GroupService.DeleteAsync(code));
    }
}