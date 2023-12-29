using JobScheduling.Domain.Jobs.Models;
using LM.Domain.Repositories;
using LM.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobScheduling.Domain.Jobs.Repositories.Contracts
{
    public interface IJobRepository : IRepository<Job>
    {
        Task<Maybe<Job>> FindAsNoTrackingAsync(Guid code);

        Task<List<Job>> GetAllAsNoTrackingAsync();
    }
}