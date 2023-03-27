using TrafficMonitor.Helpers;
using TrafficMonitor.Models;

namespace TrafficMonitor.Services
{
    public class TrafficRateChecker
    {
        private static bool _isUnderHighLoad = false;
        private static int _alertThreshold = 10;
        private static TimeSpan _alertInterval = TimeSpan.FromSeconds(120);

        // Define two events: one for when we enter high load, and one for when we exit high load
        public static event EventHandler<TrafficLoadEventArgs> EnteredHighLoad;
        public static event EventHandler<TrafficLoadEventArgs> ExitedHighLoad;

        public TrafficRateChecker()
        {
            var config = InstanceFactory.CreateAppConfigurationBuilder();

            if (config != null)
            {
                _alertThreshold = int.Parse(config["alertThreshold"]);
                _alertInterval = TimeSpan.FromSeconds(int.Parse(config["alertIntervalInSeconds"]));
            }
        }

        public void CheckAndAlertIfNeeded(IEnumerable<RequestInfoModel> logEntries)
        {
            // Save the current time
            var dtNow = DateTime.Now;

            var xMinutesBeforeNow = dtNow - _alertInterval;
            var totalHitsForLastXMinutes = logEntries.Count(entry => entry.Timestamp.ToLocalTime() > xMinutesBeforeNow);
            var hitsPerSecond = totalHitsForLastXMinutes / (int)_alertInterval.TotalSeconds;

            // If we were not under high load but we are now
            if (!_isUnderHighLoad && hitsPerSecond > _alertThreshold)
            {
                EnteredHighLoad?.Invoke(this, new TrafficLoadEventArgs { HitsPerSecond = hitsPerSecond, Time = dtNow });
                _isUnderHighLoad = true;
            }
            // If we were under high load but we aren't anymore
            else if (_isUnderHighLoad && hitsPerSecond <= _alertThreshold)
            {
                ExitedHighLoad?.Invoke(this, new TrafficLoadEventArgs { HitsPerSecond = hitsPerSecond, Time = dtNow });
                _isUnderHighLoad = false;
            }
        }
    }

    public class TrafficLoadEventArgs : EventArgs
    {
        public int HitsPerSecond { get; set; }
        public DateTime Time { get; set; }
    }
}