using JobScheduling.Domain.Groups.Models;
using LM.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace JobScheduling.Domain.Groups.Repositories.Contracts
{
    public interface IGroupRepository : IRepository<Group>
    {
        Task<bool> AnyAsync(string name);

        Task<bool> AnyAsync(Guid code, string name);
    }
}