using System.Collections.Generic;

namespace SpectraWay.ParamsRetriever
{
    public class ParamsRetrieverManager
    {
        static readonly SimpleModelParamsRetriever_430_710 SimpleModelParamsRetriever430710 = new SimpleModelParamsRetriever_430_710();
        static readonly SimpleModelParamsRetriever_450_670 SimpleModelParamsRetriever450670 = new SimpleModelParamsRetriever_450_670();
        static readonly MockParamsRetriever MockParamsRetriever = new MockParamsRetriever();
        public static IEnumerable<ParamsRetrieverBase> GetParamsRetrievers()
        {
            
            return new ParamsRetrieverBase[] {
#if DEBUG
                MockParamsRetriever,
#endif
                SimpleModelParamsRetriever450670,
                SimpleModelParamsRetriever430710,
                

            };
        }
    }
}