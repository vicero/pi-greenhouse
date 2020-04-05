using PiGreenhouse.Drivers;
using PiScheduler;
using System;
using Unosquare.RaspberryIO;

namespace PiGreenhouse
{
    public static class Program
    {
        static void Main(string[] args)
        {
            var twoMinutes = (int)Math.Round(MillisecondsPerMinute * 2m);

            var tasks = new PiTask[] {
                new DoubleRelayTask("Solenoid 3", relayA: PumpRelay, pinB: Pi.Gpio[13], assetId: "Solenoid_1", recurrence: MillisecondsPerDay/2, onTimeInMs: twoMinutes, onStatusChanged: OnStatusChanged),
                new DoubleRelayTask("Solenoid 1", relayA: PumpRelay, pinB: Pi.Gpio[16], assetId: "Solenoid_2", recurrence: MillisecondsPerDay/2, onTimeInMs: twoMinutes, onStatusChanged: OnStatusChanged),
                new DoubleRelayTask("Solenoid 2", relayA: PumpRelay, pinB: Pi.Gpio[18], assetId: "Solenoid_3", recurrence: MillisecondsPerDay/2, onTimeInMs: twoMinutes, onStatusChanged: OnStatusChanged),
                new DoubleRelayTask("Solenoid 4", relayA: PumpRelay, pinB: Pi.Gpio[22], assetId: "Solenoid_4", recurrence: MillisecondsPerDay/2, onTimeInMs: twoMinutes, onStatusChanged: OnStatusChanged),
                new EnvironmentTask("DHT11", pin: Pi.Gpio[37], recurrence: MillisecondsPerMinute*5, sendIoTMessage: Program.SendIoTMessage)
            };

            var scheduler = new Scheduler(tasks, granularityInMilliseconds: MillisecondsPerMinute / 12);
            scheduler.Run();
        }

        private static void OnStatusChanged(string assetId, bool? state)
        {
            if (state != null)
            {
                SendIoTMessage(assetId, state.ToString());
                Console.WriteLine($"{DateTime.Now.ToUniversalTime()} Relay {assetId} is {(state.Value ? "on" : "off")}.");
            }
        }

        private static void SendIoTMessage(string assetId, string? message)
        {
            // TODO: restore and test
            // try
            // {
            //     if (!Program._attDevice.HasValue)
            //     {
            //         var device = new Device("vicero_hZzesPX4", "2BgYhziU", apiUri: "https://api.allthingstalk.io");
            //         device.DeviceId = "fHDlCmUC00wifPXl7SiauaT6";
            //         Program._attDevice = Maybe<Device>.Some(device);
            //     }
            //     Program._attDevice.Value.Send(assetId, message);
            // }
            // catch (Exception ex)
            // {
            //     // exceptions sending IoT messages should not take down the application
            //     Console.WriteLine(ex.ToString());
            // }
        }

        //public static void SubscribeToActuator(string actuatorAssetId, Scheduler scheduler, PiTask task)
        //{
        //    // TODO: restore and test
        //    try
        //    {
        //        if (!Program._attDevice.HasValue)
        //        {
        //            var device = new Device("vicero_hZzesPX4", "2BgYhziU", apiUri: "https://api.allthingstalk.io");
        //            device.DeviceId = "fHDlCmUC00wifPXl7SiauaT6";
        //            Program._attDevice = Maybe<Device>.Some(device);
        //        }
        //        Program._attDevice.Value.ActuatorValue += (sender, e) =>
        //        {
        //            if (e.Asset == actuatorAssetId && e.Value.Value<bool>("value"))
        //            {
        //                scheduler.RunTask(task);
        //            }
        //            Program._attDevice.Value.UpdateAsset(actuatorAssetId, JObject.FromObject(new { value = false }));
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        // exceptions sending IoT messages should not take down the application
        //        Console.WriteLine(ex.ToString());
        //    }
        //}

        private const int MillisecondsPerDay = 86400000;
        private const int MillisecondsPerMinute = 60000;

        // private static Maybe<Device> _attDevice;
        private static readonly RelayDriver PumpRelay = new RelayDriver(Pi.Gpio[15], "Pump", OnStatusChanged);
    }
}