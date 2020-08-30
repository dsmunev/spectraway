using SpectraWay.Device.Stepper.L3Motor;
using SpectraWay.Device.Stepper.Mock;
using SpectraWay.Device.Stepper.Static;

namespace SpectraWay.Device.Stepper
{
    public class StepperManager:IDeviceManager<IStepper>
    {
        public static readonly IDeviceManager<IStepper> Instance = new StepperManager();

        private StepperManager()
        {
        }

        private readonly L3MotorStepper _l3MotorStepper = new L3MotorStepper();
        private readonly NativeL3MotorStepper _nativeL3MotorStepper = new NativeL3MotorStepper();
        private readonly StaticStepper _saticStepper = new StaticStepper();
        private readonly MockStepper _mockStepper = new MockStepper();
        public IStepper GetDevice(string type)
        {
            switch (type)
            {
                case "Mock":
                    return _mockStepper;
                case "LOTIS CMS-400 [static]":
                    return _saticStepper;
            }
            //return _l3MotorStepper;
            return _nativeL3MotorStepper;
        }
    }

}