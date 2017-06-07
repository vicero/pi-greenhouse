using PiGreenhouse.Drivers;
using PiScheduler;
using Raspberry.IO.GeneralPurpose;

using System;
using System.Diagnostics;

namespace PiGreenhouse
{
    [DebuggerDisplay("{ _name } { NextOccurrence }")]
    internal sealed class DoubleRelayTask : PiTask
    {
        private int _onTimeInMs;
        private string _name;
        private RelayDriver _relayA;
        private RelayDriver _relayB;

        public DoubleRelayTask(
            string name,
            RelayDriver relayA,
            ProcessorPin pinB,
            string assetId,
            int recurrence,
            int onTimeInMs,
            Action<string, bool?> onStatusChanged)
            : base(recurrence, name, "Relay", onTimeInMs)
        {
            _name = name;
            _relayA = relayA;
            _relayB = new RelayDriver(pinB, assetId, onStatusChanged);
            _onTimeInMs = onTimeInMs;
        }

        protected override void DoWork()
        {
            Console.WriteLine($"Turning relay {_relayA.AssetId} on.");
            _relayA.TurnRelayOn();

            Console.WriteLine($"Turning relay {_relayB.AssetId} on.");
            _relayB.TurnRelayOn();
        }

        public override void OnComplete()
        {
            Console.WriteLine($"Turning relay {_relayA.AssetId} off.");
            _relayA.TurnRelayOff();

            Console.WriteLine($"Turning relay {_relayB.AssetId} off.");
            _relayB.TurnRelayOff();
        }
    }
}
