using JobScheduling.ExternalServices.Dtos;
using JobScheduling.ExternalServices.Services.Contracts;
using LM.Responses;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;

namespace JobScheduling.ExternalServices.Services
{
    public class ExternalService : IExternalService
    {
        HttpClient HttpClient { get; }

        public ExternalService(IHttpClientFactory factory)
        {
            HttpClient = factory.CreateClient("Default");
        }

        public async Task<Response> SendAsync(MessageDto dto)
        {
            var response = Response.Create();

            Log.Information($"[SendAsync] Iniciando envio dos dados para a [URL]: {dto.Url}.");

            HttpClient.BaseAddress = new Uri(dto.Url);

            foreach (var header in dto.Headers)
            {
                HttpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            var content = new StringContent(dto.Body, Encoding.UTF8, "application/json");

            var message = await HttpClient.PostAsync(string.Empty, content);

            if (!message.IsSuccessStatusCode)
                Log.Error($"[SendAsync] Falha ao tentar chamar o serviço configurado. " +
                    $"[DTO]: {JsonConvert.SerializeObject(dto)}. " +
                    $"[StatusCode]: {message.IsSuccessStatusCode}. " +
                    $"[Content]: {await message.Content.ReadAsStringAsync()}");

            Log.Information($"[SendAsync] Serviço chamado com sucesso: [DTO]: {JsonConvert.SerializeObject(dto)}.");

            return response;
        }
    }
}