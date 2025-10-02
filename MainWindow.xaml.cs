using System.Windows;

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
            var crosswordWindow = new SimpleCrossword.MainWindow();
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
    }
}
