using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpectraWay.DataProvider.Entities;

namespace SpectraWay.DataProvider
{
    public abstract class BaseDataProvider<T> where T : Entity
    {
        public abstract IEnumerable<T> Select();
    }
}
