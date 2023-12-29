using JobScheduling.Domain.Jobs.Models;
using JobScheduling.Messages.Responses;
using LM.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobScheduling.Domain.Histories.Services.Contracts
{
    public interface IHistoryService
    {
        Task<Response<List<HistoryResponseMessage>>> GetByJobCodeAsync(Guid code);

        Task<Response> RunAsync(Job job);
    }
}