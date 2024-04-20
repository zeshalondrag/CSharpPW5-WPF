using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using Audio_Player___MVVM.Commands;
using Audio_Player___MVVM.Models;

namespace Audio_Player___MVVM.ViewModels
{
    public class HistoryViewModel : ModelBase
    {
        private ObservableCollection<FileInfo> _listeningHistory;
        private MediaElement _mediaElement;

        public ICommand PlaySelectedSongCommand { get; }

        public ObservableCollection<FileInfo> ListeningHistory
        {
            get { return _listeningHistory; }
            set { _listeningHistory = value; OnPropertyChanged(); }
        }

        public HistoryViewModel(ObservableCollection<FileInfo> history, MediaElement mediaElement)
        {
            ListeningHistory = history;
            _mediaElement = mediaElement;
            PlaySelectedSongCommand = new RelayCommand(PlaySelectedSong);
        }

        private void PlaySelectedSong(object obj)
        {
            if (obj is FileInfo fileInfo)
            {
                string selectedAudioPath = fileInfo.FullName;
                _mediaElement.Source = new Uri(selectedAudioPath);
                _mediaElement.Play();
            }
        }
    }
}