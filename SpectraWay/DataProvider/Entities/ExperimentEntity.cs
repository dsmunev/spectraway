using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpectraWay.ParamsRetriever;

namespace SpectraWay.DataProvider.Entities
{
    public class ExperimentEntity: Entity
    {
        public string Category { get; set; }
        public DateTime DateTime { get; set; }
        public ExperimentStatus ExperimentStatus { get; set; }
        public string Spectrometer { get; set; }
        public string PhysicModel { get; set; }
        public double WaveMin { get; set; }
        public double WaveMax { get; set; }
        public IEnumerable<double> DistanceRange { get; set; }
        public double BaseDistance { get; set; }
        public ExperimentEntityData Data { get; set; }
        public IEnumerable<Params> RetrievedParams { get; set; }

}

    public class ExperimentEntityData
    {
        public double[] WaveLengthArray { get; set; }
        public IEnumerable<ExperimentEntityDataItem>  DataItems { get; set; }
    }

    public class ExperimentEntityDataItem
    {
        public bool IsNoise { get; set; }
        public bool IsBase { get; set; }
        public bool IsNormalize { get; set; }
        public bool IsFiltred { get; set; }
        public bool IsAppliedNormalizing { get; set; }
        public bool IsNoiseRemoved { get; set; }
        public bool IsShow { get; set; }
        public double Distance { get; set; }
        public double[] IntensityArray { get; set; }
    }
}
