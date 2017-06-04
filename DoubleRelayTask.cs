using att.iot.client;

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
        private IDevice _attDevice;
        private string _assetId;

        public DoubleRelayTask(
            string name,
            RelayDriver relayA,
            ConnectorPin pinB,
            string assetId,
            int recurrence,
            int onTimeInMs)
            : base(recurrence, name, "Relay", onTimeInMs)
        {
            _name = name;
            _relayA = relayA;
            _relayB = new RelayDriver(pinB);
            _onTimeInMs = onTimeInMs;
            _attDevice = new Device("vicero_hZzesPX4", "2BgYhziU");
            _attDevice.DeviceId = "fHDlCmUC00wifPXl7SiauaT6";
            _assetId = assetId;
        }

        protected override void DoWork()
        {
            Console.WriteLine("Turning relay " + Name + ".A on.");
            _relayA.TurnRelayOn();
            Console.WriteLine("Relay A " + Name + " is " + _relayA.State);

            Console.WriteLine("Turning relay " + Name + ".B on.");
            _relayB.TurnRelayOn();
            Console.WriteLine("Relay B " + Name + " is " + _relayB.State);

            _attDevice.Send(this._assetId, true.ToString());
        }

        public override void OnComplete()
        {
            Console.WriteLine("Turning relay " + Name + ".A off.");
            _relayA.TurnRelayOff();
            Console.WriteLine("Relay A " + Name + " is " + _relayA.State);

            Console.WriteLine("Turning relay " + Name + ".B off.");
            _relayB.TurnRelayOff();
            Console.WriteLine("Relay B " + Name + " is " + _relayB.State);

            _attDevice.Send(this._assetId, false.ToString());
        }
    }
}
