using Microsoft.AspNetCore.Mvc;

namespace JobScheduling.Api.Controllers.Defaults
{
    [Route(""), ApiController]
    public class MeController : ControllerBase
    {
        [HttpGet, Route("")]
        public IActionResult Get() => Ok(new { name = "JobScheduling" });
    }
}