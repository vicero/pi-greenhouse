using System;
using System.Diagnostics;
using Sensors.Dht;
using Windows.Devices.Gpio;
using System.Threading.Tasks;

namespace PiGreenhouse.Drivers
{
    internal sealed class Dht11Driver : IDisposable
    {
        public Dht11Driver(int pinNumber)
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
                _pin.SetDriveMode(GpioPinDriveMode.Input);
                _dht11 = new Dht11(_pin, GpioPinDriveMode.Input);
            }
        }

        /// <summary>
        /// Deletes an instance of the <see cref="RelayDriver"/> class.
        /// </summary>
        ~Dht11Driver()
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
                    var pin = _pin;
                    if (pin != null)
                    {
                        _pin = null;
                        pin.Dispose();
                    }
                }
                finally
                {
                    _disposed = true;
                }
            }
        }

        /// <summary>
        /// Gets a reading from the DHT11 sensor.
        /// </summary>
        public async Task<DhtReading> Read()
        {
            if (_dht11 == null) {
                return new DhtReading();
            }

            var reading = await _dht11.GetReadingAsync().AsTask();
            return reading;
        }

        private Dht11 _dht11;
        private bool _disposed;
        private GpioPin _pin;
    }
}
