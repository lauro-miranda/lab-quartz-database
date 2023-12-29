using JobScheduling.Domain.Jobs.Models;
using JobScheduling.Domain.Jobs.Repositories.Contracts;
using JobScheduling.Domain.Jobs.Services;
using JobScheduling.Infra.Schedulings.Scheduler;
using JobScheduling.Infra.Schedulings.Services.Contracts;
using LM.Responses;
using Newtonsoft.Json;
using Serilog;

namespace JobScheduling.Api.BackgoundServices
{
    public class QuartzBackgroundService : IHostedService
    {
        ICustomScheduler Scheduler { get; }

        IServiceScopeFactory ScopeFactory { get; }

        ISchedulerJobService SchedulerJobService { get; }

        public QuartzBackgroundService(ICustomScheduler customScheduler
            , IServiceScopeFactory scopeFactory
            , ISchedulerJobService schedulerJobService)
        {
            Scheduler = customScheduler;
            ScopeFactory = scopeFactory;
            SchedulerJobService = schedulerJobService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                Log.Information($"[QuartzBackgroundService][ExecuteAsync] Iniciando serviço.");

                await Scheduler.Value.Start(cancellationToken);

                JobService.CreatedOnHandler += JobService_CreatedOnHandler;

                JobService.UpdatedOnHandler += JobService_UpdatedOnHandler;

                JobService.DeletedOnHandler += JobService_DeletedOnHandler;

                await Schedule(cancellationToken);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"[QuartzBackgroundService][ExecuteAsync] {ex.Message}.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            JobService.CreatedOnHandler -= JobService_CreatedOnHandler;

            JobService.UpdatedOnHandler -= JobService_UpdatedOnHandler;

            JobService.DeletedOnHandler -= JobService_DeletedOnHandler;

            Log.Information($"[QuartzBackgroundService][StopAsync] Serviço finalizado.");

            return Task.CompletedTask;
        }

        async Task JobService_CreatedOnHandler(Job job, CancellationToken cancellationToken)
        {
            await TryInvokeAsync(() => SchedulerJobService.CreateAsync(job, cancellationToken));
        }

        async Task JobService_UpdatedOnHandler(Job job, CancellationToken cancellationToken)
        {
            await TryInvokeAsync(() => SchedulerJobService.UpdateAsync(job, cancellationToken));
        }

        async Task JobService_DeletedOnHandler(Job job, CancellationToken cancellationToken)
        {
            await TryInvokeAsync(() => SchedulerJobService.DeleteAsync(job, cancellationToken));
        }

        static async Task TryInvokeAsync(Func<Task<Response>> func)
        {
            try
            {
                var response = await func.Invoke();

                if (response.HasError)
                    Log.Error($"[TryInvokeAsync] Falha ao tentar executar a operação. Response: {JsonConvert.SerializeObject(response)}");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"[TryInvokeAsync] Falha ao tentar executar a operação.");
            }
        }

        async Task Schedule(CancellationToken cancellationToken)
        {
            using var scope = ScopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IJobRepository>();

            var jobs = await repository.GetAllAsync();

            foreach (var job in jobs)
            {
                await SchedulerJobService.CreateAsync(job, cancellationToken);
            }
        }
    }
}