using System.Collections.Generic;

namespace SpectraWay.DataProvider.Entities.ReadonlyStorage
{
    public class SpectrometerEntityReadonlyStorage:IReadonlyStorage<SpectrometerEntity>
    {
        public IEnumerable<SpectrometerEntity> Select()
        {
            return new List<SpectrometerEntity>
            {
                new SpectrometerEntity() { Name = "LOTIS CMS-400"},
                new SpectrometerEntity() { Name = "LOTIS CMS-400 [static]"},
                new SpectrometerEntity() { Name = "Solar S100"},
                new SpectrometerEntity() { Name = "Mock"},
            };
        }
    }
}