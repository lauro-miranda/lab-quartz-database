using Serilog.Events;
using Serilog;

namespace JobScheduling.Api.Extensions
{
    public static class StackifyConfigurationExtensions
    {
        public static ConfigurationManager ConfigureLogging(this ConfigurationManager configuration)
        {
            StackifyLib.Utils.StackifyAPILogger.LogEnabled = true;

            Log.Logger = new LoggerConfiguration()
                            .Enrich.FromLogContext()
                            .ReadFrom.Configuration(configuration)
                            .WriteTo.Console(LogEventLevel.Debug)
                            .WriteTo.Stackify(restrictedToMinimumLevel: LogEventLevel.Information)
                            .CreateLogger();

            return configuration;
        }
    }
}