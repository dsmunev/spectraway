using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using SpectraWay.Localization;

namespace SpectraWay.Device.Spectrometer.Lotis
{
    public class LotisSpectrometer : ISpectrometer
    {
        public static readonly ISpectrometer Instance = new LotisSpectrometer();
        private volatile bool _isSpectrometerReady;
        private double _waveLenthMin;
        private double _waveLenthMax;
        private Random _random = new Random();
        private int _ccdLevels;
        private int _exposTime;
        private object _syncObj = new object();
        protected int Id;
        public string SerialNumber { get; protected set; }
        //private Timer _timer;

        protected LotisSpectrometer()
        {
            _ccdLevels = 2048;
            _isSpectrometerReady = false;
            //_timer = new Timer(o => NotifyDataChanged(null), this, 0, 30);
        }

        private int _ccd_pixel_count;
        private Thread _loopThread;

        public void Start()
        {
            try
            {
                _loopThread?.Abort();
                SerialNumber = OrminsCcdDriverWrapper.GetSerialNum(Id);
                if (_loopThread != null) Thread.Sleep(100);
                if (!OrminsCcdDriverWrapper.Init(ref Id))
                    throw new ApplicationException(
                        $"{StringResourceProvider.Instance[StringResourceProvider.Keys.LotisSpectrometerCannotBeUsed]} ({StringResourceProvider.Instance[StringResourceProvider.Keys.OrminsCcdCantBeInitialize]})");
                Id = 0;
                OrminsCcdDriverWrapper.CameraReset(Id);
                SerialNumber = OrminsCcdDriverWrapper.GetSerialNum(Id);

                if (!OrminsCcdDriverWrapper.HitTest(Id))
                    throw new ApplicationException(
                        $"{StringResourceProvider.Instance[StringResourceProvider.Keys.LotisSpectrometerCannotBeUsed]} ({StringResourceProvider.Instance[StringResourceProvider.Keys.OrminsCcdHitTestFailed]})");


                var @params = OrminsCcdDriverWrapper.GetParameters(Id);
                //@params = OrminsCcdDriverWrapper.GetParameters();
                if (@params.dwDigitCapacity == 0 && @params.nNumPixels == 0 && @params.nExposureTime == 0)
                    throw new ApplicationException(
                        $"{StringResourceProvider.Instance[StringResourceProvider.Keys.LotisSpectrometerCannotBeUsed]} ({StringResourceProvider.Instance[StringResourceProvider.Keys.OrminsCcdHasIncorrectParams]})");

                _exposTime = @params.nExposureTime;
                _ccdLevels = (int)Math.Pow(2, @params.dwDigitCapacity);
                _ccd_pixel_count = @params.nNumPixels;

                _intencityArray = new int[_ccd_pixel_count];
                WaveLengthArray = new double[_ccd_pixel_count];

                ReadCalibration();

                
                _loopThread = new Thread(DataLoop)
                {
                    IsBackground = true,
                    Priority = ThreadPriority.Lowest
                };
                Thread.Sleep(100);

                _isSpectrometerReady = true;
                _loopThread.Start();

            }
            catch (Exception e)
            {
                LastError = e.Message + "\n" + e.InnerException?.Message + "\n" + e.StackTrace;
                _loopThread?.Abort();
                _loopThread = null;
                _isSpectrometerReady = false;
            }
            NotifyStatusChanged(_isSpectrometerReady);
        }

        public string LastError { get; private set; }

        protected virtual void ReadCalibration()
        {
            int counter = 0;
            string line;

            // Read the file line by line.  
            var file = new StreamReader("lotis_calibration.txt");
            string[] result;
            while ((line = file.ReadLine()) != null)
            {
                if ((result = line.Split('\t')).Length > 1)
                {
                    WaveLengthArray[counter++] = double.Parse(result[1],
                        System.Globalization.NumberStyles.AllowDecimalPoint,
                        System.Globalization.NumberFormatInfo.InvariantInfo);
                }

            }

            file.Close();
        }

        private int[] _intencityArray;
        protected double[] WaveLengthArray;

        private void DataLoop()
        {
            var errcount = 0;
            bool isNotify;
            while (true)
            {

                if (_isSpectrometerReady)
                {
                    try
                    {

                        lock (_syncObj)
                        {
                            if (OrminsCcdDriverWrapper.InitMeasuring(Id))
                            {
                                if (!OrminsCcdDriverWrapper.StartWaitMeasuring(Id)) continue;
                                if (!OrminsCcdDriverWrapper.GetData(_intencityArray, Id)) continue;
                                isNotify = true;
                                //Thread.Sleep(10);
                            }
                            //if (OrminsCcdDriverWrapper.InitMeasuringData(_intencityArray))
                            //{
                            //    //OrminsCcdDriverWrapper.InitMeasuring();
                            //    //OrminsCcdDriverWrapper.StartMeasuring();
                            //    OrminsCcdDriverWrapper.StartWaitMeasuring();
                            //    //OrminsCcdDriverWrapper.GetData(_intencityArray);
                            //    NotifyDataChanged(GetData());
                            //    Thread.Sleep(10);
                            //}
                            else
                            {
                                OrminsCcdDriverWrapper.CameraReset(Id);
                                OrminsCcdDriverWrapper.SetExposition(_exposTime, Id);
                                Thread.Sleep(100);
                                isNotify = false;
                            }
                        }
                        if (isNotify) NotifyDataChanged(GetData());
                        //Thread.Sleep(10);
                    }
                    catch (Exception)
                    {
                        OrminsCcdDriverWrapper.CameraReset(Id);
                        OrminsCcdDriverWrapper.SetExposition(_exposTime, Id);
                        errcount++;
                        if (errcount > 5) throw;
                        Thread.Sleep(100);
                    }

                }
                else
                {
                    return;
                }

            }
        }

        public void Stop()
        {
            _isSpectrometerReady = false;
            _loopThread?.Abort();
            _loopThread = null;
        }

        public int GetExposureTime()
        {
            return _exposTime;
        }


        public bool SetExposureTime(int exposureTime)
        {
            if (_isSpectrometerReady)
            {
                lock (_syncObj)
                {
                    var result = OrminsCcdDriverWrapper.SetExposition(exposureTime, Id);
                    if (result)
                    {
                        _exposTime = exposureTime;
                    }
                    return result;
                }
            }
            return false;
        }

        public event SpectrometerStatusChangedEventHandler StatusChanged;

        protected void NotifyStatusChanged(bool isReady)
        {
            var e = new SpectrometerStatusChangedEventHandlerArgs(isReady);
            OnStatusChanged(e);
        }

        protected virtual void OnStatusChanged(SpectrometerStatusChangedEventHandlerArgs e)
        {
            StatusChanged?.Invoke(this, e);
        }


        public event SpectrometerDataChangedEventHandler DataChanged;

        protected void NotifyDataChanged(List<SpectrometerDataPoint> data)
        {
            var e = new SpectrometerDataChangedEventHandlerArgs(data);
            OnDataChanged(e);
        }

        protected virtual void OnDataChanged(SpectrometerDataChangedEventHandlerArgs e)
        {
            DataChanged?.Invoke(this, e);
        }

        public void SetSpectralRange(double waveLenthMin, double waveLenthMax)
        {
            _waveLenthMin = waveLenthMin;
            _waveLenthMax = waveLenthMax;
        }

        public int CcdLevels
        {
            get { return _ccdLevels; }
        }

        public bool IsSpectrometerReady
        {
            get { return _isSpectrometerReady; }
        }

        double _noiseAmplitude = 0.02;

        public List<SpectrometerDataPoint> GetData()
        {

            var data = new List<SpectrometerDataPoint>();
            if (_intencityArray == null
                || WaveLengthArray == null
                || WaveLengthArray.Length < 1
                || _intencityArray?.Length < 1
                || WaveLengthArray.Length != _intencityArray.Length)
            {
                return data;
            }
            for (int i = 0; i < WaveLengthArray.Length; i++)
            {
                if (WaveLengthArray[i] < _waveLenthMin) continue;
                if (WaveLengthArray[i] > _waveLenthMax) return data;

                data.Add(new SpectrometerDataPoint(WaveLengthArray[i], _intencityArray[i]));
            }

            return data;
        }


    }

}