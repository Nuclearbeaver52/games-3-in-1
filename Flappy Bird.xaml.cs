using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MyApp
{
    public partial class FlappyBirdWindow : Window
    {
        private Ellipse bird;
        private double birdY = 150;
        private double velocity = 0;
        private DispatcherTimer timer = new DispatcherTimer();
        private Random rand = new Random();
        private double gravity = 0.5;
        private Rectangle pipeTop, pipeBottom;
        private double pipeX = 300;
        private double pipeGap = 120;
        private int score = 0;

        public FlappyBirdWindow()
        {
            InitializeComponent();
            this.Loaded += FlappyBirdWindow_Loaded;
        }

        private void FlappyBirdWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitGame();
        }

        private void InitGame()
        {
            GameCanvas.Children.Clear();

            // Птица
            bird = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = Brushes.Yellow
            };
            GameCanvas.Children.Add(bird);
            Canvas.SetLeft(bird, 50);
            Canvas.SetTop(bird, birdY);

            // Трубы
            pipeTop = new Rectangle { Width = 50, Height = 150, Fill = Brushes.Green };
            pipeBottom = new Rectangle { Width = 50, Height = 150, Fill = Brushes.Green };
            GameCanvas.Children.Add(pipeTop);
            GameCanvas.Children.Add(pipeBottom);
            ResetPipe();

            // Таймер
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Движение птицы
            birdY += velocity;
            velocity += gravity;
            Canvas.SetTop(bird, birdY);

            // Движение труб
            pipeX -= 4;
            Canvas.SetLeft(pipeTop, pipeX);
            Canvas.SetLeft(pipeBottom, pipeX);

            // Проверка столкновений
            if (birdY < 0 || birdY + bird.Height > GameCanvas.ActualHeight ||
                (pipeX < 70 && pipeX + pipeTop.Width > 30 &&
                 (birdY < Canvas.GetTop(pipeTop) + pipeTop.Height || birdY + bird.Height > Canvas.GetTop(pipeBottom))))
            {
                timer.Stop();
                MessageBox.Show($"Game Over! Score: {score}");
                Close();
            }

            // Перезапуск трубы
            if (pipeX < -50)
            {
                ResetPipe();
                score++;
            }
        }

        private void ResetPipe()
        {
            pipeX = GameCanvas.ActualWidth;

            // Максимальная высота верхней трубы, чтобы нижняя не была отрицательной
            double maxTopHeight = Math.Max(50, GameCanvas.ActualHeight - pipeGap - 20);
            double topHeight = rand.Next(50, (int)maxTopHeight);

            double bottomHeight = GameCanvas.ActualHeight - topHeight - pipeGap;
            if (bottomHeight < 20) bottomHeight = 20;

            pipeTop.Height = topHeight;
            pipeBottom.Height = bottomHeight;

            Canvas.SetTop(pipeTop, 0);
            Canvas.SetTop(pipeBottom, topHeight + pipeGap);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.Up)
            {
                velocity = -8; // прыжок вверх
            }
            if (e.Key == Key.Down)
            {
                velocity = 4; // ускоренное падение
            }
            if (e.Key == Key.Escape)
            {
                timer.Stop();
                Close();
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                velocity = 0; // прекращение ускоренного падения
            }
        }
    }
}
