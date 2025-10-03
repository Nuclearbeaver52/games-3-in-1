using System.Windows;
using System.Windows.Input;

namespace MyApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenCrossword_Click(object sender, RoutedEventArgs e)
        {
            var crosswordWindow = new MyApp.CrosswordWindow();
            crosswordWindow.Closed += (s, args) => this.Show();
            crosswordWindow.Show();
            this.Hide();
        }


        private void OpenImageGame_Click(object sender, RoutedEventArgs e)
        {
            var window = new MoleGameWindow( );
            window.Show();
            this.Hide();
        }

        private void OpenSudoku_Click(object sender, RoutedEventArgs e)
        {
            var window = new SudokuWindow();
            window.Show();
            this.Hide();
        }
        private void SecretButton_Click(object sender, RoutedEventArgs e)
        {
            SecretGamesWindow secretGames = new SecretGamesWindow();
            secretGames.Show();
        }
                private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl + S показывает секретную кнопку
            if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
            {
                SecretButton.Visibility = Visibility.Visible;
            }
        }
    }
}
