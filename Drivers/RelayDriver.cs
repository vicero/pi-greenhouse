using System;
using Unosquare.RaspberryIO.Abstractions;

namespace PiGreenhouse.Drivers
{
    internal sealed class RelayDriver
    {
        public RelayDriver(IGpioPin pin, string assetId, Action<string, bool?> onStatusChanged)
        {
            _onStatusChanged = onStatusChanged;
            _pin = pin;
            _pin.PinMode = GpioPinDriveMode.Output;
            AssetId = assetId;
            TurnRelayOff();
        }

        /// <summary>
        /// Turns the relay on.
        /// </summary>
        public void TurnRelayOn()
        {
            _pin.Value = false;
            _onStatusChanged(AssetId, true);
        }

        /// <summary>
        /// Turns the relay off.
        /// </summary>
        public void TurnRelayOff()
        {
            _pin.Value = true;
            _onStatusChanged(AssetId, false);
        }

        /// <summary>
        /// Asset ID reported to cloud service.
        /// </summary>
        public string AssetId { get; }

        private readonly Action<string, bool?> _onStatusChanged;
        private readonly IGpioPin _pin;
    }
}
