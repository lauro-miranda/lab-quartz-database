using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using System;

namespace JobScheduling.Infra.Schedulings.Factories
{
    public class JobFactory : IJobFactory
    {
        IServiceProvider ServiceProvider { get; }

        public JobFactory(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var service = ServiceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;

            return service ?? throw new NullReferenceException($"[NewJob] Job {bundle.JobDetail.JobType} não encontrado.");
        }

        public void ReturnJob(IJob job) => (job as IDisposable)?.Dispose();
    }
}