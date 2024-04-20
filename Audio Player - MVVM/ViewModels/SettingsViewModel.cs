using System.Windows;
using System.Windows.Input;
using Audio_Player___MVVM.Commands;
using Audio_Player___MVVM.Views;

namespace Audio_Player___MVVM.ViewModels
{
    internal class SettingsViewModel
    {
        private readonly Window _currentWindow;

        public ICommand ToggleDarkModeCommand { get; private set; }
        public ICommand GoBackCommand { get; private set; }

        public SettingsViewModel(Window currentWindow)
        {
            ToggleDarkModeCommand = new RelayCommand(ToggleDarkMode);
            _currentWindow = currentWindow;
            GoBackCommand = new RelayCommand(_ => GoBack());
        }

        private void ToggleDarkMode(object parameter) // Тёмный режим
        {
            MessageBox.Show("В разработке!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning); // Доделать
        }

        private void GoBack() // Возвращение на главное окно
        {
            MainWindow mainWindow = new MainWindow();

            Application.Current.MainWindow = mainWindow;
            mainWindow.Show();
            _currentWindow.Close();
        }
    }
}