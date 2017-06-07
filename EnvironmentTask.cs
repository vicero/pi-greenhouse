using att.iot.client;
using PiScheduler;
using Raspberry.IO.Components.Sensors.Temperature.Dht;
using Raspberry.IO.GeneralPurpose;
using System;
using System.Diagnostics;

namespace PiGreenhouse
{
    [DebuggerDisplay("{ _name } { NextOccurrence }")]
    internal sealed class EnvironmentTask : PiTask
    {
        private string _name;
        private Dht11Connection _driver;
        private IDevice _attDevice;

        public EnvironmentTask(
            string name,
            ConnectorPin measurePin,
            int recurrence,
            IDevice attDevice)
            : base(recurrence, name, "Environment")
        {
            _name = name;
            var driver = GpioConnectionSettings.GetBestDriver(GpioConnectionDriverCapabilities.CanChangePinDirectionRapidly);
            var pin = driver.InOut(measurePin);
            _driver = new Dht11Connection(pin, autoStart: false);
            _attDevice = attDevice;
        }

        protected override void DoWork()
        {
            try
            {
                Console.WriteLine("Beginning read from " + Name + ".");
                _driver.Start();
                var reading = _driver.GetData();

                if (reading != null)
                {
                    Console.WriteLine($"Temperature: {reading.Temperature.DegreesCelsius} Humidity: { reading.RelativeHumidity.Percent} AttemptCount: {reading.AttemptCount}");
                    _attDevice.Send("Temperature", reading.Temperature.DegreesCelsius.ToString());
                    _attDevice.Send("Humidity", reading.RelativeHumidity.Percent.ToString());
                }
                else
                {
                    Console.WriteLine("Error reading DHT11 data.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}{System.Environment.NewLine}{ex.StackTrace}");
            }
            
        }

        public override void OnComplete()
        {
            /* do nothing */
        }
    }
}
