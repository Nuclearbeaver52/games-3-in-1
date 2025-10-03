using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MyApp
{
    public partial class MinesweeperWindow : Window
    {
        private const int Rows = 8;
        private const int Cols = 8;
        private const int Mines = 10;
        private Button[,] buttons = new Button[Rows, Cols];
        private bool[,] hasMine = new bool[Rows, Cols];

        public MinesweeperWindow()
        {
            InitializeComponent(); // теперь работает, так как XAML привязан
            InitGame();
        }

        private void InitGame()
        {
            Random rand = new Random();

            // расставляем мины
            for (int i = 0; i < Mines;)
            {
                int r = rand.Next(Rows);
                int c = rand.Next(Cols);
                if (!hasMine[r, c])
                {
                    hasMine[r, c] = true;
                    i++;
                }
            }

            // создаем кнопки
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    Button btn = new Button();
                    btn.Click += Btn_Click;
                    btn.Tag = new Tuple<int, int>(r, c);
                    buttons[r, c] = btn;
                    GameGrid.Children.Add(btn);
                }
            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var pos = (Tuple<int, int>)btn.Tag;
            int r = pos.Item1, c = pos.Item2;

            if (hasMine[r, c])
            {
                btn.Background = Brushes.Red;
                MessageBox.Show("Boom! Game Over.");
                Close();
                return;
            }

            int count = 0;
            for (int dr = -1; dr <= 1; dr++)
                for (int dc = -1; dc <= 1; dc++)
                    if (r + dr >= 0 && r + dr < Rows && c + dc >= 0 && c + dc < Cols && hasMine[r + dr, c + dc])
                        count++;

            btn.Content = count > 0 ? count.ToString() : "";
            btn.IsEnabled = false;
        }
    }
}
