using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SpectraWay.DataProvider.Entities;
using SpectraWay.DataProvider.Entities.ReadonlyStorage;
using SpectraWay.DataProvider.SaveManagers;
using SpectraWay.Device.Spectrometer;
using SpectraWay.Device.Spectrometer.Lotis;

namespace SpectraWay.DataProvider
{
    public class EntityDataProvider<T> : BaseDataProvider<T> where T : Entity
    {

        private static readonly EntityDataProvider<T> instance = new EntityDataProvider<T>();
        public static EntityDataProvider<T> Instance => instance;

        private EntityDataProvider()
        {
        }
        
        public override IEnumerable<T> Select()
        {
            var readonlyStorage = Activator.CreateInstance(Type.GetType($"SpectraWay.DataProvider.Entities.ReadonlyStorage.{typeof(T).Name}ReadonlyStorage")) as IReadonlyStorage<T>;
            return readonlyStorage?.Select();
        }

        public IEnumerable<TK> Select<TK>(Func<T, TK> func) where TK : class
        {
            return Select().Select(func);
        }

        public IEnumerable<IGrouping<TK, T>> GroupBy<TK>(Func<T, TK> func) where TK : class
        {
            return Select().GroupBy(func);
        }

        private readonly ExperimentSaveManager _experimentSaveManager = new ExperimentSaveManager();
        public ISaveManager<T> SaveManager {
            get
            {
                //todo make fabric method
                if (typeof (T) == typeof (ExperimentEntity))
                {
                    return (ISaveManager<T>)_experimentSaveManager;
                }
                throw new NotImplementedException();
            }
        }
    }
}
