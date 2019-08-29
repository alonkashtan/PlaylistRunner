using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using ak.oss.PlaylistRunner.Data;
using ak.oss.PlaylistRunner.Services;
using ak.oss.PlaylistRunner.ViewModel;
using ak.oss.PlaylistRunner.View;
using ak.oss.PlaylistRunner.View.ClipPresenters;
using Microsoft.Win32;
using System.IO;

namespace ak.oss.PlaylistRunner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PlaylistViewModel viewModel;

        public MainWindow()
        {
            Dictionary<string, IPlaylistObtainer> extensionToObtainer = new Dictionary<string, IPlaylistObtainer>
            {
                { ".csv", new TextFilePlaylistObtainer() },
                { ".xlsx", new ExcelPlaylistObtainer() }
            };
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "*.xlsx;*.csv|*.xlsx;*.csv", Multiselect = false };
            openFileDialog.ShowDialog();
            if (string.IsNullOrEmpty(openFileDialog.FileName)) App.Current.Shutdown();

            Playlist playlist = extensionToObtainer[Path.GetExtension(openFileDialog.FileName)].GetPlaylist(new ExtensionToTypeConverter(), openFileDialog.FileName);
            if (playlist == null) { Application.Current.Shutdown(); return; }

            InitializeComponent();

            DataContext = viewModel = new PlaylistViewModel(playlist, new ClipPresenterFactory());
            _control = new PlaylistControlView(viewModel, viewModel);
            _control.DataContext = this.DataContext;
            _control.Show();
            _control.Closed += control_Closed;
        }

        PlaylistControlView _control;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _control.Owner = this;
        }

        void control_Closed(object sender, EventArgs e)
        {
            Close();
        }

        private void clipView_TotalTimeChanged(TimeSpan time)
        {
            viewModel.TotalTime = time;
        }

        private void clipView_PositionChanged(TimeSpan position)
        {
            viewModel.Position = position;
        }

        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void maxButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void dragButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        
    }
}
