using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using SpectraWay.Device.Stepper;
using SpectraWay.Helpers;
using SpectraWay.ViewModel;
using SpectraWay.ViewModel.Experiment;

namespace SpectraWay.Controls
{
    /// <summary>
    /// Interaction logic for ApplyFilterWindow.xaml
    /// </summary>
    public partial class ApplyFilterWindow
    {
        private ExperimentEntityDataViewModel _filteredData;
        public ExperimentEntityDataViewModel OriginalData { get; set; }

        public ExperimentEntityDataViewModel FilteredData
        {
            get { return _filteredData; }
            set
            {
                _filteredData = value;
                var bindingExpression = BindingOperations.GetBindingExpression(CustomMultilineGraph, CustomMultilineGraph.DataProperty);
                bindingExpression?.UpdateTarget();
            }
        }

        public double Rho { get; set; }

        public int M { get; set; }
        public ApplyFilterWindow()
        {
            InitializeComponent();
            Rho = 0.1;
            M = 100;
            RhoTextbox.Value = Rho;
            MTextbox.Value = M;
            DataContext = this;
        }

        public async void ApplyFilter()
        {
            Waitor.Visibility = Visibility.Visible;
            //TODO check ALL
            try
            {
                var waveLengthArray = FilteredData.WaveLengthArray;
                var dataToFilter = FilteredData.DataItems.First(x => !x.IsFiltred).IntensityArray;

                var result = await SpectraFilteringHelper.Filtering(dataToFilter, waveLengthArray, Rho, M);

                FilteredData.DataItems.First(x => x.IsFiltred).IntensityArray = result;
            }
            finally
            {
                Waitor.Visibility = Visibility.Collapsed;
            }


        }



        private void MTextbox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) ApplyFilter();
        }

        private void MTextbox_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            M = (int)(e.NewValue ?? 30);
        }

        private void RhoTextbox_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            Rho = e.NewValue ?? -1;
        }

        private void RhoTextbox_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) ApplyFilter();
        }

        private void ButtonApply_OnClick(object sender, RoutedEventArgs e)
        {
            ApplyFilter();
        }

        private void ButtonSave_OnClick(object sender, RoutedEventArgs e)
        {
            //TODO check ALL
            var distance = FilteredData.DataItems.First().Distance;
            OriginalData.DataItems.First(x => x.Distance == distance).IntensityArray =
                FilteredData.DataItems.First(x => x.IsFiltred).IntensityArray;
        }

        private void ButtonExit_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
