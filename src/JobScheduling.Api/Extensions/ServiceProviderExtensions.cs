using JobScheduling.Api.BackgoundServices;

namespace JobScheduling.Api.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static void Register(this IServiceProvider provider)
        {
            provider.GetRequiredService<QuartzBackgroundService>().StartAsync(new CancellationToken());
        }
    }
}