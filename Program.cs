using PiGreenhouse.Drivers;
using PiScheduler;
using Serilog;
using System;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace PiGreenhouse
{
    public static class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            try
            {
                Log.Information("Bootstrapping wiring");

                Pi.Init<BootstrapWiringPi>();
                _pumpRelay = new RelayDriver(Pi.Gpio[BcmPin.Gpio00], "Pump", OnStatusChanged);
                var twoMinutes = MillisecondsPerMinute * 2;

                var tasks = new PiTask[]
                {
                    new DoubleRelayTask("Solenoid 3", relayA: _pumpRelay, pinB: Pi.Gpio[BcmPin.Gpio02],
                        assetId: "Solenoid_1", recurrence: MillisecondsPerDay / 2, onTimeInMs: twoMinutes,
                        onStatusChanged: OnStatusChanged),
                    new DoubleRelayTask("Solenoid 1", relayA: _pumpRelay, pinB: Pi.Gpio[BcmPin.Gpio03],
                        assetId: "Solenoid_2", recurrence: MillisecondsPerDay / 2, onTimeInMs: twoMinutes,
                        onStatusChanged: OnStatusChanged),
                    new DoubleRelayTask("Solenoid 2", relayA: _pumpRelay, pinB: Pi.Gpio[BcmPin.Gpio21],
                        assetId: "Solenoid_3", recurrence: MillisecondsPerDay / 2, onTimeInMs: twoMinutes,
                        onStatusChanged: OnStatusChanged),
                    new DoubleRelayTask("Solenoid 4", relayA: _pumpRelay, pinB: Pi.Gpio[BcmPin.Gpio22],
                        assetId: "Solenoid_4", recurrence: MillisecondsPerDay / 2, onTimeInMs: twoMinutes,
                        onStatusChanged: OnStatusChanged),
                    new EnvironmentTask("DHT11", pin: Pi.Gpio[BcmPin.Gpio25], recurrence: twoMinutes,
                        sendIoTMessage: Program.SendIoTMessage)
                };
                Log.Information("Scheduling Tasks", tasks);

                var scheduler = new Scheduler(tasks, granularityInMilliseconds: MillisecondsPerMinute / 12);
                scheduler.Run();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Fatal Error");
                throw;
            }
        }

        private static void OnStatusChanged(string assetId, bool? state)
        {
            if (state != null)
            {
                SendIoTMessage(assetId, state.ToString());
                Log.Information("Relay {AssetId} is {State}", assetId, state.Value ? "on" : "off");
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
        private static RelayDriver? _pumpRelay;
    }
}