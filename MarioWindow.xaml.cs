using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MyApp
{
    public partial class MarioWindow : Window
    {
        private Rectangle player;
        private Rectangle ground;
        private double velocityY = 0;
        private bool isJumping = false;
        private DispatcherTimer timer = new DispatcherTimer();

        private bool moveLeft, moveRight;

        public MarioWindow()
        {
            InitializeComponent();
            InitGame();
        }

        private void InitGame()
        {
            // Земля
            ground = new Rectangle { Width = 600, Height = 50, Fill = Brushes.SaddleBrown };
            Canvas.SetLeft(ground, 0);
            Canvas.SetTop(ground, 350);
            GameCanvas.Children.Add(ground);

            // Игрок
            player = new Rectangle { Width = 30, Height = 50, Fill = Brushes.Red };
            Canvas.SetLeft(player, 50);
            Canvas.SetTop(player, 300);
            GameCanvas.Children.Add(player);

            // Таймер движения
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Гравитация
            velocityY += 1;
            Canvas.SetTop(player, Canvas.GetTop(player) + velocityY);

            // Столкновение с землей
            if (Canvas.GetTop(player) + player.Height >= Canvas.GetTop(ground))
            {
                Canvas.SetTop(player, Canvas.GetTop(ground) - player.Height);
                velocityY = 0;
                isJumping = false;
            }

            // Левое/правое движение
            if (moveLeft) Canvas.SetLeft(player, Canvas.GetLeft(player) - 5);
            if (moveRight) Canvas.SetLeft(player, Canvas.GetLeft(player) + 5);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left) moveLeft = true;
            if (e.Key == Key.Right) moveRight = true;
            if (e.Key == Key.Space && !isJumping)
            {
                velocityY = -15;
                isJumping = true;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left) moveLeft = false;
            if (e.Key == Key.Right) moveRight = false;
        }
    }
}
