using System.ComponentModel;
using SpectraWay.DataProvider.Entities;

namespace SpectraWay.DataProvider.SaveManagers
{
    public interface ISaveManager<in T> where T:Entity
    {
        event ReadyToSaveChangedEventHandler ReadyToSaveChanged;
        void Map(INotifyPropertyChanged viewModel, T entity = null);
        bool SaveAll();
        bool Save(INotifyPropertyChanged viewModel);
        bool IsReadyToSave { get; }

    }

    public delegate void ReadyToSaveChangedEventHandler(object sender, ReadyToSaveChangedEventHandlerArgs args);

    public class ReadyToSaveChangedEventHandlerArgs
    {
        public ReadyToSaveChangedEventHandlerArgs(bool isReady)
        {
            IsReadyToSave = isReady;
        }

        public bool IsReadyToSave { get; private set; }
    }
}