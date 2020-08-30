using NLog;

namespace SpectraWay.Device.Arduino
{
    public class UnoR3
    {
        protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private ArduinoDriver.ArduinoDriver _driver;
        public UnoR3()
        {

        }
    }
}