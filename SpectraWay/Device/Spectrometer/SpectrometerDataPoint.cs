namespace SpectraWay.Device.Spectrometer
{
    public class SpectrometerDataPoint
    {
        public SpectrometerDataPoint(double waveLength, double intensity)
        {
            WaveLength = waveLength;
            Intencity = intensity;
        }

        public SpectrometerDataPoint()
        {
        }

        public double WaveLength;

        public double Intencity;

        public override string ToString()
        {
            return $"{{{WaveLength}:{Intencity}}}";
        }
    }
}