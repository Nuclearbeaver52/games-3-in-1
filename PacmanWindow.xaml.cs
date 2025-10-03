using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MyApp
{
    public partial class PacmanWindow : Window
    {
        private const int CellSize = 25;
        private const int Rows = 20;
        private const int Cols = 20;

        private int[,] map = new int[Rows, Cols]; // 0=путь, 1=стена, 2=еда, 3=пилюля
        private Ellipse pacman;
        private Point pacmanPos = new Point(1, 1);
        private int dx = 0, dy = 0;

        private List<Point> ghosts = new List<Point>();
        private List<Ellipse> ghostShapes = new List<Ellipse>();
        private bool pacmanPowered = false;
        private DispatcherTimer powerTimer = new DispatcherTimer();

        private DispatcherTimer gameTimer = new DispatcherTimer();
        private Random rand = new Random();
        private int score = 0;

        public PacmanWindow()
        {
            InitializeComponent();
            InitMap();
            InitGame();
        }

        private void InitMap()
        {
            // границы
            for (int y = 0; y < Rows; y++)
                for (int x = 0; x < Cols; x++)
                    map[y, x] = (x == 0 || x == Cols - 1 || y == 0 || y == Rows - 1) ? 1 : 2;

            // внутренние стены
            for (int y = 2; y < Rows - 2; y += 4)
                for (int x = 2; x < Cols - 2; x += 4)
                    map[y, x] = 1;

            // супер-пилюли в углах
            map[1, 1] = 3;
            map[1, Cols - 2] = 3;
            map[Rows - 2, 1] = 3;
            map[Rows - 2, Cols - 2] = 3;
        }

        private void InitGame()
        {
            GameCanvas.Children.Clear();

            // отрисовка стен и еды
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Cols; x++)
                {
                    Rectangle rect = new Rectangle
                    {
                        Width = CellSize,
                        Height = CellSize,
                        Fill = map[y, x] == 1 ? Brushes.Blue : Brushes.Black,
                        Stroke = Brushes.Gray,
                        StrokeThickness = 0.5
                    };
                    Canvas.SetLeft(rect, x * CellSize);
                    Canvas.SetTop(rect, y * CellSize);
                    GameCanvas.Children.Add(rect);

                    if (map[y, x] == 2 || map[y, x] == 3)
                    {
                        Ellipse dot = new Ellipse
                        {
                            Width = 6,
                            Height = 6,
                            Fill = map[y, x] == 2 ? Brushes.Yellow : Brushes.Orange
                        };
                        Canvas.SetLeft(dot, x * CellSize + 7);
                        Canvas.SetTop(dot, y * CellSize + 7);
                        GameCanvas.Children.Add(dot);
                    }
                }
            }

            // Pacman
            pacman = new Ellipse
            {
                Width = CellSize - 2,
                Height = CellSize - 2,
                Fill = Brushes.Yellow
            };
            GameCanvas.Children.Add(pacman);
            DrawPacman();

            // Привидения
            ghosts.Clear();
            ghostShapes.Clear();
            ghosts.Add(new Point(Cols - 2, 1));
            ghosts.Add(new Point(1, Rows - 2));
            ghosts.Add(new Point(Cols - 2, Rows - 2));
            foreach (var g in ghosts)
            {
                Ellipse ghost = new Ellipse
                {
                    Width = CellSize - 2,
                    Height = CellSize - 2,
                    Fill = Brushes.Red
                };
                GameCanvas.Children.Add(ghost);
                ghostShapes.Add(ghost);
                Canvas.SetLeft(ghost, g.X * CellSize + 1);
                Canvas.SetTop(ghost, g.Y * CellSize + 1);
            }

            // Таймер игры
            gameTimer.Interval = TimeSpan.FromMilliseconds(200);
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();

            // Таймер супер-силы
            powerTimer.Interval = TimeSpan.FromSeconds(5);
            powerTimer.Tick += PowerTimer_Tick;
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            MovePacman();
            MoveGhosts();
            CheckCollisions();
        }

        private void PowerTimer_Tick(object sender, EventArgs e)
        {
            pacmanPowered = false;
            foreach (var ghost in ghostShapes) ghost.Fill = Brushes.Red;
            powerTimer.Stop();
        }

        private void MovePacman()
        {
            Point newPos = new Point(pacmanPos.X + dx, pacmanPos.Y + dy);
            if (IsWalkable(newPos))
                pacmanPos = newPos;

            DrawPacman();

            // Еда и пилюли
            int cell = map[(int)pacmanPos.Y, (int)pacmanPos.X];
            if (cell == 2)
            {
                map[(int)pacmanPos.Y, (int)pacmanPos.X] = 0;
                score += 10;
                RedrawMap();
            }
            else if (cell == 3)
            {
                map[(int)pacmanPos.Y, (int)pacmanPos.X] = 0;
                score += 50;
                pacmanPowered = true;
                foreach (var ghost in ghostShapes) ghost.Fill = Brushes.LightBlue;
                powerTimer.Start();
                RedrawMap();
            }
        }

        private void DrawPacman()
        {
            Canvas.SetLeft(pacman, pacmanPos.X * CellSize + 1);
            Canvas.SetTop(pacman, pacmanPos.Y * CellSize + 1);
        }

        private void MoveGhosts()
        {
            for (int i = 0; i < ghosts.Count; i++)
            {
                Point g = ghosts[i];
                List<Point> possible = new List<Point>();
                if (IsWalkable(new Point(g.X + 1, g.Y))) possible.Add(new Point(g.X + 1, g.Y));
                if (IsWalkable(new Point(g.X - 1, g.Y))) possible.Add(new Point(g.X - 1, g.Y));
                if (IsWalkable(new Point(g.X, g.Y + 1))) possible.Add(new Point(g.X, g.Y + 1));
                if (IsWalkable(new Point(g.X, g.Y - 1))) possible.Add(new Point(g.X, g.Y - 1));

                if (possible.Count > 0)
                    ghosts[i] = possible[rand.Next(possible.Count)];

                Canvas.SetLeft(ghostShapes[i], ghosts[i].X * CellSize + 1);
                Canvas.SetTop(ghostShapes[i], ghosts[i].Y * CellSize + 1);
            }
        }

        private void CheckCollisions()
        {
            for (int i = 0; i < ghosts.Count; i++)
            {
                Point g = ghosts[i];
                if (g.X == pacmanPos.X && g.Y == pacmanPos.Y)
                {
                    if (pacmanPowered)
                    {
                        ghosts[i] = new Point(Cols / 2, Rows / 2); // сброс в центр
                        ghostShapes[i].Fill = Brushes.Red;
                        score += 200;
                    }
                    else
                    {
                        gameTimer.Stop();
                        MessageBox.Show($"Game Over! Score: {score}");
                        Close();
                    }
                }
            }

            // Проверка победы
            bool anyFood = false;
            for (int y = 0; y < Rows; y++)
                for (int x = 0; x < Cols; x++)
                    if (map[y, x] == 2 || map[y, x] == 3) anyFood = true;

            if (!anyFood)
            {
                gameTimer.Stop();
                MessageBox.Show($"You Win! Score: {score}");
                Close();
            }
        }

        private bool IsWalkable(Point p)
        {
            return p.X >= 0 && p.X < Cols && p.Y >= 0 && p.Y < Rows && map[(int)p.Y, (int)p.X] != 1;
        }

        private void RedrawMap()
        {
            for (int i = GameCanvas.Children.Count - 1; i >= 0; i--)
            {
                if (GameCanvas.Children[i] is Ellipse e && e != pacman && !ghostShapes.Contains(e))
                    GameCanvas.Children.RemoveAt(i);
            }

            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Cols; x++)
                {
                    if (map[y, x] == 2 || map[y, x] == 3)
                    {
                        Ellipse dot = new Ellipse
                        {
                            Width = 6,
                            Height = 6,
                            Fill = map[y, x] == 2 ? Brushes.Yellow : Brushes.Orange
                        };
                        Canvas.SetLeft(dot, x * CellSize + 7);
                        Canvas.SetTop(dot, y * CellSize + 7);
                        GameCanvas.Children.Add(dot);
                    }
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up: dx = 0; dy = -1; break;
                case Key.Down: dx = 0; dy = 1; break;
                case Key.Left: dx = -1; dy = 0; break;
                case Key.Right: dx = 1; dy = 0; break;
            }
        }
    }
}
