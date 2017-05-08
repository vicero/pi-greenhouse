using System;
using System.Diagnostics;
using Windows.Devices.Gpio;

namespace PiGreenhouse.Drivers
{
    internal sealed class RelayDriver : IDisposable
    {
        public RelayDriver(int pinNumber)
        {
            GpioPin pin;
            GpioOpenStatus openStatus;
            
            var gpioController = GpioController.GetDefault();
            gpioController.TryOpenPin(pinNumber, GpioSharingMode.Exclusive, out pin, out openStatus);
            if (openStatus != GpioOpenStatus.PinOpened)
            {
                Debug.WriteLine(string.Format("Could not open pin {0}: {1}", pinNumber, openStatus));
            }
            else
            {
                _pin = pin;
                _pin.SetDriveMode(GpioPinDriveMode.Output);
                _pin.Write(GpioPinValue.High);
            }
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
            _pin.Write(GpioPinValue.Low);
        }

        /// <summary>
        /// Turns the relay off.
        /// </summary>
        public void TurnRelayOff()
        {
            if (_pin != null)
            {
                _pin.Write(GpioPinValue.High);
            }
        }

        /// <summary>
        /// Current state of the relay.  [true] if on, [false] if off
        /// </summary>
        public bool? State
        {
            get { return _pin == null ? null : (bool?)(_pin.Read() == GpioPinValue.Low); }
        }

        private bool _disposed;
        private GpioPin _pin;
    }
}
