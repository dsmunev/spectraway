using System.Collections.Generic;

namespace SpectraWay.DataProvider.Entities.ReadonlyStorage
{
    public interface IReadonlyStorage<out T> where T : Entity
    {
        IEnumerable<T> Select();
    }
}