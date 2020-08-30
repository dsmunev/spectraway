using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpectraWay.Device.Spectrometer.Lotis;
using SpectraWay.Device.Spectrometer.Mock;
using SpectraWay.Device.Spectrometer.Solar;

namespace SpectraWay.Device.Spectrometer
{
    public class SpectrometerManager: IDeviceManager<ISpectrometer>
    {
        public static readonly IDeviceManager<ISpectrometer> Instance = new SpectrometerManager();

        private SpectrometerManager()
        {
        }

        public ISpectrometer GetDevice(string type)
        {
            switch (type)
            {
                case "LOTIS CMS-400": return LotisSpectrometer.Instance;
                //case "LOTIS CMS-400 [static]": return LotisSpectrometer.Instance;
                case "LOTIS CMS-400 [native stepper]": return LotisSpectrometer.Instance;
                case "Solar S100": return SolarSpectrometer.Instance;
#if DEBUG
                case "Mock": return MockSpectrometer.Instance;
#endif
                default: throw new ArgumentException("Invalid type", nameof(type));
            }
        }
    }
}
