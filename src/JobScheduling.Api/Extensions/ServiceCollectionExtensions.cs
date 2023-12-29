using JobScheduling.Domain.Groups.Repositories.Contracts;
using JobScheduling.Domain.Groups.Services;
using JobScheduling.Domain.Groups.Services.Contracts;
using JobScheduling.Domain.Histories.Repositories.Contracts;
using JobScheduling.Domain.Histories.Services;
using JobScheduling.Domain.Histories.Services.Contracts;
using JobScheduling.Domain.Jobs.Repositories.Contracts;
using JobScheduling.Domain.Jobs.Services;
using JobScheduling.Domain.Jobs.Services.Contracts;
using JobScheduling.ExternalServices.Services;
using JobScheduling.ExternalServices.Services.Contracts;
using JobScheduling.Infra.Data.Context;
using JobScheduling.Infra.Data.Repositories;
using LM.Domain.UnitOfWork;
using LM.Infra.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace JobScheduling.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureDI(this IServiceCollection services)
        {
            services.AddTransient<IGroupService, GroupService>();
            services.AddTransient<IGroupRepository, GroupRepository>();

            services.AddTransient<IJobService, JobService>();
            services.AddTransient<IJobRepository, JobRepository>();

            services.AddTransient<IHistoryService, HistoryService>();
            services.AddTransient<IHistoryRepository, HistoryRepository>();

            services.AddTransient<IExternalService, ExternalService>();
        }

        public static void ConfigureBD(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork<JobSchedulingContext>>();
            services.AddDbContext<JobSchedulingContext>(options => options.UseSqlServer(configuration.GetConnectionString("Me")));
        }
    }
}