using JobScheduling.Domain.Groups.Repositories.Contracts;
using JobScheduling.Domain.Jobs.Mappers;
using JobScheduling.Domain.Jobs.Models;
using JobScheduling.Domain.Jobs.Repositories.Contracts;
using JobScheduling.Domain.Jobs.Services.Contracts;
using JobScheduling.Messages.Requests;
using JobScheduling.Messages.Responses;
using LM.Domain.UnitOfWork;
using LM.Responses;
using LM.Responses.Extensions;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace JobScheduling.Domain.Jobs.Services
{
    public class JobService : IJobService
    {
        IJobRepository JobRepository { get; }

        IGroupRepository GroupRepository { get; }

        IUnitOfWork Uow { get; }

        public static event Func<Job, CancellationToken, Task>? CreatedOnHandler;

        public static event Func<Job, CancellationToken, Task>? UpdatedOnHandler;

        public static event Func<Job, CancellationToken, Task>? DeletedOnHandler;

        public JobService(IJobRepository jobRepository
            , IGroupRepository groupRepository
            , IUnitOfWork uow)
        {
            JobRepository = jobRepository;
            GroupRepository = groupRepository;
            Uow = uow;
        }

        public async Task<Response<List<JobResponseMessage>>> GetAllAsync()
        {
            var jobs = await JobRepository.GetAllAsNoTrackingAsync();

            return jobs.ToJobResponseMessage();
        }

        public async Task<Response<JobResponseMessage>> GetAsync(Guid code)
        {
            var job = await JobRepository.FindAsNoTrackingAsync(code);

            return job.Value.ToJobResponseMessage();
        }

        public async Task<Response<JobResponseMessage>> CreateAsync(JobRequestMessage message)
        {
            var response = Response<JobResponseMessage>.Create();

            try
            {
                Log.Information($"[JobService][CreateAsync] Iniciando configuração do job. Message: {JsonConvert.SerializeObject(message)}.");

                var group = await GroupRepository.FindAsync(message.GroupCode);

                if (!group.HasValue)
                    return response.WithBusinessError(nameof(message.GroupCode)
                        , $"Não foi possível encontrar o grupo com o código '{message.GroupCode}'.");

                var job = Job.Create(group, message);

                if (job.HasError) return response.WithMessages(job.Messages);

                await JobRepository.AddAsync(job);

                if (!await Uow.CommitAsync())
                    return response.WithCriticalError("Falha ao tentar salvar o grupo.");

                if (CreatedOnHandler != null)
                    await CreatedOnHandler.Invoke(job, new CancellationToken());

                Log.Information($"[JobService][CreateAsync] Finalizando configuração do job. Message: {JsonConvert.SerializeObject(message)}.");

                return response.SetValue(job.Data.Value.ToJobResponseMessage());
            }
            catch (Exception ex)
            {
                var text = $"[JobService][CreateAsync] Falha ao tentar configurar o job. {JsonConvert.SerializeObject(message)}";
                Log.Fatal(ex, text);
                return response.WithCriticalError(text);
            }
        }

        public async Task<Response<JobResponseMessage>> UpdateAsync(Guid code, JobRequestMessage message)
        {
            var response = Response<JobResponseMessage>.Create();

            try
            {
                Log.Information($"[JobService][UpdateAsync] Iniciando a atualização da configuração do job. Message: {JsonConvert.SerializeObject(message)}.");

                var job = await JobRepository.FindAsync(code);

                if (!job.HasValue)
                    return response.WithBusinessError(nameof(code)
                        , $"Não foi possível encontrar o job com o código '{code}'.");

                if (job.Value.JobGroup.Group.Code != message.GroupCode)
                {
                    var group = await GroupRepository.FindAsync(message.GroupCode);

                    if (!group.HasValue)
                        return response.WithBusinessError(nameof(message.GroupCode)
                            , $"Não foi possível encontrar o grupo com o código '{message.GroupCode}'.");

                    if ((response.WithMessages(job.Value.Update(group, message).Messages).HasError))
                        return response;
                }
                else
                {
                    if ((response.WithMessages(job.Value.Update(message).Messages).HasError))
                        return response;
                }

                await JobRepository.UpdateAsync(job);

                if (!await Uow.CommitAsync())
                    return response.WithCriticalError("Falha ao tentar salvar o grupo.");

                if (UpdatedOnHandler != null)
                    await UpdatedOnHandler.Invoke(job, new CancellationToken());

                Log.Information($"[JobService][UpdateAsync] Finalizando a atualização da configuração do job. Message: {JsonConvert.SerializeObject(message)}.");

                return response.SetValue(job.Value.ToJobResponseMessage());
            }
            catch (Exception ex)
            {
                var text = $"[JobService][UpdateAsync] Falha ao tentar atualizar a configuração do job. Message: {JsonConvert.SerializeObject(message)}";
                Log.Fatal(ex, text);
                return response.WithCriticalError(text);
            }
        }

        public async Task<Response> DeleteAsync(Guid code)
        {
            var response = Response<GroupResponseMessage>.Create();

            try
            {
                Log.Information($"[JobService][DeleteAsync] Iniciando a exclusão da configuração do job. Code: {code}.");

                var job = await JobRepository.FindAsync(code);

                if (!job.HasValue)
                    return response.WithBusinessError(nameof(code)
                        , $"Não foi possível encontrar o job com o código '{code}'.");

                job.Value.Delete();

                await JobRepository.UpdateAsync(job);

                if (!await Uow.CommitAsync())
                    return response.WithCriticalError("Falha ao tentar remover o grupo.");

                if (DeletedOnHandler != null)
                    await DeletedOnHandler.Invoke(job, new CancellationToken());

                Log.Information($"[JobService][DeleteAsync] Finalizando a exclusão da configuração do job. Code: {code}.");

                return response;
            }
            catch (Exception ex)
            {
                var text = $"[JobService][DeleteAsync] Falha ao tentar excluir a configuração do job. Code: {code}.";
                Log.Fatal(ex, text);
                return response.WithCriticalError(text);
            }
        }
    }
}