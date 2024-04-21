using MaterialDesignThemes.Wpf;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Audio_Player___MVVM.Commands;
using Audio_Player___MVVM.Models;
using Audio_Player___MVVM.Views;
using System.Windows.Controls;
using System.Windows.Media;

namespace Audio_Player___MVVM.ViewModels
{
    public class PlayerViewModel : ModelBase
    {
        private readonly Window _currentWindow;
        private readonly MediaElement _mediaElement;
        private ObservableCollection<FileInfo> _files = new ObservableCollection<FileInfo>();
        private ObservableCollection<FileInfo> _listeningHistory = new ObservableCollection<FileInfo>();
        private int _currentSongIndex = 0;
        private bool _isPlaying = false;
        private bool _isRepeating = false;
        private bool _isShuffled = false;

        public ICommand PlayCommand { get; }
        public ICommand NextSongCommand { get; }
        public ICommand PreviousSongCommand { get; }
        public ICommand ShuffleCommand { get; }
        public ICommand ReplayCommand { get; }
        public ICommand FolderMusicCommand { get; }
        public ICommand HistoryMusicCommand { get; }
        public ICommand ExitMusicCommand { get; }
        public ICommand SelectSongCommand { get; }

        public ObservableCollection<FileInfo> Files
        {
            get { return _files; }
            set { _files = value; OnPropertyChanged(); }
        }

        public ObservableCollection<FileInfo> ListeningHistory
        {
            get { return _listeningHistory; }
            set
            {
                _listeningHistory = value;
                OnPropertyChanged(nameof(ListeningHistory)); // Уведомляем интерфейс о изменениях
            }
        }

        private TimeSpan _currentTime;
        public TimeSpan CurrentTime
        {
            get { return _currentTime; }
            set { _currentTime = value; OnPropertyChanged(); }
        }

        private TimeSpan _totalDuration;
        public TimeSpan TotalDuration
        {
            get { return _totalDuration; }
            set { _totalDuration = value; OnPropertyChanged(); }
        }

        public bool IsPlaying
        {
            get { return _isPlaying; }
            set { _isPlaying = value; OnPropertyChanged(); }
        }

        private PackIconKind _playButtonContent = PackIconKind.Play;
        public PackIconKind PlayButtonContent
        {
            get { return _playButtonContent; }
            set { _playButtonContent = value; OnPropertyChanged(); }
        }

        private PackIconKind _replayButtonContent = PackIconKind.Replay;
        public PackIconKind ReplayButtonContent
        {
            get { return _replayButtonContent; }
            set { _replayButtonContent = value; OnPropertyChanged(); }
        }

        private SolidColorBrush _replayButtonForeground = Brushes.Teal;
        public SolidColorBrush ReplayButtonForeground
        {
            get { return _replayButtonForeground; }
            set { _replayButtonForeground = value; OnPropertyChanged(); }
        }

        private double _volume;
        public double Volume
        {
            get { return _volume; }
            set
            {
                _volume = value;
                OnPropertyChanged(nameof(Volume));
                UpdateMediaVolume();
            }
        }

        private ICommand _playSelectedSongCommand;
        public ICommand PlaySelectedSongCommand
        {
            get
            {
                if (_playSelectedSongCommand == null)
                {
                    _playSelectedSongCommand = new RelayCommand(PlaySelectedSong);
                }
                return _playSelectedSongCommand;
            }
        }

        public PlayerViewModel(Window currentWindow, MediaElement mediaElement)
        {
            PlayButtonContent = PackIconKind.Play;
            ReplayButtonContent = PackIconKind.Replay;
            ReplayButtonForeground = Brushes.Teal;
            _currentWindow = currentWindow;
            _mediaElement = mediaElement;
            PlayCommand = new RelayCommand(Play);
            NextSongCommand = new RelayCommand(NextSong);
            PreviousSongCommand = new RelayCommand(PreviousSong);
            ShuffleCommand = new RelayCommand(Shuffle);
            ReplayCommand = new RelayCommand(Replay);
            FolderMusicCommand = new RelayCommand(FolderMusic);
            HistoryMusicCommand = new RelayCommand(HistoryMusic);
            ExitMusicCommand = new RelayCommand(ExitMusic);
            SelectSongCommand = new RelayCommand(SelectSong);
        }

        private void SelectSong(object selectedSong)
        {
            if (selectedSong != null && selectedSong is FileInfo fileInfo)
            {
                _currentSongIndex = Files.IndexOf(fileInfo);
                PlaySelectedSong();
            }
        }


        private void PlaySelectedSong(object selectedSong)
        {
            if (selectedSong != null && selectedSong is FileInfo fileInfo)
            {
                string selectedAudioPath = fileInfo.FullName;
                _mediaElement.Source = new Uri(selectedAudioPath);
                _mediaElement.Play();
                IsPlaying = true;
            }
        }

        private void UpdateMediaVolume() // Регулирование громкости
        {
            if (_mediaElement != null)
            {
                _mediaElement.Volume = _volume / 100;
            }
        }

        private void Play(object obj) // Воспроизведение/Пауза музыки
        {
            if (IsPlaying)
            {
                _mediaElement.Pause();
                PlayButtonContent = PackIconKind.Pause;
            }
            else
            {
                _mediaElement.Play();
                PlayButtonContent = PackIconKind.Play;
            }

            IsPlaying = !IsPlaying;
        }

        private void NextSong(object obj) // Следующая песня
        {
            if (_currentSongIndex < _files.Count - 1)
            {
                _currentSongIndex++;
            }
            else
            {
                _currentSongIndex = 0;
            }

            PlaySelectedSong();
        }

        private void PreviousSong(object obj) // Предыдущая песня
        {
            if (_currentSongIndex > 0)
            {
                _currentSongIndex--;
            }
            else
            {
                _currentSongIndex = _files.Count - 1;
            }

            PlaySelectedSong();
        }

        private void Shuffle(object obj) // Включить перемешку музыки
        {
            if (!_isShuffled)
            {
                Random rng = new Random();
                _files = new ObservableCollection<FileInfo>(_files.OrderBy(x => rng.Next()));
                _isShuffled = true;
            }
            else
            {
                _files = new ObservableCollection<FileInfo>(_files.OrderBy(file => file.Name));
                _isShuffled = false;
            }

            _currentSongIndex = 0;
            PlaySelectedSong();
        }

        private void Replay(object obj) // Поставить повтор музыки
        {
            _isRepeating = !_isRepeating;

            if (_isRepeating)
            {
                ReplayButtonContent = PackIconKind.Replay;
                ReplayButtonForeground = Brushes.Red;

                _mediaElement.MediaEnded += MediaElement_MediaEnded;
            }
            else
            {
                ReplayButtonContent = PackIconKind.Replay;
                ReplayButtonForeground = Brushes.Teal;

                _mediaElement.MediaEnded -= MediaElement_MediaEnded;
            }
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (_isRepeating)
            {
                _mediaElement.Position = TimeSpan.Zero;
                _mediaElement.Play();
            }
        }

        private void FolderMusic(object obj) // Выбор папки с музыкой
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog { IsFolderPicker = true };
            var result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                string folderPath = dialog.FileName;
                var audioFiles = Directory.GetFiles(folderPath)
                                          .Select(item => new FileInfo(item))
                                          .Where(file => IsAudioFile(file.Extension.ToLower()))
                                          .ToList();

                Files = new ObservableCollection<FileInfo>(audioFiles);

                if (Files.Count > 0)
                {
                    _currentSongIndex = 0;
                    PlaySelectedSong();
                }
            }
        }

        private void HistoryMusic(object obj) // Открытие окна истории прослушивания
        {
            HistoryWindow history = new HistoryWindow(ListeningHistory, _mediaElement);
            history.ShowDialog();
        }


        private void ExitMusic(object obj) // Назад на главное окно
        {
            MainWindow mainWindow = new MainWindow();
            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();
            _currentWindow.Close();
        }

        private void PlaySelectedSong()
        {
            try
            {
                string selectedAudioPath = Files[_currentSongIndex].FullName;

                // Добавляем воспроизведенную песню в историю прослушивания
                var selectedSong = Files[_currentSongIndex];
                if (!ListeningHistory.Contains(selectedSong))
                {
                    ListeningHistory.Add(selectedSong);
                }

                _mediaElement.Source = new Uri(selectedAudioPath);

                if (!IsPlaying)
                {
                    _mediaElement.Play();
                    IsPlaying = true;
                }
            }
            catch
            {
                MessageBox.Show("Выберите папку с музикой для того чтобы пользоваться опциями!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateTimerCallback(object state)
        {
            // Обновить таймер
            // Обновить UI
        }

        private bool IsAudioFile(string extension)
        {
            return extension == ".mp3" || extension == ".m4a" || extension == ".wav";
        }
    }
}
