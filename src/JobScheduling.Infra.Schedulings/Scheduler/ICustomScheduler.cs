using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Spi;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduling.Infra.Schedulings.Scheduler
{
    public interface ICustomScheduler
    {
        IScheduler Value { get; }

        Task ExecuteAsync(string jobName, string groupName);

        Task<List<JobKey>> GetExecutionJobsAsync(string groupName);
    }

    public class CustomScheduler : ICustomScheduler
    {
        public IScheduler Value { get; }

        public CustomScheduler(ISchedulerFactory schedulerFactory
            , IJobFactory factory)
        {
            Value = schedulerFactory.GetScheduler().GetAwaiter().GetResult();
            Value.JobFactory = factory;
        }

        public async Task ExecuteAsync(string jobName, string groupName)
        {
            var matcher = GroupMatcher<JobKey>.GroupEquals(groupName);
            var executionJobs = await Value.GetJobKeys(matcher);

            var key = executionJobs.Where(j => j.Name == jobName).SingleOrDefault();

            if (key == null) return;

            await Value.TriggerJob(key);
        }

        public async Task<List<JobKey>> GetExecutionJobsAsync(string groupName)
        {
            var matcher = GroupMatcher<JobKey>.GroupEquals(groupName);

            return (await Value.GetJobKeys(matcher)).ToList();
        }
    }
}