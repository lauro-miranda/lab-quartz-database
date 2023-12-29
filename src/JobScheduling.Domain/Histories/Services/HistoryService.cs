using JobScheduling.Domain.Histories.Mappers;
using JobScheduling.Domain.Histories.Models;
using JobScheduling.Domain.Histories.Repositories.Contracts;
using JobScheduling.Domain.Histories.Services.Contracts;
using JobScheduling.Domain.Jobs.Models;
using JobScheduling.ExternalServices.Services.Contracts;
using JobScheduling.Messages.Responses;
using LM.Domain.UnitOfWork;
using LM.Responses;
using LM.Responses.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduling.Domain.Histories.Services
{
    public class HistoryService : IHistoryService
    {
        IHistoryRepository HistoryRepository { get; }

        IUnitOfWork Uow { get; }

        IExternalService ExternalService { get; }

        public HistoryService(IHistoryRepository historyRepository
            , IUnitOfWork uow
            , IExternalService externalService)
        {
            HistoryRepository = historyRepository;
            Uow = uow;
            ExternalService = externalService;
        }

        public async Task<Response<List<HistoryResponseMessage>>> GetByJobCodeAsync(Guid code)
        {
            var response = Response<List<HistoryResponseMessage>>.Create();

            var histories = await HistoryRepository.GetByJobCodeAsync(code);

            if (!histories.Any()) return response;

            return response.SetValue(histories.ToHistoryResponseMessage());
        }

        public async Task<Response> RunAsync(Job job)
        {
            var response = Response.Create();

            if (job == null) return response.WithBusinessError(nameof(job)
                , "O job não foi informado.");

            var history = History.Create(job);

            if (history.HasError) return response.WithMessages(history.Messages);

            var sended = await ExternalService.SendAsync(job.ToMessageDto());

            if (sended.HasError) response.WithMessages(sended.Messages);

            await HistoryRepository.AddAsync(history);

            if (!await Uow.CommitAsync())
                return response.WithCriticalError($"Falha ao tentar salvar o histórico. " +
                    $"[History]: {JsonConvert.SerializeObject(history.Data.Value)}");

            return response;
        }
    }
}