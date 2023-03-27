using TrafficMonitor.Models;

namespace TrafficMonitor.Services
{
    internal class ConsoleLogger
    {
        public ConsoleLogger()
        {
            // Subscribe to the TrafficRateChecker events
            TrafficRateChecker.EnteredHighLoad += OnEnteredHighLoad;
            TrafficRateChecker.ExitedHighLoad += OnExitedHighLoad;
        }

        private void OnEnteredHighLoad(object sender, TrafficLoadEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nHigh traffic generated an alert - hits = {e.HitsPerSecond}, triggered at {e.Time}\n");
            Console.ResetColor();
        }

        private void OnExitedHighLoad(object sender, TrafficLoadEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nTraffic rate has been recovered - hits = {e.HitsPerSecond}, triggered at {e.Time}\n");
            Console.ResetColor();
        }

        public static void PrintResultsTable(IEnumerable<StatsModel> sectionStats, ref TotalStatsModel totalStats)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Section\t\tHits\t\tSuccesses\tErrors");
            Console.ResetColor();

            foreach (var sectionStat in sectionStats.OrderByDescending(section => section.Hits))
            {
                Console.WriteLine($"{sectionStat.Section}\t\t{sectionStat.Hits}\t\t{sectionStat.Successes}\t\t{sectionStat.Errors}");
            }

            Console.WriteLine($"Total\t\t{totalStats.Hits}\t\t{totalStats.Successes}\t\t{totalStats.Errors}");
        }
    }
}
