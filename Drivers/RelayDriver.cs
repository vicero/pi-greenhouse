using Raspberry.IO.GeneralPurpose;
using System;
using System.Diagnostics;

namespace PiGreenhouse.Drivers
{
    internal sealed class RelayDriver : IDisposable
    {
        public RelayDriver(ProcessorPin pin, string assetId, Action<string, bool?> onStatusChanged)
        {
            var driver = GpioConnectionSettings.GetBestDriver(GpioConnectionDriverCapabilities.CanSetPinResistor);
            driver.SetPinResistor(pin, PinResistor.None);
            _pin = driver.Out(pin);
            _onStatusChanged = onStatusChanged;
            AssetId = assetId;
        }

        /// <summary>
        /// Deletes an instance of the <see cref="RelayDriver"/> class.
        /// </summary>
        ~RelayDriver()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases resources used by this <see cref="RelayDriver"/> object.
        /// </summary>
        public void Dispose()
        {
            this._pin.Dispose();
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the resources associated with the <see cref="RelayDriver"/> object.
        /// </summary>
        /// <param name="disposing">
        /// <b>true</b> to release both managed and unmanaged resources;
        /// <b>false</b> to release only unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                try
                {
                    if (_pin != null)
                    {
                        _pin.Dispose();
                        _pin = null;
                    }
                }
                finally
                {
                    _disposed = true;
                }
            }
        }

        /// <summary>
        /// Turns the relay on.
        /// </summary>
        public void TurnRelayOn()
        {
            if (_pin == null) { return; }
            _pin.Write(false);
            _onStatusChanged(AssetId, true);
        }

        /// <summary>
        /// Turns the relay off.
        /// </summary>
        public void TurnRelayOff()
        {
            if (_pin != null)
            {
                _pin.Write(true);
                _onStatusChanged(AssetId, false);
            }
        }

        /// <summary>
        /// Asset ID reported to cloud service.
        /// </summary>
        public string AssetId { get; }

        private bool _disposed;
        private Action<string, bool?> _onStatusChanged;
        private GpioOutputBinaryPin _pin;
    }
}
