using PiScheduler;
using Raspberry.IO.GeneralPurpose;

namespace PiGreenhouse
{
    class Program
    {
        static void Main(string[] args)
        {
            var scheduler = new Scheduler(Tasks, granularityInMilliseconds: millisecondsPerMinute / 2);
            scheduler.Run();
        }

        private const int millisecondsPerDay = 86400000;
        private const int millisecondsPerMinute = 60000;

        private static readonly PiTask[] Tasks = {
            new DoubleRelayTask("Solenoid 1", pinA: ConnectorPin.P1Pin22, pinB: ConnectorPin.P1Pin23, assetId: "Solenoid_1", recurrence: millisecondsPerDay*1, onTimeInMs: millisecondsPerMinute*3),
            //new RelayTask("Pump 2", pin: 23, recurrence: millisecondsPerDay*1, onTimeInMs: millisecondsPerMinute*2),
            //new RelayTask("Pump 3", pin: 24, recurrence: millisecondsPerDay*1, onTimeInMs: millisecondsPerMinute*2),
            //new RelayTask("Pump 4", pin: 25, recurrence: millisecondsPerDay*1, onTimeInMs: millisecondsPerMinute*2),
            new EnvironmentTask("DHT11", measurePin: ConnectorPin.P1Pin26, recurrence: millisecondsPerMinute*1)
        };
    }
}