using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;
using NLog;
using SpectraWay.Extension;
using SpectraWay.ViewModel;

namespace SpectraWay.Controls
{
    /// <summary>
    /// Interaction logic for LogViewer.xaml
    /// </summary>
    public partial class LogViewer : MetroWindow
    {
        private string CurrentLogString => ((App) Application.Current).LogStringBuilder.ToString();

        public LogViewer()
        {
            InitializeComponent();
            SpectraWayNLogTarget.NewMessageRecieved += TargetOnNewMessageRecieved;
            Closed += OnClosed;
            Loaded += OnLoaded; 
            
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            SetText();
            Loaded -= OnLoaded;
        }

        private void SetText()
        {
            BaseViewModel.CallThreadSafe(() =>
            {
                TextBox.Text = CurrentLogString;
                ScrollViewer.ScrollToBottom();
            });
        }

        private void OnClosed(object sender, EventArgs eventArgs)
        {
            SpectraWayNLogTarget.NewMessageRecieved -= TargetOnNewMessageRecieved;
            Closed -= OnClosed;
        }

        private void TargetOnNewMessageRecieved(object sender, NewMessageRecievedEventHandlerArgs args)
        {
            SetText();
        }

        private void buttonClipboard_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(CurrentLogString);
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = $"spectraway_log_{DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss")}.txt";
            saveFileDialog.DefaultExt = "txt";
            if (saveFileDialog.ShowDialog() == true)
                File.WriteAllText(saveFileDialog.FileName, CurrentLogString);
        }

        private void buttonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
