using TrafficMonitor.Models;
using TrafficMonitor.Services;

namespace TrafficMonitor_Tests
{
    public class TrafficRateCheckerTests
    {
        [Fact]
        public void CheckAndAlertIfNeeded_ShouldNotRaiseEnteredHighLoadEvent_WhenHitsPerSecondIsLessThanAlertThreshold()
        {
            // Arrange
            var trafficRateChecker = new TrafficRateChecker();
            var logEntries = new List<RequestInfoModel>
            {
                new RequestInfoModel { Section = "home", StatusCode = 200, Timestamp = DateTime.Now }
            };

            bool eventFired = false;
            TrafficRateChecker.EnteredHighLoad += (sender, args) => { eventFired = true; };

            // Act
            trafficRateChecker.CheckAndAlertIfNeeded(logEntries);

            // Assert
            Assert.False(eventFired);
        }

        [Fact]
        public void CheckAndAlertIfNeeded_ShouldRaiseEnteredHighLoadEvent_WhenHitsPerSecondIsGreaterThanAlertThreshold()
        {
            // Arrange
            var trafficRateChecker = new TrafficRateChecker();
            var logEntries = new List<RequestInfoModel>();

            for (int i = 0; i < 2000; i++)
            {
                logEntries.Add(new RequestInfoModel { Section = "home", StatusCode = 200, Timestamp = DateTime.Now });
            }

            bool eventFired = false;
            TrafficRateChecker.EnteredHighLoad += (sender, args) => { eventFired = true; };

            // Act
            trafficRateChecker.CheckAndAlertIfNeeded(logEntries);

            // Assert
            Assert.True(eventFired);
        }

        [Fact]
        public void CheckAndAlertIfNeeded_ShouldRaiseExitedHighLoadEvent_WhenHitsPerSecondIsLessThanAlertThreshold()
        {
            // Arrange
            var trafficRateChecker = new TrafficRateChecker();
            var logEntries = new List<RequestInfoModel>();
            for (int i = 0; i < 2000; i++)
            {
                logEntries.Add(new RequestInfoModel { Section = "home", StatusCode = 200, Timestamp = DateTime.Now });
            }

            bool enteredEventFired = false;
            TrafficRateChecker.EnteredHighLoad += (sender, args) => { enteredEventFired = true; };
            trafficRateChecker.CheckAndAlertIfNeeded(logEntries);

            logEntries.Clear();
            logEntries.Add(new RequestInfoModel { Section = "home", StatusCode = 200, Timestamp = DateTime.Now });
            bool exitedEventFired = false;
            TrafficRateChecker.ExitedHighLoad += (sender, args) => { exitedEventFired = true; };

            // Act
            trafficRateChecker.CheckAndAlertIfNeeded(logEntries);

            // Assert
            Assert.True(enteredEventFired);
            Assert.True(exitedEventFired);
        }
    }
}