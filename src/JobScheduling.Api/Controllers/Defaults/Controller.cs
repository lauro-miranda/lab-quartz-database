using LM.Responses;
using LM.Responses.Extensions;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace JobScheduling.Api.Controllers
{
    public abstract class Controller : ControllerBase
    {
        protected async Task<IActionResult> WithResponseAsync<TResponseMessage>(Func<Task<Response<TResponseMessage>>> func)
        {
            try
            {
                var response = await func.Invoke();

                if (!response.HasError)
                    return Ok(response);

                if (response.Messages.Any(m => m.Type == MessageType.BusinessError))
                    return BadRequest(response);

                Log.Error(new Exception("500"), string.Join(";", response.Messages.Select(x => x.Text)));
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, Response<TResponseMessage>.Create().WithCriticalError(ex.Message));
            }
        }

        protected async Task<IActionResult> WithResponseAsync(Func<Task<Response>> func)
        {
            try
            {
                var response = await func.Invoke();

                if (!response.HasError)
                    return Ok(response);

                if (response.Messages.Any(m => m.Type == MessageType.BusinessError))
                    return BadRequest(response);

                Log.Error(new Exception("500"), string.Join(";", response.Messages.Select(x => x.Text)));
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, Response<bool>.Create().WithCriticalError(ex.Message));
            }
        }

        protected IActionResult WithResponse(Func<Response> func)
        {
            try
            {
                var response = func.Invoke();

                if (!response.HasError)
                    return Ok(response);

                if (response.Messages.Any(m => m.Type == MessageType.BusinessError))
                    return BadRequest(response);

                Log.Error(new Exception("500"), string.Join(";", response.Messages.Select(x => x.Text)));
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, Response<bool>.Create().WithCriticalError(ex.Message));
            }
        }
    }
}