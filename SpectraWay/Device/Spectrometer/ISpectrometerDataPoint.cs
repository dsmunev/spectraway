namespace SpectraWay.Device.Spectrometer
{
    public interface ISpectrometerDataPoint
    {
        double WaveLength { get; }
        double Intencity { get; }
    }
}