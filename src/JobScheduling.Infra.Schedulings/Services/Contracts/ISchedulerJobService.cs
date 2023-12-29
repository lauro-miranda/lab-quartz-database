using JobScheduling.Domain.Jobs.Models;
using LM.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace JobScheduling.Infra.Schedulings.Services.Contracts
{
    public interface ISchedulerJobService
    {
        Task<Response> CreateAsync(Job job, CancellationToken cancellationToken);

        Task<Response> UpdateAsync(Job job, CancellationToken cancellationToken);

        Task<Response> DeleteAsync(Job job, CancellationToken cancellationToken);
    }
}