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
            ProcessorPin pin,
            string assetId,
            int recurrence,
            int onTimeInMs,
            Action<string, bool?> onStatusChanged)
            : base(recurrence, name, "Relay", onTimeInMs)
        {
            _name = name;
            _driver = new RelayDriver(pin, assetId, onStatusChanged);
            _onTimeInMs = onTimeInMs;
        }

        protected override void DoWork()
        {
            Console.WriteLine("Turning relay " + Name + " on.");
            _driver.TurnRelayOn();
        }

        public override void OnComplete()
        {
            Console.WriteLine("Turning relay " + Name + " off.");
            _driver.TurnRelayOff();
        }
    }
}
