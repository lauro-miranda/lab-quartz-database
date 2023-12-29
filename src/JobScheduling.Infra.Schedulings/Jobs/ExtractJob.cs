using JobScheduling.Domain.Histories.Services.Contracts;
using JobScheduling.Domain.Jobs.Models;
using JobScheduling.Domain.Jobs.Repositories.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduling.Infra.Schedulings.Jobs
{
    public class ExtractJob : IJob
    {
        IServiceScopeFactory ScopeFactory { get; }

        public ExtractJob(IServiceScopeFactory scopeFactory)
        {
            ScopeFactory = scopeFactory;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                if (!context.JobDetail.JobDataMap.TryGetGuid(nameof(Job.Code), out var code))
                {
                    Log.Error($"[ExtractJob][Execute] Código do Job não configurado.");
                    return;
                }

                var data = context.JobDetail.JobDataMap.GetString(nameof(Job.Data));

                if (string.IsNullOrEmpty(data))
                {
                    Log.Error($"[ExtractJob][Execute] Nenhum dado foi configurado.");
                    return;
                }

                using (var scope = ScopeFactory.CreateScope())
                {
                    var service = scope.ServiceProvider.GetRequiredService<IHistoryService>();

                    var jobRepository = scope.ServiceProvider.GetRequiredService<IJobRepository>();

                    var job = await jobRepository.FindAsync(code);

                    if (!job.HasValue) 
                    {
                        Log.Error($"Não foi possível encontrar o job com o código '{code}'.");
                        return;
                    }

                    var response = await service.RunAsync(job);

                    if (response.HasError)
                    {
                        Log.Error($"[ExtractJob][Execute] Houveram falhas na execução do Job. {string.Join(" - ", response.Messages.Select(m => m.Text))}");
                        return;
                    }
                }

                Log.Information($"[ExtractJob][Execute] Job executado com sucesso.");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"[ExtractJob][Execute] Falha ao tentar executar o Job. {ex.Message}");
            }
        }
    }
}