using JobScheduling.Domain.Jobs.Models;
using JobScheduling.Domain.Jobs.Repositories.Contracts;
using JobScheduling.Infra.Data.Context;
using LM.Infra.Repositories;
using LM.Responses;
using Microsoft.EntityFrameworkCore;

namespace JobScheduling.Infra.Data.Repositories
{
    public class JobRepository : Repository<Job, JobSchedulingContext>, IJobRepository
    {
        public JobRepository(JobSchedulingContext context) : base(context) { }

        public override async Task<Maybe<Job>> FindAsync(Guid code)
        {
            var job = await DbSet
                .Include(x => x.JobGroup)
                    .ThenInclude(x => x.Group)
                .FirstOrDefaultAsync(x => x.Code == code);

            if (job == null) return Maybe<Job>.Create();

            return job;
        }

        public async Task<Maybe<Job>> FindAsNoTrackingAsync(Guid code)
        {
            var job = await DbSet.AsNoTracking()
                .Include(x => x.JobGroup)
                    .ThenInclude(x => x.Group)
                .FirstOrDefaultAsync(x => x.Code == code);

            if (job == null) return Maybe<Job>.Create();

            return job;
        }

        public override async Task<List<Job>> GetAllAsync()
        {
            return await DbSet
                .Include(x => x.JobGroup)
                    .ThenInclude(x => x.Group)
                .ToListAsync();
        }

        public async Task<List<Job>> GetAllAsNoTrackingAsync()
        {
            return await DbSet.AsNoTracking()
                .Include(x => x.JobGroup)
                    .ThenInclude(x => x.Group)
                .ToListAsync();
        }
    }
}