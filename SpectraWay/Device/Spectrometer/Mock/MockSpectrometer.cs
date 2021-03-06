﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SpectraWay.Device.Spectrometer.Mock
{
    public class MockSpectrometer : ISpectrometer
    {
        public static readonly ISpectrometer Instance = new MockSpectrometer();
        private bool _isSpectrometerReady;
        private double _waveLenthMin;
        private double _waveLenthMax;
        private Random _random = new Random();
        private int _ccdLevels;

        private Timer _timer;

        protected MockSpectrometer()
        {
            _ccdLevels = 2048;
            _isSpectrometerReady = false;
            //_timer = new Timer(o => NotifyDataChanged(GetData()), this, 0, 100);
        }

        public void Start()
        {
            _timer?.Dispose();
            Task.Delay(3000);
            _timer = new Timer(o => NotifyDataChanged(GetData()), this, 0, 100);
            
            _isSpectrometerReady = true;
            NotifyStatusChanged(_isSpectrometerReady);
        }

        public void Stop()
        {
            _timer?.Dispose();
            _timer = null;
        }

        public int GetExposureTime()
        {
            return 100;
        }

        public bool SetExposureTime(int exposureTime)
        {
            return true;
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

        List<SpectrometerDataPoint> ISpectrometer.GetData()
        {
            throw new NotImplementedException();
        }

        public string LastError
        {
            get { return _lastError; }
        }

        double _noiseAmplitude = 0.02;
        public List<SpectrometerDataPoint> GetData()
        {
            var data = new List<SpectrometerDataPoint>();
            for (int i = (int)_waveLenthMin; i < (int)_waveLenthMax + 1; i++)
            {

                data.Add(new SpectrometerDataPoint
                (
                    i,
                    (int)(((_data.ContainsKey(i) ? (_data[i] > _noiseAmplitude ? _data[i] : _noiseAmplitude) : _noiseAmplitude) - _noiseAmplitude * _random.NextDouble()) * CcdLevels)
                ));
            }
            return data.ToList();
        }

        public IList<SpectrometerDataPoint> GetClearData()
        {
            var data = new List<SpectrometerDataPoint>();
            for (int i = (int)_waveLenthMin; i < (int)_waveLenthMax + 1; i++)
            {

                data.Add(new SpectrometerDataPoint
                (
                    i,
                  (int)((_data.ContainsKey(i) ? _data[i] : 0) * CcdLevels)
                ));
            }
            return data.ToArray();
        }



        public IList<SpectrometerDataPoint> GetNoise()
        {
            var noise = new List<SpectrometerDataPoint>();
            for (int i = (int)_waveLenthMin; i < (int)_waveLenthMax + 1; i++)
            {

                noise.Add(new SpectrometerDataPoint
                (
                    i,
                    (int)((_noiseAmplitude - _noiseAmplitude * _random.NextDouble()) * CcdLevels)
                ));
            }
            return noise;
        }



        private Dictionary<double, double> _data = new Dictionary<double, double>()
        {
                {   4.0000000e+02,   8.9276909e-03},
    {   4.0100000e+02,   9.2646535e-03},
    {   4.0200000e+02,   1.0037078e-02},
    {   4.0300000e+02,   1.0622594e-02},
    {   4.0400000e+02,   1.0985720e-02},
    {   4.0500000e+02,   1.1324114e-02},
    {   4.0600000e+02,   1.2275470e-02},
    {   4.0700000e+02,   1.2699118e-02},
    {   4.0800000e+02,   1.4158064e-02},
    {   4.0900000e+02,   1.6027221e-02},
    {   4.1000000e+02,   1.8327979e-02},
    {   4.1100000e+02,   1.9855530e-02},
    {   4.1200000e+02,   2.2952929e-02},
    {   4.1300000e+02,   2.6724774e-02},
    {   4.1400000e+02,   3.0651512e-02},
    {   4.1500000e+02,   3.5859366e-02},
    {   4.1600000e+02,   4.2601322e-02},
    {   4.1700000e+02,   5.0257267e-02},
    {   4.1800000e+02,   5.7972669e-02},
    {   4.1900000e+02,   6.8383853e-02},
    {   4.2000000e+02,   8.1504419e-02},
    {   4.2100000e+02,   9.4871820e-02},
    {   4.2200000e+02,   1.0860683e-01},
    {   4.2300000e+02,   1.3170153e-01},
    {   4.2400000e+02,   1.5008994e-01},
    {   4.2500000e+02,   1.7551486e-01},
    {   4.2600000e+02,   2.0481909e-01},
    {   4.2700000e+02,   2.3245529e-01},
    {   4.2800000e+02,   2.6442730e-01},
    {   4.2900000e+02,   2.9810748e-01},
    {   4.3000000e+02,   3.3860278e-01},
    {   4.3100000e+02,   3.8685632e-01},
    {   4.3200000e+02,   4.3635963e-01},
    {   4.3300000e+02,   4.9298118e-01},
    {   4.3400000e+02,   5.4820124e-01},
    {   4.3500000e+02,   6.1756486e-01},
    {   4.3600000e+02,   6.8429129e-01},
    {   4.3700000e+02,   7.5193581e-01},
    {   4.3800000e+02,   8.1108786e-01},
    {   4.3900000e+02,   8.7089598e-01},
    {   4.4000000e+02,   9.2167603e-01},
    {   4.4100000e+02,   9.5998530e-01},
    {   4.4200000e+02,   9.8462583e-01},
    {   4.4300000e+02,   1.0000000e+00},
    {   4.4400000e+02,   9.8945804e-01},
    {   4.4500000e+02,   9.6412837e-01},
    {   4.4600000e+02,   9.2144281e-01},
    {   4.4700000e+02,   8.7299966e-01},
    {   4.4800000e+02,   8.0788213e-01},
    {   4.4900000e+02,   7.3289553e-01},
    {   4.5000000e+02,   6.5660478e-01},
    {   4.5100000e+02,   5.8197623e-01},
    {   4.5200000e+02,   5.0914153e-01},
    {   4.5300000e+02,   4.4117964e-01},
    {   4.5400000e+02,   3.8691904e-01},
    {   4.5500000e+02,   3.3762041e-01},
    {   4.5600000e+02,   3.0014085e-01},
    {   4.5700000e+02,   2.6725742e-01},
    {   4.5800000e+02,   2.4302822e-01},
    {   4.5900000e+02,   2.2018516e-01},
    {   4.6000000e+02,   1.9975498e-01},
    {   4.6100000e+02,   1.8465120e-01},
    {   4.6200000e+02,   1.7158854e-01},
    {   4.6300000e+02,   1.5942291e-01},
    {   4.6400000e+02,   1.4423153e-01},
    {   4.6500000e+02,   1.3219369e-01},
    {   4.6600000e+02,   1.2397441e-01},
    {   4.6700000e+02,   1.1357366e-01},
    {   4.6800000e+02,   1.0469069e-01},
    {   4.6900000e+02,   9.9048855e-02},
    {   4.7000000e+02,   9.2044870e-02},
    {   4.7100000e+02,   8.8574267e-02},
    {   4.7200000e+02,   8.2190969e-02},
    {   4.7300000e+02,   7.9957176e-02},
    {   4.7400000e+02,   7.6258972e-02},
    {   4.7500000e+02,   7.6016770e-02},
    {   4.7600000e+02,   7.4283139e-02},
    {   4.7700000e+02,   7.3254025e-02},
    {   4.7800000e+02,   7.0911503e-02},
    {   4.7900000e+02,   6.9995185e-02},
    {   4.8000000e+02,   7.0260163e-02},
    {   4.8100000e+02,   6.8548922e-02},
    {   4.8200000e+02,   6.8441951e-02},
    {   4.8300000e+02,   6.8797922e-02},
    {   4.8400000e+02,   6.8669955e-02},
    {   4.8500000e+02,   6.8613735e-02},
    {   4.8600000e+02,   6.8955872e-02},
    {   4.8700000e+02,   6.9329885e-02},
    {   4.8800000e+02,   7.3750398e-02},
    {   4.8900000e+02,   7.3795516e-02},
    {   4.9000000e+02,   7.8964988e-02},
    {   4.9100000e+02,   8.4019857e-02},
    {   4.9200000e+02,   8.6357949e-02},
    {   4.9300000e+02,   9.1637276e-02},
    {   4.9400000e+02,   9.6933129e-02},
    {   4.9500000e+02,   1.0100598e-01},
    {   4.9600000e+02,   1.0763279e-01},
    {   4.9700000e+02,   1.1400859e-01},
    {   4.9800000e+02,   1.2113930e-01},
    {   4.9900000e+02,   1.2788743e-01},
    {   5.0000000e+02,   1.3332788e-01},
    {   5.0100000e+02,   1.4164354e-01},
    {   5.0200000e+02,   1.4541623e-01},
    {   5.0300000e+02,   1.5469944e-01},
    {   5.0400000e+02,   1.5921711e-01},
    {   5.0500000e+02,   1.7132140e-01},
    {   5.0600000e+02,   1.7816661e-01},
    {   5.0700000e+02,   1.8229271e-01},
    {   5.0800000e+02,   1.9509456e-01},
    {   5.0900000e+02,   2.0093178e-01},
    {   5.1000000e+02,   2.0847049e-01},
    {   5.1100000e+02,   2.1581685e-01},
    {   5.1200000e+02,   2.2087542e-01},
    {   5.1300000e+02,   2.2940525e-01},
    {   5.1400000e+02,   2.3691901e-01},
    {   5.1500000e+02,   2.4238710e-01},
    {   5.1600000e+02,   2.4953952e-01},
    {   5.1700000e+02,   2.5535701e-01},
    {   5.1800000e+02,   2.5987081e-01},
    {   5.1900000e+02,   2.6775857e-01},
    {   5.2000000e+02,   2.7006992e-01},
    {   5.2100000e+02,   2.7555144e-01},
    {   5.2200000e+02,   2.8203606e-01},
    {   5.2300000e+02,   2.8544778e-01},
    {   5.2400000e+02,   2.8795422e-01},
    {   5.2500000e+02,   2.9371274e-01},
    {   5.2600000e+02,   2.9640474e-01},
    {   5.2700000e+02,   3.0119635e-01},
    {   5.2800000e+02,   3.0317319e-01},
    {   5.2900000e+02,   3.0701208e-01},
    {   5.3000000e+02,   3.1302731e-01},
    {   5.3100000e+02,   3.1525141e-01},
    {   5.3200000e+02,   3.2031922e-01},
    {   5.3300000e+02,   3.2381639e-01},
    {   5.3400000e+02,   3.2668799e-01},
    {   5.3500000e+02,   3.3277275e-01},
    {   5.3600000e+02,   3.3359707e-01},
    {   5.3700000e+02,   3.3910085e-01},
    {   5.3800000e+02,   3.4139330e-01},
    {   5.3900000e+02,   3.4439189e-01},
    {   5.4000000e+02,   3.4796800e-01},
    {   5.4100000e+02,   3.4945264e-01},
    {   5.4200000e+02,   3.5252554e-01},
    {   5.4300000e+02,   3.5477730e-01},
    {   5.4400000e+02,   3.5659335e-01},
    {   5.4500000e+02,   3.5906228e-01},
    {   5.4600000e+02,   3.6168775e-01},
    {   5.4700000e+02,   3.6146818e-01},
    {   5.4800000e+02,   3.6330746e-01},
    {   5.4900000e+02,   3.6530888e-01},
    {   5.5000000e+02,   3.6924120e-01},
    {   5.5100000e+02,   3.7134353e-01},
    {   5.5200000e+02,   3.7290084e-01},
    {   5.5300000e+02,   3.7729723e-01},
    {   5.5400000e+02,   3.8155316e-01},
    {   5.5500000e+02,   3.7782171e-01},
    {   5.5600000e+02,   3.7857232e-01},
    {   5.5700000e+02,   3.8124015e-01},
    {   5.5800000e+02,   3.8417967e-01},
    {   5.5900000e+02,   3.8553116e-01},
    {   5.6000000e+02,   3.8585608e-01},
    {   5.6100000e+02,   3.9263561e-01},
    {   5.6200000e+02,   3.9107284e-01},
    {   5.6300000e+02,   3.8989655e-01},
    {   5.6400000e+02,   3.9447718e-01},
    {   5.6500000e+02,   3.9199639e-01},
    {   5.6600000e+02,   3.9629395e-01},
    {   5.6700000e+02,   3.9787500e-01},
    {   5.6800000e+02,   3.9778623e-01},
    {   5.6900000e+02,   3.9961620e-01},
    {   5.7000000e+02,   4.0358292e-01},
    {   5.7100000e+02,   3.9861099e-01},
    {   5.7200000e+02,   4.0035923e-01},
    {   5.7300000e+02,   4.0098730e-01},
    {   5.7400000e+02,   4.0372584e-01},
    {   5.7500000e+02,   4.0486965e-01},
    {   5.7600000e+02,   4.0037731e-01},
    {   5.7700000e+02,   4.0814169e-01},
    {   5.7800000e+02,   4.1020054e-01},
    {   5.7900000e+02,   4.0777840e-01},
    {   5.8000000e+02,   4.0914508e-01},
    {   5.8100000e+02,   4.0569739e-01},
    {   5.8200000e+02,   4.1360958e-01},
    {   5.8300000e+02,   4.1125345e-01},
    {   5.8400000e+02,   4.1263321e-01},
    {   5.8500000e+02,   4.1503500e-01},
    {   5.8600000e+02,   4.1987928e-01},
    {   5.8700000e+02,   4.1614808e-01},
    {   5.8800000e+02,   4.1571507e-01},
    {   5.8900000e+02,   4.1490958e-01},
    {   5.9000000e+02,   4.0956097e-01},
    {   5.9100000e+02,   4.1118893e-01},
    {   5.9200000e+02,   4.0975970e-01},
    {   5.9300000e+02,   4.0736453e-01},
    {   5.9400000e+02,   4.0590957e-01},
    {   5.9500000e+02,   4.0258337e-01},
    {   5.9600000e+02,   4.0157259e-01},
    {   5.9700000e+02,   3.9144429e-01},
    {   5.9800000e+02,   3.8375405e-01},
    {   5.9900000e+02,   3.7652604e-01},
    {   6.0000000e+02,   3.6206722e-01},
    {   6.0100000e+02,   3.5162447e-01},
    {   6.0200000e+02,   3.3866659e-01},
    {   6.0300000e+02,   3.3216913e-01},
    {   6.0400000e+02,   3.4374397e-01},
    {   6.0500000e+02,   3.5632930e-01},
    {   6.0600000e+02,   3.5910336e-01},
    {   6.0700000e+02,   3.5777057e-01},
    {   6.0800000e+02,   3.5278131e-01},
    {   6.0900000e+02,   3.4889778e-01},
    {   6.1000000e+02,   3.4534964e-01},
    {   6.1100000e+02,   3.4109291e-01},
    {   6.1200000e+02,   3.4103117e-01},
    {   6.1300000e+02,   3.3564741e-01},
    {   6.1400000e+02,   3.3068926e-01},
    {   6.1500000e+02,   3.2581606e-01},
    {   6.1600000e+02,   3.2295868e-01},
    {   6.1700000e+02,   3.1976669e-01},
    {   6.1800000e+02,   3.1390522e-01},
    {   6.1900000e+02,   3.0901094e-01},
    {   6.2000000e+02,   3.0305828e-01},
    {   6.2100000e+02,   2.9829203e-01},
    {   6.2200000e+02,   2.9154711e-01},
    {   6.2300000e+02,   2.8810098e-01},
    {   6.2400000e+02,   2.8595387e-01},
    {   6.2500000e+02,   2.8056366e-01},
    {   6.2600000e+02,   2.7608491e-01},
    {   6.2700000e+02,   2.7128202e-01},
    {   6.2800000e+02,   2.6421191e-01},
    {   6.2900000e+02,   2.6334589e-01},
    {   6.3000000e+02,   2.5858256e-01},
    {   6.3100000e+02,   2.5302761e-01},
    {   6.3200000e+02,   2.5133664e-01},
    {   6.3300000e+02,   2.4740404e-01},
    {   6.3400000e+02,   2.4457147e-01},
    {   6.3500000e+02,   2.4311580e-01},
    {   6.3600000e+02,   2.3950833e-01},
    {   6.3700000e+02,   2.3819004e-01},
    {   6.3800000e+02,   2.3836119e-01},
    {   6.3900000e+02,   2.4304706e-01},
    {   6.4000000e+02,   2.4270258e-01},
    {   6.4100000e+02,   2.3511234e-01},
    {   6.4200000e+02,   2.2601481e-01},
    {   6.4300000e+02,   2.1700620e-01},
    {   6.4400000e+02,   2.0988289e-01},
    {   6.4500000e+02,   2.0171920e-01},
    {   6.4600000e+02,   2.0075074e-01},
    {   6.4700000e+02,   1.9436890e-01},
    {   6.4800000e+02,   1.8853872e-01},
    {   6.4900000e+02,   1.8785989e-01},
    {   6.5000000e+02,   1.8103034e-01},
    {   6.5100000e+02,   1.8528408e-01},
    {   6.5200000e+02,   1.7683161e-01},
    {   6.5300000e+02,   1.7401236e-01},
    {   6.5400000e+02,   1.7428111e-01},
    {   6.5500000e+02,   1.6589666e-01},
    {   6.5600000e+02,   1.6434783e-01},
    {   6.5700000e+02,   1.6083182e-01},
    {   6.5800000e+02,   1.5833945e-01},
    {   6.5900000e+02,   1.5741564e-01},
    {   6.6000000e+02,   1.5286346e-01},
    {   6.6100000e+02,   1.5168423e-01},
    {   6.6200000e+02,   1.5065206e-01},
    {   6.6300000e+02,   1.4686924e-01},
    {   6.6400000e+02,   1.4462592e-01},
    {   6.6500000e+02,   1.4298172e-01},
    {   6.6600000e+02,   1.3913368e-01},
    {   6.6700000e+02,   1.3473425e-01},
    {   6.6800000e+02,   1.3318804e-01},
    {   6.6900000e+02,   1.3110422e-01},
    {   6.7000000e+02,   1.2808133e-01},
    {   6.7100000e+02,   1.2547053e-01},
    {   6.7200000e+02,   1.2427426e-01},
    {   6.7300000e+02,   1.2024508e-01},
    {   6.7400000e+02,   1.1718190e-01},
    {   6.7500000e+02,   1.1567427e-01},
    {   6.7600000e+02,   1.1233038e-01},
    {   6.7700000e+02,   1.0848032e-01},
    {   6.7800000e+02,   1.0816231e-01},
    {   6.7900000e+02,   1.0603555e-01},
    {   6.8000000e+02,   1.0101794e-01},
    {   6.8100000e+02,   9.8063934e-02},
    {   6.8200000e+02,   9.9747531e-02},
    {   6.8300000e+02,   9.5556345e-02},
    {   6.8400000e+02,   9.5972741e-02},
    {   6.8500000e+02,   9.2521543e-02},
    {   6.8600000e+02,   9.0572482e-02},
    {   6.8700000e+02,   8.3278623e-02},
    {   6.8800000e+02,   7.9915301e-02},
    {   6.8900000e+02,   7.8889546e-02},
    {   6.9000000e+02,   8.0096840e-02},
    {   6.9100000e+02,   8.1855737e-02},
    {   6.9200000e+02,   7.9101725e-02},
    {   6.9300000e+02,   7.7174696e-02},
    {   6.9400000e+02,   7.6834087e-02},
    {   6.9500000e+02,   7.4317703e-02},
    {   6.9600000e+02,   7.3691513e-02},
    {   6.9700000e+02,   7.3612222e-02},
    {   6.9800000e+02,   7.2983247e-02},
    {   6.9900000e+02,   7.0767020e-02},
    {   7.0000000e+02,   6.8961156e-02},
    {   7.0100000e+02,   6.8516836e-02},
    {   7.0200000e+02,   6.5914791e-02},
    {   7.0300000e+02,   6.9407033e-02},
    {   7.0400000e+02,   6.6926341e-02},
    {   7.0500000e+02,   6.6277157e-02},
    {   7.0600000e+02,   6.6010641e-02},
    {   7.0700000e+02,   6.4722011e-02},
    {   7.0800000e+02,   6.3800526e-02},
    {   7.0900000e+02,   6.3955021e-02},
    {   7.1000000e+02,   6.1660888e-02},
    {   7.1100000e+02,   6.3206718e-02},
    {   7.1200000e+02,   6.4054987e-02},
    {   7.1300000e+02,   6.1459774e-02},
    {   7.1400000e+02,   6.1069261e-02},
    {   7.1500000e+02,   5.7986982e-02},
    {   7.1600000e+02,   5.9390243e-02},
    {   7.1700000e+02,   5.4041345e-02},
    {   7.1800000e+02,   4.9878825e-02},
    {   7.1900000e+02,   4.7485267e-02},
    {   7.2000000e+02,   4.6687601e-02},
    {   7.2100000e+02,   4.8402556e-02},
    {   7.2200000e+02,   5.2360200e-02},
    {   7.2300000e+02,   5.1841462e-02},
    {   7.2400000e+02,   4.9533603e-02},
    {   7.2500000e+02,   4.5752716e-02},
    {   7.2600000e+02,   4.6451769e-02},
    {   7.2700000e+02,   4.5696986e-02},
    {   7.2800000e+02,   4.7134095e-02},
    {   7.2900000e+02,   4.4275668e-02},
    {   7.3000000e+02,   4.6995127e-02},
    {   7.3100000e+02,   4.5603379e-02},
    {   7.3200000e+02,   4.7235204e-02},
    {   7.3300000e+02,   4.9236276e-02},
    {   7.3400000e+02,   5.0178434e-02},
    {   7.3500000e+02,   5.0977493e-02},
    {   7.3600000e+02,   4.9463709e-02},
    {   7.3700000e+02,   4.9149850e-02},
    {   7.3800000e+02,   5.0886358e-02},
    {   7.3900000e+02,   4.9201333e-02},
    {   7.4000000e+02,   4.8062970e-02},
    {   7.4100000e+02,   4.8148698e-02},
    {   7.4200000e+02,   4.9798976e-02},
    {   7.4300000e+02,   5.1362895e-02},
    {   7.4400000e+02,   5.0183936e-02},
    {   7.4500000e+02,   5.0010694e-02},
    {   7.4600000e+02,   4.7723061e-02},
    {   7.4700000e+02,   4.8447670e-02},
    {   7.4800000e+02,   4.8068233e-02},
    {   7.4900000e+02,   4.8183855e-02},
    {   7.5000000e+02,   4.5625984e-02},

        };

        private string _lastError;
    }


}