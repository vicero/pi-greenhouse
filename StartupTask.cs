using Windows.ApplicationModel.Background;

using PiScheduler;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace PiGreenhouse
{
    public sealed class StartupTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var scheduler = new Scheduler(Tasks, granularityInMilliseconds: millisecondsPerMinute / 2);
            scheduler.Run();
        }

        private const int millisecondsPerDay = 86400000;
        private const int millisecondsPerMinute = 60000;

        private static readonly PiTask[] Tasks = {
            new DoubleRelayTask("Solenoid 1", pinA: 22, pinB: 23, assetId: "Solenoid_1", recurrence: millisecondsPerDay*1, onTimeInMs: millisecondsPerMinute*3),
            //new RelayTask("Pump 2", pin: 23, recurrence: millisecondsPerDay*1, onTimeInMs: millisecondsPerMinute*2),
            //new RelayTask("Pump 3", pin: 24, recurrence: millisecondsPerDay*1, onTimeInMs: millisecondsPerMinute*2),
            //new RelayTask("Pump 4", pin: 25, recurrence: millisecondsPerDay*1, onTimeInMs: millisecondsPerMinute*2),
            new EnvironmentTask("DHT11", pin: 26, recurrence: millisecondsPerMinute*1)
        };
    }
}
