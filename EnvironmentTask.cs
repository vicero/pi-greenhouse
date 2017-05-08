using att.iot.client;

using PiGreenhouse.Drivers;
using PiScheduler;

using System.Diagnostics;

namespace PiGreenhouse
{
    [DebuggerDisplay("{ _name } { NextOccurrence }")]
    internal sealed class EnvironmentTask : PiTask
    {
        private string _name;
        private Dht11Driver _driver;
        private IDevice _attDevice;

        public EnvironmentTask(
            string name,
            int pin,
            int recurrence)
            : base(recurrence, name, "Environment")
        {
            _name = name;
            _driver = new Dht11Driver(pin);
            _attDevice = new Device("vicero_hZzesPX4", "2BgYhziU");
            _attDevice.DeviceId = "fHDlCmUC00wifPXl7SiauaT6";
        }

        protected override void DoWork()
        {
            Debug.WriteLine("Reading DHT11 " + Name + " on.");
            var task = _driver.Read();
            task.Wait();
            var reading = task.Result;
            Debug.WriteLine(string.Format("IsValid: {0} Temperature: {1} Humidity: {2}", reading.IsValid, reading.Temperature, reading.Humidity));

            _attDevice.Send("Temperature", reading.Temperature.ToString());
            _attDevice.Send("Humidity", reading.Humidity.ToString());
        }

        public override void OnComplete()
        {
            /* do nothing */
        }
    }
}
