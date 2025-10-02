using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyApp
{
    public partial class SudokuWindow : Window
    {
        private TextBox[,] cells = new TextBox[9, 9];

        // Фиксированная головоломка (0 = пустая клетка)
        private int[,] puzzle = new int[9, 9]
        {
            {5,3,0,0,7,0,0,0,0},
            {6,0,0,1,9,5,0,0,0},
            {0,9,8,0,0,0,0,6,0},
            {8,0,0,0,6,0,0,0,3},
            {4,0,0,8,0,3,0,0,1},
            {7,0,0,0,2,0,0,0,6},
            {0,6,0,0,0,0,2,8,0},
            {0,0,0,4,1,9,0,0,5},
            {0,0,0,0,8,0,0,7,9}
        };

        // Решение головоломки
        private int[,] solution = new int[9, 9]
        {
            {5,3,4,6,7,8,9,1,2},
            {6,7,2,1,9,5,3,4,8},
            {1,9,8,3,4,2,5,6,7},
            {8,5,9,7,6,1,4,2,3},
            {4,2,6,8,5,3,7,9,1},
            {7,1,3,9,2,4,8,5,6},
            {9,6,1,5,3,7,2,8,4},
            {2,8,7,4,1,9,6,3,5},
            {3,4,5,2,8,6,1,7,9}
        };

        public SudokuWindow()
        {
            InitializeComponent();
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            // Создаем строки и колонки
            for (int i = 0; i < 9; i++)
            {
                SudokuGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) });
                SudokuGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(40) });
            }

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    var tb = new TextBox
                    {
                        MaxLength = 1,
                        FontSize = 18,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1)
                    };

                    if (puzzle[i, j] != 0)
                    {
                        tb.Text = puzzle[i, j].ToString();
                        tb.IsReadOnly = true;
                        tb.Background = Brushes.LightGray;
                    }

                    Grid.SetRow(tb, i);
                    Grid.SetColumn(tb, j);
                    SudokuGrid.Children.Add(tb);
                    cells[i, j] = tb;
                }
            }
        }

        private void CheckSudoku_Click(object sender, RoutedEventArgs e)
        {
            int mistakes = 0;
            int totalEmpty = 0;
            int correctCount = 0;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (puzzle[i, j] == 0) // только пустые клетки
                    {
                        totalEmpty++;
                        if (cells[i, j].Text == solution[i, j].ToString())
                        {
                            correctCount++;
                            cells[i, j].Background = Brushes.LightGreen; // правильный ввод
                        }
                        else
                        {
                            mistakes++;
                            cells[i, j].Background = Brushes.LightCoral; // подсветка ошибок
                        }
                    }
                }
            }

            double percent = totalEmpty > 0 ? Math.Round((double)correctCount / totalEmpty * 100, 2) : 100;
            ResultText.Text = mistakes == 0
                ? $"Судоку решено правильно! ({percent}%)"
                : $"Вы допустили {mistakes} ошибок. Правильно: {percent}%";
        }


        private void ClearSudoku_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (puzzle[i, j] == 0)
                    {
                        cells[i, j].Text = "";
                        cells[i, j].Background = Brushes.White;
                    }
                }
            }

            ResultText.Text = "";
        }

        private void BackToMain_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Show();

            // Закрываем текущее окно игры
            this.Close();
        }
    }
}
