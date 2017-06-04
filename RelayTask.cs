using PiGreenhouse.Drivers;
using PiScheduler;
using Raspberry.IO.GeneralPurpose;
using System;
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
            ConnectorPin pin,
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
            Console.WriteLine("Turning relay " + Name + " on.");
            _driver.TurnRelayOn();
            Console.WriteLine("Relay " + Name + " is " + _driver.State);
        }

        public override void OnComplete()
        {
            Console.WriteLine("Turning relay " + Name + " off.");
            _driver.TurnRelayOff();
            Console.WriteLine("Relay " + Name + " is " + _driver.State);
        }
    }
}
