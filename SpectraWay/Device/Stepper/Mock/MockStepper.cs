using System.Threading.Tasks;

namespace SpectraWay.Device.Stepper.Mock
{
    public class MockStepper : IStepper
    {
        private string _lastError;
        private bool _isStepperReady;
        private double _currentDistance;

        public void Start()
        {
            IsStepperReady = true;
            OnStatusChanged(IsStepperReady);
        }

        public void Stop()
        {
            IsStepperReady = false;
            OnStatusChanged(IsStepperReady);
        }

        public void GoToDistance(double distance)
        {
            Task.Delay(500);
            CurrentDistance = distance;
        }

        public async Task GoToDistanceAsync(double distance)
        {
            await Task.Run(() => GoToDistance(distance));
        }

        public string LastError
        {
            get { return _lastError; }
        }

        public bool IsStepperReady
        {
            get { return _isStepperReady; }
            set { _isStepperReady = value; }
        }

        public double CurrentDistance
        {
            get { return _currentDistance; }
            private set
            {
                _currentDistance = value;
                OnDistanceChanged();
            }
        }


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

        public event StepperStatusChangedEventHandler StatusChanged;
        public event StepperDistanceChangedEventHandler DistanceChanged;
        public event StepperDistanceChangingEventHandler DistanceChanging;
    }
}