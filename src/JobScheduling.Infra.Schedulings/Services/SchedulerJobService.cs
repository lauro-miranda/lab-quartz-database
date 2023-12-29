using JobScheduling.Domain.Jobs.Models;
using JobScheduling.Infra.Schedulings.Scheduler;
using JobScheduling.Infra.Schedulings.Services.Contracts;
using LM.Domain.Helpers;
using LM.Responses;
using LM.Responses.Extensions;
using Quartz;
using Quartz.Impl.Matchers;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JobScheduling.Infra.Schedulings.Services
{
    public class SchedulerJobService : ISchedulerJobService
    {
        ICustomScheduler Scheduler { get; }

        public SchedulerJobService(ICustomScheduler scheduler)
        {
            Scheduler = scheduler;
        }

        public async Task<Response> CreateAsync(Job job, CancellationToken cancellationToken)
        {
            var response = Response.Create();

            if (await CheckExists(job, cancellationToken))
                return response.WithBusinessError($"Job '{job.Name}' já existe.");

            Response<IJobDetail> detail = CreateDetail(job);
            if (detail.HasError) return response.WithMessages(detail.Messages);

            TriggerBuilder triggerBuilder = CreateTrigger(job);

            CreateCron(job, triggerBuilder);

            await Scheduler.Value.ScheduleJob(detail.Data.Value, triggerBuilder.Build(), cancellationToken);

            return response;
        }

        public async Task<Response> UpdateAsync(Job job, CancellationToken cancellationToken)
        {
            var response = Response.Create();

            if (!await CheckExists(job, cancellationToken))
                return response.WithBusinessError($"Job '{job.Name}' não encontrado.");

            Response<IJobDetail> detail = CreateDetail(job);
            if (detail.HasError) return response.WithMessages(detail.Messages);

            TriggerBuilder triggerBuilder = CreateTrigger(job);

            CreateCron(job, triggerBuilder);

            await Scheduler.Value.RescheduleJob(new TriggerKey($"{job.Code}"), triggerBuilder.Build());

            return response;
        }

        public async Task<Response> DeleteAsync(Job job, CancellationToken cancellationToken)
        {
            var response = Response.Create();

            if (!await CheckExists(job, cancellationToken))
                return response.WithBusinessError($"Job '{job.Name}' não encontrado.");

            await Scheduler.Value.DeleteJob(await GetJobKey(job, cancellationToken));

            return response;
        }

        #region ' private methods '

        static Response<IJobDetail> CreateDetail(Job job)
        {
            var response = Response<IJobDetail>.Create();

            var ass = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes()).Where(t => String.Equals(t.Name, job.Name, StringComparison.Ordinal))
                .First();

            if (ass == null || string.IsNullOrEmpty(ass.AssemblyQualifiedName))
            {
                return response.WithBusinessError($"Não foi possível encontrar o Job '{job.Name}'.");
            }

            var type = Type.GetType(ass.AssemblyQualifiedName);

            if (type == null) throw new NullReferenceException(nameof(type));

            var detail = JobBuilder
                .Create(type)
                .WithIdentity(job.KeyName, job.JobGroup.Group.Name)
                .UsingJobData($"{nameof(Job.Code)}", job.Code)
                .UsingJobData($"{nameof(Job.Data)}", job.Data)
                .Build();

            return response.SetValue(detail);
        }

        static TriggerBuilder CreateTrigger(Job job)
        {
            return TriggerBuilder
                .Create()
                .WithIdentity(Guid.NewGuid().ToString(), job.JobGroup.Group.Name);
        }

        static void CreateCron(Job job, TriggerBuilder triggerBuilder)
        {
            var cron = new CronExpression(job.CronExpression);

            if (cron != null)
            {
                var nextValidTime = cron.GetNextValidTimeAfter(DateTimeOffset.Now);

                if (nextValidTime != null)
                {
                    triggerBuilder.WithCronSchedule(job.CronExpression).StartAt(nextValidTime.Value);
                }
            }
        }

        async Task<bool> CheckExists(Job job, CancellationToken cancellationToken)
        {
            var currentKey = await GetJobKey(job, cancellationToken);

            return currentKey != null && await Scheduler.Value.CheckExists(currentKey, cancellationToken);
        }

        async Task<JobKey> GetJobKey(Job job, CancellationToken cancellationToken)
        {
            var matcher = GroupMatcher<JobKey>.GroupEquals(job.JobGroup.Group.Name);
            var executionJobs = await Scheduler.Value.GetJobKeys(matcher, cancellationToken);
            return executionJobs.Where(j => j.Name == job.KeyName).SingleOrDefault();
        }

        #endregion
    }
}