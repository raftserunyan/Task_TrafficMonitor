using System.Buffers;
using System.Globalization;
using TrafficMonitor.Helpers;
using TrafficMonitor.Models;
using TrafficMonitor.Services;

namespace HttpTrafficMonitor
{
    class Program
    {
        private static string _applicationDomain = "127.0.0.1";
        private static string _logFilePath = "tmp/access.log";

        private static TimeSpan _executionInverval = TimeSpan.FromSeconds(10);

        static async Task Main(string[] args)
        {
            ApplyConfigurationsFromFile();
            var trafficRateChecker = new TrafficRateChecker();

            while (true)
            {
                var logEntries = GetRequestsFromTrafficLogs();
                var sectionStats = CalculateStatisticsForEntries(logEntries);
                var totalStats = new TotalStatsModel
                {
                    Hits = sectionStats.Sum(section => section.Hits),
                    Successes = sectionStats.Sum(section => section.Successes),
                    Errors = sectionStats.Sum(section => section.Errors)
                };

                ConsoleLogger.PrintResultsTable(sectionStats, ref totalStats);

                trafficRateChecker.CheckAndAlertIfNeeded(logEntries);

                await Task.Delay(_executionInverval);
            }
        }

        private static void ApplyConfigurationsFromFile()
        {
            var config = InstanceFactory.CreateAppConfigurationBuilder();

            if (config != null)
            {
                _logFilePath = config["logFilePath"];
                _executionInverval = TimeSpan.FromSeconds(int.Parse(config["executionIntervalInSeconds"]));
                _applicationDomain = config["applicationDomain"];
            }
        }

        private static IEnumerable<StatsModel> CalculateStatisticsForEntries(IEnumerable<RequestInfoModel> logEntries)
        {
            return logEntries.GroupBy(entry => entry.Section)
                    .Select(group => new StatsModel
                    {
                        Section = group.Key,
                        Hits = group.Count(),
                        Successes = group.Count(entry => entry.StatusCode >= 200 && entry.StatusCode < 300),
                        Errors = group.Count(entry => entry.StatusCode >= 400 && entry.StatusCode < 600)
                    });
        }

        private static IEnumerable<RequestInfoModel> GetRequestsFromTrafficLogs()
        {
            string[] parts = ArrayPool<string>.Shared.Rent(10);

            var result = File.ReadLines(_logFilePath)
                     .Where(line => line.StartsWith(_applicationDomain))
                     .Select(line =>
                     {
                         parts = line.Split(' ');
                         return new RequestInfoModel
                         {
                             Section = GetSection(parts[6]),
                             StatusCode = int.Parse(parts[8]),
                             Timestamp = DateTime.ParseExact(parts[3].Substring(1), "dd/MMM/yyyy:HH:mm:ss", CultureInfo.InvariantCulture)
                         };
                     });

            ArrayPool<string>.Shared.Return(parts);

            return result;
        }

        private static string GetSection(string resource)
        {
            var secondSlashIndex = resource.IndexOf('/', resource.IndexOf('/') + 1);

            if (secondSlashIndex > 0)
                return resource.Substring(0, secondSlashIndex);
            else
                return resource;
        }
    }
}