using att.iot.client;

using PiGreenhouse.Drivers;
using PiScheduler;

using System.Diagnostics;

namespace PiGreenhouse
{
    [DebuggerDisplay("{ _name } { NextOccurrence }")]
    internal sealed class DoubleRelayTask : PiTask
    {
        private int _onTimeInMs;
        private string _name;
        private RelayDriver _driverA;
        private RelayDriver _driverB;
        private IDevice _attDevice;
        private string _assetId;

        public DoubleRelayTask(
            string name,
            int pinA,
            int pinB,
            string assetId,
            int recurrence,
            int onTimeInMs)
            : base(recurrence, name, "Relay", onTimeInMs)
        {
            _name = name;
            _driverA= new RelayDriver(pinA);
            _driverB = new RelayDriver(pinB);
            _onTimeInMs = onTimeInMs;
            _attDevice = new Device("vicero_hZzesPX4", "2BgYhziU");
            _attDevice.DeviceId = "fHDlCmUC00wifPXl7SiauaT6";
            _assetId = assetId;
        }

        protected override void DoWork()
        {
            Debug.WriteLine("Turning relay A" + Name + " on.");
            _driverA.TurnRelayOn();
            Debug.WriteLine("Relay A " + Name + " is " + _driverA.State);

            Debug.WriteLine("Turning relay B " + Name + " on.");
            _driverB.TurnRelayOn();
            Debug.WriteLine("Relay B " + Name + " is " + _driverB.State);

            _attDevice.Send(this._assetId, true.ToString());
        }

        public override void OnComplete()
        {
            Debug.WriteLine("Turning relay A " + Name + " off.");
            _driverA.TurnRelayOff();
            Debug.WriteLine("Relay A " + Name + " is " + _driverA.State);
            Debug.WriteLine("Turning relay B " + Name + " off.");
            _driverB.TurnRelayOff();
            Debug.WriteLine("Relay B " + Name + " is " + _driverB.State);

            _attDevice.Send(this._assetId, false.ToString());
        }
    }
}
