using JobScheduling.Infra.Schedulings.Factories;
using JobScheduling.Infra.Schedulings.Scheduler;
using Quartz.Impl;
using Quartz.Spi;
using Quartz;
using JobScheduling.Infra.Schedulings.Jobs;
using JobScheduling.Api.BackgoundServices;
using JobScheduling.Infra.Schedulings.Services.Contracts;
using JobScheduling.Infra.Schedulings.Services;

namespace JobScheduling.Api.Extensions
{
    public static class JobSchedulingExtensions
    {
        public static void ConfigureScheduling(this IServiceCollection services)
        {
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddSingleton<ICustomScheduler, CustomScheduler>();
            services.AddSingleton<ExtractJob>();
            services.AddSingleton<ISchedulerJobService, SchedulerJobService>();

            services.AddHostedService<QuartzBackgroundService>();
        }
    }
}