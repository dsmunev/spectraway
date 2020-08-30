using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using MahApps.Metro.Controls;
using SpectraWay.Model;

namespace SpectraWay.ViewModel.Dialog
{
    public class SettingsViewModel : BaseViewModel
    {

        public SimpleCommand DialogSaveCommand => new SimpleCommand()
        {
            ExecuteDelegate = o => DialogSaveAction?.Invoke(this)
        };

        public SimpleCommand DialogBrowseCommand => new SimpleCommand()
        {
            ExecuteDelegate = o =>
            {
            }
        };


        public Action<BaseViewModel> DialogSaveAction { get; set; }

        private string _savePath;

        public string SavePath
        {
            get { return _savePath; }
            set
            {
                _savePath = value;
                NotifyPropertyChanged(nameof(SavePath));
            }
        }


        private string _language;

        public string Language
        {
            get { return _language; }
            set
            {
                _language = value;
                NotifyPropertyChanged(nameof(Language));
            }
        }

        public string[] LanguageList => new []{"Ru", "En"};
    }
}