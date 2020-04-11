using System;
using System.Diagnostics;

using PiGreenhouse.Drivers;
using PiScheduler;
using Serilog;
using Unosquare.RaspberryIO.Abstractions;

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
            IGpioPin pinB,
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
            Log.Information($"Turning relay {_relayA.AssetId} on.");
            _relayA.TurnRelayOn();

            Log.Information($"Turning relay {_relayB.AssetId} on.");
            _relayB.TurnRelayOn();
        }

        public override void OnComplete()
        {
            Log.Information($"Turning relay {_relayA.AssetId} off.");
            _relayA.TurnRelayOff();

            Log.Information($"Turning relay {_relayB.AssetId} off.");
            _relayB.TurnRelayOff();
        }
    }
}
