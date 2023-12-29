using JobScheduling.Domain.Histories.Models;
using LM.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobScheduling.Domain.Histories.Repositories.Contracts
{
    public interface IHistoryRepository : IRepository<History>
    {
        Task<List<History>> GetByJobCodeAsync(Guid code);
    }
}