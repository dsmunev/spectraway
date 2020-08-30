using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using MahApps.Metro.Controls;
using SpectraWay.DataProvider;
using SpectraWay.DataProvider.Entities;
using SpectraWay.Extension;
using SpectraWay.Model;
using SpectraWay.ParamsRetriever;
using SpectraWay.ViewModel.Experiment;

namespace SpectraWay.ViewModel.Dialog
{
    public class ExperimentDialogViewModel: ExperimentViewModel
    {

        public ExperimentDialogViewModel()
        {
        }

        public ExperimentDialogViewModel(ExperimentViewModel vm)
        {
            this.InitFieldsFrom(vm);
        }

        public SimpleCommand DialogAddCommand => new SimpleCommand()
        {
            ExecuteDelegate = o => DialogAddAction?.Invoke(this)
        };

        public SimpleCommand UpdateNumericUpDownBindingOnEnterCommand => new SimpleCommand()
        {
            ExecuteDelegate = o =>
            {
                NumericUpDown numericUpDown = o as NumericUpDown;
                if (numericUpDown != null)
                {
                    DependencyProperty prop = NumericUpDown.ValueProperty;
                    BindingExpression binding = BindingOperations.GetBindingExpression(numericUpDown, prop);
                    binding?.UpdateSource();
                    
                    if (Keyboard.PrimaryDevice != null)
                    {
                        if (Keyboard.PrimaryDevice.ActiveSource != null)
                        {
                            KeyEventArgs args = new KeyEventArgs(Keyboard.PrimaryDevice,
                                Keyboard.PrimaryDevice.ActiveSource, 0, Key.Tab) {RoutedEvent = Keyboard.KeyDownEvent};
                            InputManager.Current.ProcessInput(args);
                        }
                    }
                    //Keyboard.ClearFocus();
                }
            }
        };
        

        public Action<BaseViewModel> DialogAddAction { get; set; }

        private double _distanceToAdd;

        public ICommand AddCurrentDistanceToRangeCommand => new SimpleCommand
        {
            ExecuteDelegate = o =>
            {
                if(DistanceRange == null) DistanceRange = new ObservableCollection<double>();
                DistanceRange.Add(DistanceToAdd);
            }
        };

        public double DistanceToAdd
        {
            get { return _distanceToAdd; }
            set
            {
                _distanceToAdd = value;
                NotifyPropertyChanged(nameof(DistanceToAdd));
            }
        }


        public ObservableCollection<string> CategoryList
        {
            get { return new ObservableCollection<string>(EntityDataProvider<ExperimentEntity>.Instance.GroupBy(x => x.Category).Select(x => x.Key)); }
        }

        public ObservableCollection<ParamsRetrieverBase> PhysicModelList
        {
            get { return new ObservableCollection<ParamsRetrieverBase>(ParamsRetrieverManager.GetParamsRetrievers()); }
        }

        public ObservableCollection<string> SpectrometerList
        {
            get { return new ObservableCollection<string>(EntityDataProvider<SpectrometerEntity>.Instance.Select(x => x.Name)); }
        }
    }
}
