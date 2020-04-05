using PiGreenhouse.Drivers;
using PiScheduler;
using System;
using System.Diagnostics;
using Unosquare.RaspberryIO.Abstractions;

namespace PiGreenhouse
{
    [DebuggerDisplay("{ _name } { NextOccurrence }")]
    internal sealed class RelayTask : PiTask
    {
        private string _name;
        private readonly RelayDriver _driver;

        public RelayTask(
            string name,
            IGpioPin pin,
            string assetId,
            int recurrence,
            int onTimeInMs,
            Action<string, bool?> onStatusChanged)
            : base(recurrence, name, "Relay", onTimeInMs)
        {
            _name = name;
            _driver = new RelayDriver(pin, assetId, onStatusChanged);
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
