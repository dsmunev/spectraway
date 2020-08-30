using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SpectraWay.ViewModel.Experiment;

namespace SpectraWay.ParamsRetriever
{
    public abstract class ParamsRetrieverBase
    {
        public string Name { get; protected set; }
        public string DisplayName { get; protected set; }
        public string Description { get; protected set; }

        public abstract double[] GetDistancesArray();
        public abstract double[] GetWavelengthArray();
        public abstract IEnumerable<Params> GetParams(SpectralPoint[] spectalPoints);
        //public abstract IEnumerable<Params> GetParams(ExperimentEntityDataViewModel data);
        protected double[] PrepareInput(SpectralPoint[] spectalPoints)
        {
            var wavelengthArray = GetWavelengthArray();
            var waveArrayLength = wavelengthArray.Length;
            var distancesArray = GetDistancesArray();
            var distanceArrayLength = distancesArray.Length;
            var size = waveArrayLength * (distanceArrayLength - 1);
            var input = new double[size];
            var dictionary = spectalPoints.ToDictionary(x => x.WaveLength, x => x);

            for (int i = 1; i < distanceArrayLength; i++)
            {
                for (int j = 0; j < waveArrayLength; j++)
                {
                    var div = dictionary[wavelengthArray[j]].DistancedSignal[distancesArray[i]] / dictionary[wavelengthArray[j]].DistancedSignal[distancesArray[0]];
                    var log = -Math.Log(div);
                    input[j + (i - 1) * waveArrayLength] = log;

                }
            }
//#if DEBUG
//            var str = string.Join(", ", input);
//            File.AppendAllText("measurements.txt", str); 
//#endif

            return input;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class Params
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Value { get; set; }
        public double Rms { get; set; }
    }

    public class SpectralPoint
    {
        public double WaveLength { get; set; }
        public Dictionary<double, double> DistancedSignal { get; set; }
    }
}