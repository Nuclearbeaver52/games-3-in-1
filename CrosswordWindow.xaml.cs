using System.Windows;
using System.Windows.Controls;

namespace SimpleCrossword
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            int totalCells = 26; // Всего клеток во всех словах
            int filledCells = 0;

            // Считаем заполненные клетки
            foreach (var child in FindVisualChildren<TextBox>(this))
            {
                if (!string.IsNullOrEmpty(child.Text))
                    filledCells++;
            }

            double progress = (double)filledCells / totalCells * 100;
            
        }

        private void Cell_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProgress();
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            int correctCells = 0;
            int totalCells = 0;

            // Проверяем каждую клетку
            foreach (var child in FindVisualChildren<TextBox>(this))
            {
                if (child.Tag != null)
                {
                    totalCells++;
                    string correct = child.Tag.ToString();
                    string user = child.Text.ToUpper();

                    if (user == correct)
                    {
                        child.Background = System.Windows.Media.Brushes.LightGreen;
                        correctCells++;
                    }
                    else if (!string.IsNullOrEmpty(user))
                    {
                        child.Background = System.Windows.Media.Brushes.LightCoral;
                    }
                    else
                    {
                        child.Background = System.Windows.Media.Brushes.White;
                    }
                }
            }

            double percent = (double)correctCells / totalCells * 100;
            MessageBox.Show($"Правильно: {correctCells} из {totalCells} клеток ({percent:F0}%)");
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            foreach (var child in FindVisualChildren<TextBox>(this))
            {
                child.Text = "";
                child.Background = System.Windows.Media.Brushes.White;
            }
            UpdateProgress();
        }

        private void ShowAnswers_Click(object sender, RoutedEventArgs e)
        {
            foreach (var child in FindVisualChildren<TextBox>(this))
            {
                if (child.Tag != null)
                {
                    child.Text = child.Tag.ToString();
                    child.Background = System.Windows.Media.Brushes.LightBlue;
                }
            }
            UpdateProgress();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Show();

            // Закрываем текущее окно игры
            this.Close();
        }

        // Простой способ найти все TextBox'ы
        private System.Collections.Generic.IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = System.Windows.Media.VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}