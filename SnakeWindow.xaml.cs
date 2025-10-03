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
    public partial class SnakeWindow : Window
    {
        private DispatcherTimer timer = new DispatcherTimer();
        private List<Rectangle> snakeParts = new List<Rectangle>();
        private int directionX = 20;
        private int directionY = 0;
        private Rectangle food;
        private Random rand = new Random();

        public SnakeWindow()
        {
            InitializeComponent();
            InitGame();
        }

        private void InitGame()
        {
            snakeParts.Clear();
            GameCanvas.Children.Clear();
            directionX = 20; directionY = 0;

            // Начальная змейка
            Rectangle head = new Rectangle { Width = 20, Height = 20, Fill = Brushes.Green };
            Canvas.SetLeft(head, 200);
            Canvas.SetTop(head, 200);
            snakeParts.Add(head);
            GameCanvas.Children.Add(head);

            // Создаем еду
            food = new Rectangle { Width = 20, Height = 20, Fill = Brushes.Red };
            SpawnFood();
            GameCanvas.Children.Add(food);

            // Таймер
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void SpawnFood()
        {
            int maxX = (int)(GameCanvas.ActualWidth / 20);
            int maxY = (int)(GameCanvas.ActualHeight / 20);
            Canvas.SetLeft(food, rand.Next(0, maxX) * 20);
            Canvas.SetTop(food, rand.Next(0, maxY) * 20);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            MoveSnake();
        }

        private void MoveSnake()
        {
            double headX = Canvas.GetLeft(snakeParts[0]);
            double headY = Canvas.GetTop(snakeParts[0]);
            double newX = headX + directionX;
            double newY = headY + directionY;

            // Проверка столкновений с границами
            if (newX < 0 || newY < 0 || newX >= GameCanvas.ActualWidth || newY >= GameCanvas.ActualHeight)
            {
                timer.Stop();
                MessageBox.Show("Игра окончена!");
                Close();
                return;
            }

            // Создаем новый кусок головы
            Rectangle newHead = new Rectangle { Width = 20, Height = 20, Fill = Brushes.Green };
            Canvas.SetLeft(newHead, newX);
            Canvas.SetTop(newHead, newY);
            GameCanvas.Children.Add(newHead);
            snakeParts.Insert(0, newHead);

            // Проверка на еду
            if (Math.Abs(newX - Canvas.GetLeft(food)) < 1 && Math.Abs(newY - Canvas.GetTop(food)) < 1)
            {
                SpawnFood(); // новая еда
            }
            else
            {
                // Удаляем хвост
                GameCanvas.Children.Remove(snakeParts[snakeParts.Count - 1]);
                snakeParts.RemoveAt(snakeParts.Count - 1);
            }

            // Проверка столкновения с телом
            for (int i = 1; i < snakeParts.Count; i++)
            {
                if (Canvas.GetLeft(snakeParts[i]) == newX && Canvas.GetTop(snakeParts[i]) == newY)
                {
                    timer.Stop();
                    MessageBox.Show("Игра окончена!");
                    Close();
                    return;
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up: if (directionY == 0) { directionX = 0; directionY = -20; } break;
                case Key.Down: if (directionY == 0) { directionX = 0; directionY = 20; } break;
                case Key.Left: if (directionX == 0) { directionX = -20; directionY = 0; } break;
                case Key.Right: if (directionX == 0) { directionX = 20; directionY = 0; } break;
            }
        }
    }
}
