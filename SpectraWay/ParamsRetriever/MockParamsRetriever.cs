using System;
using System.Collections.Generic;

namespace SpectraWay.ParamsRetriever
{
    public class MockParamsRetriever : ParamsRetrieverBase
    {
        private readonly double[] _distanceArray;
        private readonly double[] _lambdaArray;

        public MockParamsRetriever()
        {
            DisplayName = "MockParamsRetriever";
            Name = "MockParamsRetriever";
            Description = "1.00 : 0.25 : 2.00 mm";
            _distanceArray = new double[5];
            for (var i = 0; i < 5; i++)
            {
                _distanceArray[i] = 1.00 + 0.25 * i;
            }
            _lambdaArray = new double[32];
            for (var i = 0; i < 32; i++)
            {
                _lambdaArray[i] = 440.0 + 5.0 * i;
            }

        }

        public override double[] GetDistancesArray()
        {

            return _distanceArray;
        }

        public override double[] GetWavelengthArray()
        {

            return _lambdaArray;
        }


        public override IEnumerable<Params> GetParams(SpectralPoint[] spectalPoints)
        {
            var @params = new List<Params>();
            var rand = new Random();
            //0.2     7    1;% F_BLOOD = 6;
            //80      210  1;% C_HB = 7;
            var hb = 0.2*0.01*80 + rand.NextDouble()*(7*0.01*210 - 0.2*0.01*80);
            var param = new Params
            {
                Value = hb,
                Rms = 7 * 0.01 * hb,
                Name = "Hb",
                Description = "G/liter"
            };
            @params.Add(param);

            var s = 40 + rand.NextDouble() * (99 - 40);
            param = new Params
            {
                Value = s,
                Rms = 3 * 0.01 * s,
                Name = "S",
                Description = "%"
            };
            @params.Add(param);
            //0.1     40
            var bilirubin = 0.1 + rand.NextDouble() * (40 - 0.1);
            param = new Params
            {
                Value = bilirubin,
                Rms = 10.5 * 0.01 * bilirubin,
                Name = "Bilirubin",
                Description = "mG/liter"
            };
            @params.Add(param);

            return @params;
        }
    }
}