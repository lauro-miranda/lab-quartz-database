using JobScheduling.Domain.Groups.Repositories.Contracts;
using JobScheduling.Domain.Jobs.Repositories.Contracts;
using JobScheduling.Infra.Schedulings.Scheduler;
using JobScheduling.Messages.Responses;
using LM.Domain.Helpers;
using LM.Responses;
using Microsoft.AspNetCore.Mvc;
using Quartz;

namespace JobScheduling.Api.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class MonitorController : ControllerBase
    {
        IJobRepository Repository { get; }

        ICustomScheduler Scheduler { get; }

        public MonitorController(IJobRepository repository
            , ICustomScheduler scheduler)
        {
            Repository = repository;
            Scheduler = scheduler;
        }

        [HttpGet, Route("")]
        public async Task<IActionResult> GetAsync()
        {
            var response = Response<List<ExecutionJobResponseMessage>>.Create(new List<ExecutionJobResponseMessage>());

            var jobs = await Repository.GetAllAsync();

            if (!jobs.Any()) return NoContent();

            foreach (var grouped in jobs.GroupBy(j => j.JobGroup.Group))
            {
                var executionJobs = await Scheduler.GetExecutionJobsAsync(grouped.Key.Name);

                if (!executionJobs.Any()) continue;

                var message = new ExecutionJobResponseMessage
                {
                    Code = grouped.Key.Code,
                    Name = grouped.Key.Name
                };

                foreach (var job in grouped)
                {
                    var cron = new CronExpression(job.CronExpression);

                    message.Jobs.Add(new ExecutionJobResponseMessage.JobResponseMessage
                    {
                        Code = job.Code,
                        Name = job.Name,
                        Data = job.Data,
                        Description = job.Description,
                        CronExpression = job.CronExpression,
                        NextValidTimeAfter = cron.GetNextValidTimeAfter(DateTimeOffset.Now)
                    });
                }

                response.Data.Value.Add(message);
            }

            return Ok(response);
        }
    }
}