using JobScheduling.Domain.Histories.Models;
using JobScheduling.Domain.Histories.Repositories.Contracts;
using JobScheduling.Infra.Data.Context;
using LM.Infra.Repositories;
using Microsoft.EntityFrameworkCore;

namespace JobScheduling.Infra.Data.Repositories
{
    public class HistoryRepository : Repository<History, JobSchedulingContext>, IHistoryRepository
    {
        public HistoryRepository(JobSchedulingContext context) : base(context) { }

        public async Task<List<History>> GetByJobCodeAsync(Guid code)
        {
            return await DbSet
                .Include(x => x.HistoryJob)
                    .ThenInclude(x => x.Job)
                .Where(x => x.HistoryJob != null && x.HistoryJob.Job != null && x.HistoryJob.Job.Code == code)
                .ToListAsync();
        }
    }
}