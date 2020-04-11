using PiScheduler;
using System;
using System.Diagnostics;
using Serilog;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.RaspberryIO.Peripherals;

namespace PiGreenhouse
{
    [DebuggerDisplay("{ _name } { NextOccurrence }")]
    internal sealed class EnvironmentTask : PiTask
    {
        private readonly string _name;
        private readonly DhtSensor _driver;
        private readonly Action<string, string> _sendIoTMessage;

        public EnvironmentTask(
            string name,
            IGpioPin pin,
            int recurrence,
            Action<string, string> sendIoTMessage)
            : base(recurrence, name, "Environment")
        {
            _name = name;
            _driver = DhtSensor.Create(DhtType.Dht11, pin);
            _driver.OnDataAvailable += DriverOnOnDataAvailable;
            _sendIoTMessage = sendIoTMessage;
        }

        protected override void DoWork()
        {
            try
            {
                _driver.Start();
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}{System.Environment.NewLine}{ex.StackTrace}");
            }
        }

        public override void OnComplete()
        {
            /* do nothing */
        }

        private void DriverOnOnDataAvailable(object? sender, DhtReadEventArgs e)
        {
            try
            {
                _sendIoTMessage("Temperature", e.Temperature.ToString());
                _sendIoTMessage("Humidity", e.HumidityPercentage.ToString());
                Log.Information("Environment: {Temperature} {Humidity}", e.Temperature, e.HumidityPercentage);
            }
            catch (Exception ex)
            {
                Log.Error($"{ex.Message}{System.Environment.NewLine}{ex.StackTrace}");
            }
        }
    }
}
