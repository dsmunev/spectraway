using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArduinoDriver;
using ArduinoDriver.SerialProtocol;
using ArduinoUploader.Hardware;
using NLog;
using SpectraWay.Localization;

namespace SpectraWay.Device.Stepper.Static
{
    public class StaticStepper : IStepper
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private ArduinoDriver.ArduinoDriver _driver;
        public void Start()
        {
            try
            {
                _driver = new ArduinoDriver.ArduinoDriver(ArduinoModel.UnoR3, "COM3", true);
                IsStepperReady = true;
                CurrentDistance = MIN_DISTANCE;
            }
            catch (Exception e)
            {
                IsStepperReady = false;
                Logger.Error(e);

                //throw;
            }
            OnStatusChanged(IsStepperReady);
        }
        private void OnStatusChanged(bool isStepperReady)
        {
            var e = new StepperStatusChangedEventHandlerArgs(isStepperReady);
            StatusChanged?.Invoke(this, e);
        }
        private const double UNIT = 0.125;
        private const double MIN_DISTANCE = 0;

        public void Stop()
        {
            if (_previousPin > 1)
            {
                _driver.Send(new DigitalWriteRequest(_previousPin, LOW));
            }
        }
        private DigitalValue HIGH = DigitalValue.High;
        private DigitalValue LOW = DigitalValue.Low;
        private Dictionary<double, byte> _distancePinMapper = new Dictionary<double, byte>()
        {
            {1,2},
            {1.25,13},
            {1.5, 4},
            {1.75, 5},
            {2, 6},
        };

        private byte _previousPin = 0;
        private double _currentDistance;

        public void InnerGoToDistance(double distance)
        {
            if (!IsStepperReady) Start();
            if (!IsStepperReady) return;
            if (distance < MIN_DISTANCE)
            {
                //CurrentDistance = MIN_DISTANCE;
                return;
            }

            try
            {
                var currentPin = _distancePinMapper[distance];
                if (_previousPin > 1)
                {
                    _driver.Send(new DigitalWriteRequest(_previousPin, LOW));
                }
                _driver.Send(new DigitalWriteRequest(currentPin, HIGH));
                _previousPin = currentPin;
                CurrentDistance = distance;

            }
            catch (Exception e)
            {
                LastError = e.Message + "\n" + e.InnerException?.Message + "\n" + e.StackTrace;
                Logger.Error(e);
            }



        }

        public void GoToDistance(double distance)
        {
            InnerGoToDistance(distance);
        }

        public async Task GoToDistanceAsync(double distance)
        {
            await Task.Run(() => InnerGoToDistance(distance));
        }

        public string LastError { get; protected set; }
        public bool IsStepperReady { get; protected set; }
        public double CurrentDistance
        {
            get { return _currentDistance; }
            private set
            {
                _currentDistance = value;
                OnDistanceChanged();
            }
        }
        private void OnDistanceChanged()
        {
            var e = new StepperDistanceChangedEventHandlerArgs();
            DistanceChanged?.Invoke(this, e);
        }
        public event StepperStatusChangedEventHandler StatusChanged;
        public event StepperDistanceChangedEventHandler DistanceChanged;
        public event StepperDistanceChangingEventHandler DistanceChanging;
    }
}
