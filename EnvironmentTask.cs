using att.iot.client;
using PiScheduler;
using Raspberry.IO.Components.Sensors.Temperature.Dht;
using Raspberry.IO.GeneralPurpose;
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
            int recurrence)
            : base(recurrence, name, "Environment")
        {
            _name = name;
            var driver = GpioConnectionSettings.GetBestDriver(GpioConnectionDriverCapabilities.CanChangePinDirectionRapidly);
            var pin = driver.InOut(measurePin);
            _driver = new Dht11Connection(pin);
            _attDevice = new Device("vicero_hZzesPX4", "2BgYhziU");
            _attDevice.DeviceId = "fHDlCmUC00wifPXl7SiauaT6";
        }

        protected override void DoWork()
        {
            Debug.WriteLine("Reading DHT11 " + Name + " on.");
            var reading = _driver.GetData();
            Debug.WriteLine(string.Format("AttemptCount: {0} Temperature: {1} Humidity: {2}", reading.AttemptCount, reading.Temperature.DegreesCelsius, reading.RelativeHumidity.Percent));

            _attDevice.Send("Temperature", reading.Temperature.DegreesCelsius.ToString());
            _attDevice.Send("Humidity", reading.RelativeHumidity.Percent.ToString());
        }

        public override void OnComplete()
        {
            /* do nothing */
        }
    }
}
