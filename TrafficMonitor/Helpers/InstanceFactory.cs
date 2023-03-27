using Microsoft.Extensions.Configuration;

namespace TrafficMonitor.Helpers
{
    internal static class InstanceFactory
    {
        public static IConfiguration CreateAppConfigurationBuilder()
        {
            IConfiguration config = null;

            try
            {
                config = new ConfigurationBuilder()
                        .AddJsonFile("configuration.json")
                        .Build();
            }
            catch (Exception)
            {
            }

            return config;
        }
    }
}
