using System.Threading.Tasks;

namespace SpectraWay.Helpers
{
    public static class SpectraFilteringHelper
    {
        public static async Task<double[]> Filtering(double[] dataToFilter, double[] waveLengthArray, double rho, int m)
        {
            var result = await Task.Run(() =>
            {
                alglib.spline1dinterpolant s;
                int info;
                alglib.spline1dfitreport rep;
                var dataSmoozed = new double[dataToFilter.Length];
                alglib.spline1dfitpenalized(waveLengthArray, dataToFilter, m, rho, out info, out s, out rep);
                for (var i = 0; i < waveLengthArray.Length; i++)
                {
                    var lambda = waveLengthArray[i];
                    var val = alglib.spline1dcalc(s, lambda);
                    dataSmoozed[i] = val;
                }
                return dataSmoozed;
            });
            return result;
        }
    }
}