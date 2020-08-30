using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SpectraWay.DataProvider;
using SpectraWay.DataProvider.Entities;
using SpectraWay.Device.Spectrometer;
using SpectraWay.Extension;

namespace SpectraWay.ViewModel.Experiment
{
    public class ExperimentListViewModel : BaseViewModel
    {
        public ExperimentListViewModel()
        {
            ReloadExperimentCollection();
        }

        private async void ReloadExperimentCollection()
        {
            WaitMessage = "Loading ...";
            IsWait = true;
            var collection = await Task.Run(() =>
            {
                return new ObservableCollection<ExperimentViewModel>(
                    EntityDataProvider<ExperimentEntity>.Instance.Select(x => x.ToViewModel()));
            });
            await Task.Delay(3000);

            ExperimentTileCollection = collection;
            IsWait = false;
        }

        private ObservableCollection<ExperimentViewModel> _experimentTileCollection;

        public ObservableCollection<ExperimentViewModel> ExperimentTileCollection
        {
            get { return _experimentTileCollection; }
            set
            {
                _experimentTileCollection = value;
                NotifyPropertyChanged(nameof(ExperimentTileCollection));
            }
        }

        private ExperimentViewModel _selectedExperimentTile;

        public ExperimentViewModel SelectedExperimentTile
        {
            get { return _selectedExperimentTile; }
            set
            {
                var oldTile = _selectedExperimentTile;
                var newTile = value;
                _selectedExperimentTile = value;


                

                oldTile?.StopSpectrometer(false);
                //IsWait = true;
                oldTile?.StopStepper(false);
                //IsWait = true;

                newTile?.RestartSpectrometer();

                newTile?.RestartStepper();
                NotifyPropertyChanged(nameof(SelectedExperimentTile));


            }
        }


    }
}
