using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using NLog;
using SpectraWay.Localization;

namespace SpectraWay.Device.Stepper.L3Motor
{
    public class NativeL3MotorStepper : IStepper
    {
        bool _isConnected = false;
        String[] ports;
        SerialPort _port;


        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private double _currentDistance;
        private int _currentStep;

        private void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //string data = _port.ReadLine();
            //if ("0".Equals(data))
            //{
            //   
            //}
            //else if ("1".Equals(data))
            //{
            //    
            //}

        }

        public void Start()
        {

            try
            {
                _port?.Dispose();
                string selectedPort = "COM4";
                _port = new SerialPort(selectedPort, 9600, Parity.None, 8, StopBits.One);
                _port.DataReceived += new SerialDataReceivedEventHandler(DataReceived);//Home
                _port.Open();
                _port.Write("#STAR\n");
                _port.DtrEnable = true; //koka
                _port.Write("#LED1ON\n");
                _port.Write("#LED2ON\n");

                GoToBase();
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

        private void GoToBase()
        {
            _port.WriteLine("-1\n");
            _port.WriteLine("1\n");
        }

        private const double UNIT = 0.125;
        private const double MIN_DISTANCE = 1.25;

        public void Stop()
        {
            try
            {
                _port.Write("#LED1OF\n");
                _port.Write("#LED2OF\n");
                _port.Write("#STOP\n");
            }
            catch (Exception e)
            {
                Logger.Error(e);
                //throw;
            }
            _port?.Dispose();
            IsStepperReady = false;
            OnStatusChanged(IsStepperReady);
        }

        public void InnerGoToDistance(double distance)
        {
            if (!IsStepperReady) Start();
            if (!IsStepperReady) return;
            if (distance < MIN_DISTANCE && CurrentDistance == MIN_DISTANCE)
            {
                //CurrentDistance = MIN_DISTANCE;
                return;
            }

            var realDistance = MIN_DISTANCE + Math.Round((distance - MIN_DISTANCE) / UNIT) * UNIT;
            var isBase = false;
            if (distance < MIN_DISTANCE)
            {
                realDistance = MIN_DISTANCE;
                isBase = true;
                Logger.Warn(StringResourceProvider.Instance[StringResourceProvider.Keys.RequestedDistanceLessThanMinDistance]);
                Logger.Info(StringResourceProvider.Instance[StringResourceProvider.Keys.CurrentDistanceSetAsPossibleMinimalDistance_PLACE_Mm].Value, MIN_DISTANCE);
            }

            var previousDistance = CurrentDistance;
            try
            {
                var delta = realDistance - previousDistance;
                var steps = (int)Math.Round(delta / UNIT);
                _port.WriteLine($"{steps}\n");
                _currentStep = steps;
                CurrentDistance = realDistance;
                if (isBase)
                {
                    GoToBase();
                }

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

        public string LastError { get; private set; }
        public bool IsStepperReady { get; private set; }

        public double CurrentDistance
        {
            get { return _currentDistance; }
            private set
            {
                _currentDistance = value;
                OnDistanceChanged();
            }
        }

        public event StepperStatusChangedEventHandler StatusChanged;
        private void OnStatusChanged(bool isStepperReady)
        {
            var e = new StepperStatusChangedEventHandlerArgs(isStepperReady);
            StatusChanged?.Invoke(this, e);
        }

        private void OnDistanceChanged()
        {
            var e = new StepperDistanceChangedEventHandlerArgs();
            DistanceChanged?.Invoke(this, e);
        }

        public event StepperDistanceChangedEventHandler DistanceChanged;
        public event StepperDistanceChangingEventHandler DistanceChanging;
    }
}
