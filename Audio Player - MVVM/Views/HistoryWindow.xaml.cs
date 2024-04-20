using Audio_Player___MVVM.ViewModels;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Audio_Player___MVVM.Views
{
    public partial class HistoryWindow : Window
    {
        public HistoryWindow(ObservableCollection<FileInfo> history, MediaElement mediaElement)
        {
            InitializeComponent();
            var viewModel = new HistoryViewModel(history, mediaElement);
            DataContext = viewModel;
        }
    }
}