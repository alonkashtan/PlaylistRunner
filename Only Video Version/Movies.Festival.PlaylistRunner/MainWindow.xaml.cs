using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Movies.Festival.PlaylistRunner.Data;
using Movies.Festival.PlaylistRunner.Services;
using Movies.Festival.PlaylistRunner.ViewModel;
using Movies.Festival.PlaylistRunner.View;
using System.Diagnostics;

namespace Movies.Festival.PlaylistRunner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PlaylistViewModel viewModel;

        public MainWindow()
        {
            Playlist playlist = new TextFilePlaylistObtainer().GetPlaylist();

            InitializeComponent();
            
            DataContext = viewModel = new PlaylistViewModel(playlist);
            control = new PlaylistControlView(clipView);
            control.DataContext = this.DataContext;
            control.Show();
            control.Closed += control_Closed;
        }

        PlaylistControlView control;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            control.Owner = this;
        }

        void control_Closed(object sender, EventArgs e)
        {
            Close();
        }

        private void clipView_TotalTimeChanged(TimeSpan time)
        {
            viewModel.TotalLength = time;
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
