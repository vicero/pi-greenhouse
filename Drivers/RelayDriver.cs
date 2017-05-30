using Raspberry.IO.GeneralPurpose;
using System;
using System.Diagnostics;

namespace PiGreenhouse.Drivers
{
    internal sealed class RelayDriver : IDisposable
    {
        public RelayDriver(ConnectorPin pin)
        {
            var driver = GpioConnectionSettings.GetBestDriver(GpioConnectionDriverCapabilities.None);
            _pin = driver.InOut(pin);
            _pin.Write(true);
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
        }

        /// <summary>
        /// Turns the relay off.
        /// </summary>
        public void TurnRelayOff()
        {
            if (_pin != null)
            {
                _pin.Write(true);
            }
        }

        /// <summary>
        /// Current state of the relay.  [true] if on, [false] if off
        /// </summary>
        public bool? State
        {
            get { return _pin == null ? null : (bool?)(_pin.Read() == false); }
        }

        private bool _disposed;
        private GpioInputOutputBinaryPin _pin;
    }
}
