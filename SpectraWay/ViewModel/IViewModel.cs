using System.ComponentModel;

namespace SpectraWay.ViewModel
{
    public interface IViewModel : INotifyPropertyChanged
    {
        void UpdateView();
    }
}
