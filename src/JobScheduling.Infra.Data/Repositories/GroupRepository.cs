using JobScheduling.Domain.Groups.Models;
using JobScheduling.Domain.Groups.Repositories.Contracts;
using JobScheduling.Infra.Data.Context;
using LM.Infra.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JobScheduling.Infra.Data.Repositories
{
    public class GroupRepository : Repository<Group, JobSchedulingContext>, IGroupRepository
    {
        public GroupRepository(JobSchedulingContext context) : base(context) { }

        public async Task<bool> AnyAsync(string name) 
            => await DbSet.AnyAsync(x => x.Name.ToLower() == name.ToLower());

        public async Task<bool> AnyAsync(Guid code, string name) 
            => await DbSet.AnyAsync(x => x.Code != code && x.Name.ToLower() == name.ToLower());
    }
}