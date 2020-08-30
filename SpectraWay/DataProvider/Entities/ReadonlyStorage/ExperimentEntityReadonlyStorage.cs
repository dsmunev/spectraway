using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace SpectraWay.DataProvider.Entities.ReadonlyStorage
{
    public class ExperimentEntityReadonlyStorage:IReadonlyStorage<ExperimentEntity>
    {
        public IEnumerable<ExperimentEntity> Select()
        {
            var experimentsDirectory = "experiments";
            var ext = "json";
            if (!Directory.Exists(experimentsDirectory))
            {
                yield break;
            }
            foreach (var file in Directory.GetFiles(experimentsDirectory, $"*.{ext}"))
            {
                yield return JsonConvert.DeserializeObject<ExperimentEntity>(File.ReadAllText(file));
            }
        }
    }
}