using PiGreenhouse.Drivers;
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

        private static readonly RelayDriver PumpRelay = new RelayDriver(ConnectorPin.P1Pin15);

        private static readonly PiTask[] Tasks = {
            new DoubleRelayTask("Solenoid 1", relayA: PumpRelay, pinB: ConnectorPin.P1Pin16, assetId: "Solenoid_1", recurrence: millisecondsPerMinute*5, onTimeInMs: millisecondsPerMinute*1),
            new DoubleRelayTask("Solenoid 2", relayA: PumpRelay, pinB: ConnectorPin.P1Pin18, assetId: "Solenoid_2", recurrence: millisecondsPerMinute*5, onTimeInMs: millisecondsPerMinute*1),
            new DoubleRelayTask("Solenoid 3", relayA: PumpRelay, pinB: ConnectorPin.P1Pin22, assetId: "Solenoid_3", recurrence: millisecondsPerMinute*5, onTimeInMs: millisecondsPerMinute*1),
            new EnvironmentTask("DHT11", measurePin: ConnectorPin.P1Pin37, recurrence: millisecondsPerMinute*1)
        };
    }
}