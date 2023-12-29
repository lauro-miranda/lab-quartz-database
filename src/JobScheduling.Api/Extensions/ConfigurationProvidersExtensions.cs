using StackifyLib;

namespace JobScheduling.Api.Extensions
{
    public static class ConfigurationProvidersExtensions
    {
        public static ConfigurationManager ConfigureProviders(this ConfigurationManager configuration, string environment)
        {
            configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build()
                .ConfigureStackifyLogging();

            return configuration;
        }
    }
}