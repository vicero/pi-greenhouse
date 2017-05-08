using PiGreenhouse.Drivers;
using PiScheduler;
using System.Diagnostics;

namespace PiGreenhouse
{
    [DebuggerDisplay("{ _name } { NextOccurrence }")]
    internal sealed class RelayTask : PiTask
    {
        private int _onTimeInMs;
        private string _name;
        private RelayDriver _driver;

        public RelayTask(
            string name,
            int pin,
            int recurrence,
            int onTimeInMs)
            : base(recurrence, name, "Relay", onTimeInMs)
        {
            _name = name;
            _driver = new RelayDriver(pin);
            _onTimeInMs = onTimeInMs;
        }

        protected override void DoWork()
        {
            Debug.WriteLine("Turning relay " + Name + " on.");
            _driver.TurnRelayOn();
            Debug.WriteLine("Relay " + Name + " is " + _driver.State);
        }

        public override void OnComplete()
        {
            Debug.WriteLine("Turning relay " + Name + " off.");
            _driver.TurnRelayOff();
            Debug.WriteLine("Relay " + Name + " is " + _driver.State);
        }
    }
}
