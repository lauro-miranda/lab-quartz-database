using JobScheduling.Messages.Requests;
using JobScheduling.Messages.Responses;
using LM.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobScheduling.Domain.Jobs.Services.Contracts
{
    public interface IJobService
    {
        Task<Response<List<JobResponseMessage>>> GetAllAsync();

        Task<Response<JobResponseMessage>> GetAsync(Guid code);

        Task<Response<JobResponseMessage>> CreateAsync(JobRequestMessage message);

        Task<Response<JobResponseMessage>> UpdateAsync(Guid code, JobRequestMessage message);

        Task<Response> DeleteAsync(Guid code);
    }
}