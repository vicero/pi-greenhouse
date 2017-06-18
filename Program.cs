using att.iot.client;
using PiGreenhouse.Drivers;
using PiScheduler;
using Raspberry.IO.GeneralPurpose;
using System;

namespace PiGreenhouse
{
    class Program
    {
        static void Main(string[] args)
        {
            _attDevice = new Device("vicero_hZzesPX4", "2BgYhziU");
            _attDevice.DeviceId = "fHDlCmUC00wifPXl7SiauaT6";

            _pumpRelay = new RelayDriver(ConnectorPin.P1Pin15.ToProcessor(), "Pump", OnStatusChanged);

            var minuteAndAHalf = (int)Math.Round(millisecondsPerMinute * 1.5);

            var tasks = new PiTask[] {
                new DoubleRelayTask("Solenoid 1", relayA: _pumpRelay, pinB: ConnectorPin.P1Pin16.ToProcessor(), assetId: "Solenoid_1", recurrence: millisecondsPerDay/2, onTimeInMs: minuteAndAHalf, onStatusChanged: OnStatusChanged),
                new DoubleRelayTask("Solenoid 2", relayA: _pumpRelay, pinB: ConnectorPin.P1Pin18.ToProcessor(), assetId: "Solenoid_2", recurrence: millisecondsPerDay/2, onTimeInMs: minuteAndAHalf, onStatusChanged: OnStatusChanged),
                new DoubleRelayTask("Solenoid 3", relayA: _pumpRelay, pinB: ConnectorPin.P1Pin22.ToProcessor(), assetId: "Solenoid_3", recurrence: millisecondsPerDay/2, onTimeInMs: minuteAndAHalf, onStatusChanged: OnStatusChanged),
                new DoubleRelayTask("Solenoid 4", relayA: _pumpRelay, pinB: ConnectorPin.P1Pin13.ToProcessor(), assetId: "Solenoid_4", recurrence: millisecondsPerDay/2, onTimeInMs: minuteAndAHalf, onStatusChanged: OnStatusChanged),
                new EnvironmentTask("DHT11", measurePin: ConnectorPin.P1Pin37, recurrence: millisecondsPerMinute*5, attDevice: _attDevice)
            };

            var scheduler = new Scheduler(tasks, granularityInMilliseconds: millisecondsPerMinute / 12);
            scheduler.Run();
        }

        public static void OnStatusChanged(string assetId, bool? state)
        {
            if (state != null)
            {
                try
                {
                    _attDevice.Send(assetId, state.ToString());
                    Console.WriteLine($"Relay {assetId} is {(state.Value ? "on" : "off")}.");
                } catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}{System.Environment.NewLine}{ex.StackTrace}");
                }
            }
        }

        private const int millisecondsPerDay = 86400000;
        private const int millisecondsPerMinute = 60000;

        private static Device _attDevice;
        private static RelayDriver _pumpRelay;
    }
}