using System.Globalization;
using System.IO;
using SpectraWay.Device.Spectrometer.Lotis;

namespace SpectraWay.Device.Spectrometer.Solar
{
    public class SolarSpectrometer: LotisSpectrometer {
        public new static readonly ISpectrometer Instance = new SolarSpectrometer();
        protected override void ReadCalibration()
        {
            int counter = 0;
            string line;

            // Read the file line by line.  
            var file = new StreamReader("solar_calibration.txt");
            string[] result;
            while ((line = file.ReadLine()) != null)
            {
                double wawelength;
                if ((result = line.Split(' ')).Length > 1 && double.TryParse(result[1], NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out wawelength))
                {
                    WaveLengthArray[counter++] = wawelength;
                }

            }

            file.Close();
        }
    }
}