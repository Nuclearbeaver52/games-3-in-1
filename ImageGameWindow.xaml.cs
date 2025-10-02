using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace MyApp
{
    public partial class MoleGameWindow : Window
    {
        private Button[,] buttons = new Button[3, 3];
        private DispatcherTimer gameTimer;
        private DispatcherTimer moleTimer;
        private int score = 0;
        private int timeLeft = 60;
        private int totalMoles = 0; // все кроты, которые можно поймать
        private Random random = new Random();
        private (int, int)[] activeMoles = new (int, int)[2];

        public MoleGameWindow()
        {
            InitializeComponent();
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    var btn = new Button
                    {
                        Width = 100,
                        Height = 100,
                        FontSize = 40,
                        Content = "", // пусто, крот появится позже
                        Background = Brushes.LightGray
                    };
                    btn.Click += Cell_Click;
                    buttons[r, c] = btn;
                    GameGrid.Children.Add(btn);
                }
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Show();

            // Закрываем текущее окно игры
            this.Close();
            
        }

        private void StartGame()
        {
            score = 0;
            totalMoles = 0;
            ScoreText.Text = "0";
            timeLeft = 60;
            TimerText.Text = timeLeft.ToString();
            ClearActiveMoles();

            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromSeconds(1);
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            moleTimer = new DispatcherTimer();
            moleTimer.Interval = TimeSpan.FromMilliseconds(700);
            moleTimer.Tick += MoleTimer_Tick;
            moleTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            timeLeft--;
            TimerText.Text = timeLeft.ToString();

            if (timeLeft == 30)
            {
                // Появляются сразу 2 крота
                ClearActiveMoles();
                activeMoles[0] = (random.Next(0, 3), random.Next(0, 3));
                do
                {
                    activeMoles[1] = (random.Next(0, 3), random.Next(0, 3));
                } while (activeMoles[1] == activeMoles[0]);

                buttons[activeMoles[0].Item1, activeMoles[0].Item2].Content = "🐹";
                buttons[activeMoles[1].Item1, activeMoles[1].Item2].Content = "🐹";

                totalMoles += 2;
            }

            if (timeLeft <= 0)
            {
                EndGame();
            }
        }

        private void MoleTimer_Tick(object sender, EventArgs e)
        {
            if (timeLeft == 30) return; // фиксация двух кротов

            ClearActiveMoles();

            int r = random.Next(0, 3);
            int c = random.Next(0, 3);
            activeMoles[0] = (r, c);
            activeMoles[1] = (-1, -1);

            buttons[r, c].Content = "🐹"; // появление крота
            totalMoles++;
        }

        private void ClearActiveMoles()
        {
            foreach (var mole in activeMoles)
            {
                if (mole.Item1 >= 0 && mole.Item2 >= 0)
                    buttons[mole.Item1, mole.Item2].Content = "";
            }
            activeMoles[0] = (-1, -1);
            activeMoles[1] = (-1, -1);
        }

        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (buttons[i, j] == btn)
                    {
                        if ((i == activeMoles[0].Item1 && j == activeMoles[0].Item2) ||
                            (i == activeMoles[1].Item1 && j == activeMoles[1].Item2))
                        {
                            score++;
                            ScoreText.Text = score.ToString();
                            buttons[i, j].Content = ""; // убираем крота

                            if (timeLeft != 30)
                                activeMoles[0] = (-1, -1);
                        }
                    }
                }
            }
        }

        private void EndGame()
        {
            gameTimer.Stop();
            moleTimer.Stop();
            ClearActiveMoles();

            double percent = totalMoles > 0 ? Math.Round((double)score / totalMoles * 100, 2) : 0;
            MessageBox.Show($"Игра окончена!\nВы поймали {score} кротов из {totalMoles} ({percent}%)", "Результат");
        }
    }
}
