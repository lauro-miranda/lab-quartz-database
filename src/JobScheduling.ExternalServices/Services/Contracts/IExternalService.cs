using JobScheduling.ExternalServices.Dtos;
using LM.Responses;
using System.Threading.Tasks;

namespace JobScheduling.ExternalServices.Services.Contracts
{
    public interface IExternalService
    {
        Task<Response> SendAsync(MessageDto dto);
    }
}