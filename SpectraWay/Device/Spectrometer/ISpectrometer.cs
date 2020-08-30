using System.Collections.Generic;

namespace SpectraWay.Device.Spectrometer
{
    public interface ISpectrometer
    {
        void Start();
        void Stop();
        int GetExposureTime();
        bool SetExposureTime(int exposureTime);
        void SetSpectralRange(double waveLenthMin, double waveLenthMax);
        int CcdLevels { get; }
        bool IsSpectrometerReady { get; }
        List<SpectrometerDataPoint> GetData();
        string LastError { get; }

        event SpectrometerStatusChangedEventHandler StatusChanged;
        event SpectrometerDataChangedEventHandler DataChanged;

        //#region TEST
        //IList<SpectrometerDataPoint> GetNoise();

        //#endregion
    }

    public delegate void SpectrometerStatusChangedEventHandler(object sender, SpectrometerStatusChangedEventHandlerArgs args);
    public delegate void SpectrometerDataChangedEventHandler(object sender, SpectrometerDataChangedEventHandlerArgs args);
    public class SpectrometerStatusChangedEventHandlerArgs
    {
        public SpectrometerStatusChangedEventHandlerArgs(bool isSpectrometerReady)
        {
            IsSpectrometerReady = isSpectrometerReady;
        }

        public bool IsSpectrometerReady { get; }
    }
    public class SpectrometerDataChangedEventHandlerArgs
    {
        public SpectrometerDataChangedEventHandlerArgs(List<SpectrometerDataPoint> data)
        {
            Data = data;
        }

        public List<SpectrometerDataPoint> Data { get; }
    }
}