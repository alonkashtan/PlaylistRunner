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
using ak.oss.PlaylistRunner.Data;
using ak.oss.PlaylistRunner.ViewModel;

namespace ak.oss.PlaylistRunner.View
{
    /// <summary>
    /// Interaction logic for PlaylistView.xaml
    /// </summary>
    public partial class PlaylistControlView : Window
    {
        private IClipProvider _provider;

        public PlaylistControlView(IControlableClipPresenter presenter, IClipProvider provider)
        {
            InitializeComponent();
            _provider = provider;
            controlPanel.Presenter = presenter;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                _listBox.ScrollIntoView(e.AddedItems[0]);
        }
    }
}
