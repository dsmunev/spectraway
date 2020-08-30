using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SpectraWay.Device.Spectrometer;

namespace SpectraWay.Helpers
{
    public class SpectrometerDataHelper
    {
        private double[] _noiseArray;
        private bool _isNullNoiseArray;
        private double[] _normalizingArray;
        private bool _isNullNormalizingArray;

        public bool IsUseNoiseArray { get; set; }
        public bool IsUseNormalizingArray { get; set; }

        public double[] NoiseArray
        {
            get { return _noiseArray; }
            set
            {
                
                _noiseArray = (double[]) value.Clone();
                _isNullNoiseArray = _noiseArray != null;
            }
        }

        public double[] NormalizingArray
        {
            get { return _normalizingArray; }
            set
            {
                double[] tmp = null;
                if (value != null)
                {
                    tmp = (double[]) value.Clone();
                    var min = tmp.Where(x => x != 0).DefaultIfEmpty(0).Min()*0.1;
                    if (min == 0)
                    {
                        min = 0.00001;
                    }
                    var max = tmp.Where(x => x != 0).DefaultIfEmpty(0).Max();
                    if (max == 0)
                    {
                        max = 0.00001;
                    }
                    for (int i = 0; i < tmp.Length; i++)
                    {
                        if (tmp[i] == 0) tmp[i] = min;
                        tmp[i] /= max;
                    }
                    _isNullNormalizingArray = true;
                }
                else
                {
                    _isNullNormalizingArray = false;
                }
                _normalizingArray = tmp;
                //_normalizingArray = value;
                
            }
        }

        public List<SpectrometerDataPoint> Process(List<SpectrometerDataPoint> points)
        {
            //todo length check
            if (!(_isNullNormalizingArray || _isNullNoiseArray)) return points;
            int degreeOfParallelism = 3;

            
            if (_isNullNormalizingArray && _isNullNoiseArray && IsUseNoiseArray && IsUseNormalizingArray)
            {
                Parallel.For(0, (long)degreeOfParallelism, workerId =>
                {
                    var max = points.Count * (workerId + 1) / degreeOfParallelism;
                    for (int i = (int) (points.Count * workerId / degreeOfParallelism); i < max; i++)
                    {
                        var intencity = (points[i].Intencity - _noiseArray[i])/_normalizingArray[i];
                        points[i].Intencity = intencity < 0 ? 0 : intencity ;
                    }
                });
            }
            else if(_isNullNormalizingArray && IsUseNormalizingArray)
            {
                Parallel.For(0, (long)degreeOfParallelism, workerId =>
                {
                    var max = points.Count * (workerId + 1) / degreeOfParallelism;
                    for (int i = (int) (points.Count * workerId / degreeOfParallelism); i < max; i++)
                    {
                        var intencity = points[i].Intencity / _normalizingArray[i];
                        points[i].Intencity = intencity < 0 ? 0 : intencity;
                    }
                });
            }
            else if (_isNullNoiseArray && IsUseNoiseArray)
            {
                Parallel.For(0, (long)degreeOfParallelism, workerId =>
                {
                    var max = points.Count * (workerId + 1) / degreeOfParallelism;
                    for (int i = (int) (points.Count * workerId / degreeOfParallelism); i < max; i++)
                    {
                        var intencity = points[i].Intencity - _noiseArray[i];
                        points[i].Intencity = intencity < 0 ? 0 : intencity;
                    }
                });
            }

            return points;
        }
    }
}