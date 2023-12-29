using JobScheduling.Messages.Requests;
using JobScheduling.Messages.Responses;
using LM.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobScheduling.Domain.Groups.Services.Contracts
{
    public interface IGroupService
    {
        Task<Response<List<GroupResponseMessage>>> GetAllAsync();

        Task<Response<GroupResponseMessage>> GetAsync(Guid code);

        Task<Response<GroupResponseMessage>> CreateAsync(GroupRequestMessage message);

        Task<Response<GroupResponseMessage>> UpdateAsync(Guid code, GroupRequestMessage message);

        Task<Response> DeleteAsync(Guid code);
    }
}