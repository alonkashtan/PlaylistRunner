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
using Movies.Festival.PlaylistRunner.ViewModel;

namespace Movies.Festival.PlaylistRunner.View
{
    /// <summary>
    /// Interaction logic for PlaylistView.xaml
    /// </summary>
    public partial class PlaylistControlView : Window
    {
        public PlaylistControlView(IControlableClipPresenter presenter)
        {
            InitializeComponent();
            controlPanel.Presenter = presenter;
        }
    }
}
