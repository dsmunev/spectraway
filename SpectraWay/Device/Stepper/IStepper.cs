using System.Threading.Tasks;

namespace SpectraWay.Device.Stepper
{
    public interface IStepper
    {
        void Start();
        void Stop();
        void GoToDistance(double distance);
        Task GoToDistanceAsync(double distance);
        string LastError { get; }
        bool IsStepperReady { get; }
        double CurrentDistance { get; }

        event StepperStatusChangedEventHandler StatusChanged;
        event StepperDistanceChangedEventHandler DistanceChanged;
        event StepperDistanceChangingEventHandler DistanceChanging;
    }

    public delegate void StepperStatusChangedEventHandler(object sender, StepperStatusChangedEventHandlerArgs args);

    public class StepperStatusChangedEventHandlerArgs
    {

        public StepperStatusChangedEventHandlerArgs(bool isStepperReady)
        {
            IsStepperReady = isStepperReady;
        }

        public bool IsStepperReady { get; }
    }

    public delegate void StepperDistanceChangingEventHandler(object sender, StepperDistanceChangingEventHandlerArgs args);

    public class StepperDistanceChangingEventHandlerArgs
    {
    }

    public delegate void StepperDistanceChangedEventHandler(object sender, StepperDistanceChangedEventHandlerArgs args);

    public class StepperDistanceChangedEventHandlerArgs
    {
    }
}