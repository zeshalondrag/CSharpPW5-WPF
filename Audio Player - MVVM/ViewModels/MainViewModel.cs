using Audio_Player___MVVM.Commands;
using YourMusicPlayer.Utilities;
using Audio_Player___MVVM.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Audio_Player___MVVM.ViewModels
{
    public class MainViewModel : NotificationObject
    {
        public ICommand OpenPlayerCommand { get; private set; }
        public ICommand OpenSettingsCommand { get; private set; }
        public ICommand ExitCommand { get; private set; }

        public MainViewModel()
        {
            OpenPlayerCommand = new RelayCommand(_ => OpenPlayer());
            OpenSettingsCommand = new RelayCommand(_ => OpenSettings());
            ExitCommand = new RelayCommand(_ => ExitApplication());
        }

        private void OpenPlayer() // Открытие проигрывателя
        {
            PlayerWindow playerWindow = new PlayerWindow();
            playerWindow.Show();
            Application.Current.MainWindow.Close();
        }

        private void OpenSettings() // Открытие настроек
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Show();
            Application.Current.MainWindow.Close();
        }

        private void ExitApplication() // Выход из приложения
        {
            Application.Current.Shutdown();
        }
    }
}