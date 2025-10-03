using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyApp
{
    public partial class SudokuWindow : Window
    {
        private TextBox[,] cells = new TextBox[9, 9];
        private int[,] puzzle = new int[9, 9];
        private int[,] solution = new int[9, 9];
        private Random random = new Random();

        public SudokuWindow()
        {
            InitializeComponent();
            StartNewGame();
        }

        private void StartNewGame()
        {
            GenerateSudoku();
            InitializeGrid();
            ResultText.Text = "";
        }

        // Создание сетки TextBox
        private void InitializeGrid()
        {
            SudokuGrid.Children.Clear();
            SudokuGrid.RowDefinitions.Clear();
            SudokuGrid.ColumnDefinitions.Clear();

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
                        BorderThickness = new Thickness(1),
                        Background = Brushes.White
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

        // Генерация Sudoku
        private void GenerateSudoku()
        {
            solution = new int[9, 9];
            FillSudoku(solution);

            // Создаём головоломку, случайно убираем числа
            puzzle = new int[9, 9];
            Array.Copy(solution, puzzle, solution.Length);

            for (int i = 0; i < 81; i++)
            {
                int r = i / 9;
                int c = i % 9;
                if (random.NextDouble() < 0.5) // 50% клеток пустые
                    puzzle[r, c] = 0;
            }
        }

        // Backtracking генерация решения
        private bool FillSudoku(int[,] grid)
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (grid[row, col] == 0)
                    {
                        int[] numbers = new int[9];
                        for (int n = 0; n < 9; n++) numbers[n] = n + 1;
                        Shuffle(numbers);

                        foreach (var num in numbers)
                        {
                            if (IsSafe(grid, row, col, num))
                            {
                                grid[row, col] = num;
                                if (FillSudoku(grid)) return true;
                                grid[row, col] = 0;
                            }
                        }
                        return false;
                    }
                }
            }
            return true; // заполнено
        }

        // Проверка возможности поставить число
        private bool IsSafe(int[,] grid, int row, int col, int num)
        {
            for (int i = 0; i < 9; i++)
            {
                if (grid[row, i] == num) return false;
                if (grid[i, col] == num) return false;
            }

            int startRow = (row / 3) * 3;
            int startCol = (col / 3) * 3;
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    if (grid[startRow + r, startCol + c] == num) return false;

            return true;
        }

        private void Shuffle(int[] array)
        {
            for (int i = array.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                int temp = array[i];
                array[i] = array[j];
                array[j] = temp;
            }
        }

        // Проверка игрока
        private void CheckSudoku_Click(object sender, RoutedEventArgs e)
        {
            int totalEmpty = 0;
            int correctCount = 0;

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (puzzle[i, j] == 0) // пустые клетки
                    {
                        totalEmpty++;
                        if (cells[i, j].Text == solution[i, j].ToString())
                        {
                            correctCount++;
                            cells[i, j].Background = Brushes.LightGreen;
                        }
                        else
                        {
                            cells[i, j].Background = Brushes.LightCoral;
                        }
                    }
                }
            }

            double percent = totalEmpty > 0 ? Math.Round((double)correctCount / totalEmpty * 100, 2) : 100;
            ResultText.Text = $"Правильно: {percent}% ({correctCount}/{totalEmpty})";
        }

        // Очистка полей
        private void ClearSudoku_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (puzzle[i, j] == 0)
                    {
                        cells[i, j].Text = "";
                        cells[i, j].Background = Brushes.White;
                    }

            ResultText.Text = "";
        }

        // Новая игра
        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            StartNewGame();
        }

        // Выход на главное окно
        private void BackToMain_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Show();

            // Закрываем текущее окно игры
            this.Close();
        }
    }
}
