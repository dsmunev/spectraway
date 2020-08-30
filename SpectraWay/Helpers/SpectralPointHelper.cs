using System.Collections.Generic;
using System.Linq;
using SpectraWay.ParamsRetriever;
using SpectraWay.ViewModel.Experiment;

namespace SpectraWay.Helpers
{
    public class SpectralPointHelper
    {
        public static SpectralPoint[] ToSpectralPoints(ExperimentEntityDataViewModel data, double[] wavelengthArray)
        {
            var spectralPointArray = new SpectralPoint[wavelengthArray.Length];

            for (int i = 0; i < wavelengthArray.Length; i++)
            {
                spectralPointArray[i] = new SpectralPoint
                {
                    WaveLength = wavelengthArray[i],
                    DistancedSignal = new Dictionary<double, double>()
                };
            }

            foreach (var dataItem in data.DataItems.Where(x=>x.Distance>0))
            {
                alglib.spline1dinterpolant s;
                alglib.spline1dbuildlinear(data.WaveLengthArray, dataItem.IntensityArray, out s);
                for (int i = 0; i < wavelengthArray.Length; i++)
                {
                    if (!spectralPointArray[i].DistancedSignal.ContainsKey(dataItem.Distance))
                    {
                        var value = alglib.spline1dcalc(s, spectralPointArray[i].WaveLength);
                        spectralPointArray[i].DistancedSignal.Add(dataItem.Distance, value);
                    }
                }
            }
            

            return spectralPointArray;
        }
    }
}