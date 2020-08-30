using System.Collections.Generic;

namespace SpectraWay.DataProvider.Entities.ReadonlyStorage
{
    public class PhysicModelEntityReadonlyStorage: IReadonlyStorage<PhysicModelEntity>
    {
        public IEnumerable<PhysicModelEntity> Select()
        {
            return new List<PhysicModelEntity>
            {
                new PhysicModelEntity() { Name = "Two Layered Skin"},
                new PhysicModelEntity() { Name = "One Layered Skin"},
            };
        }
    }
}