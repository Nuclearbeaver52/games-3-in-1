using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MyApp
{
    public partial class PingPongWindow : Window
    {
        private Rectangle playerPaddle;
        private Rectangle aiPaddle;
        private Ellipse ball;

        private double ballXSpeed = 4;
        private double ballYSpeed = 4;

        private DispatcherTimer timer = new DispatcherTimer();

        private bool moveUp, moveDown;

        public PingPongWindow()
        {
            InitializeComponent();
            InitGame();
        }

        private void InitGame()
        {
            // Игрок
            playerPaddle = new Rectangle { Width = 15, Height = 80, Fill = Brushes.White };
            Canvas.SetLeft(playerPaddle, 20);
            Canvas.SetTop(playerPaddle, 150);
            GameCanvas.Children.Add(playerPaddle);

            // AI
            aiPaddle = new Rectangle { Width = 15, Height = 80, Fill = Brushes.White };
            Canvas.SetLeft(aiPaddle, 565);
            Canvas.SetTop(aiPaddle, 150);
            GameCanvas.Children.Add(aiPaddle);

            // Мяч
            ball = new Ellipse { Width = 20, Height = 20, Fill = Brushes.Yellow };
            Canvas.SetLeft(ball, 290);
            Canvas.SetTop(ball, 190);
            GameCanvas.Children.Add(ball);

            // Таймер
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Движение игрока
            double playerY = Canvas.GetTop(playerPaddle);
            if (moveUp && playerY > 0) Canvas.SetTop(playerPaddle, playerY - 6);
            if (moveDown && playerY + playerPaddle.Height < GameCanvas.ActualHeight) Canvas.SetTop(playerPaddle, playerY + 6);

            // Движение AI
            double aiY = Canvas.GetTop(aiPaddle);
            double ballY = Canvas.GetTop(ball);
            if (ballY + 10 < aiY + aiPaddle.Height / 2 && aiY > 0) Canvas.SetTop(aiPaddle, aiY - 4);
            if (ballY + 10 > aiY + aiPaddle.Height / 2 && aiY + aiPaddle.Height < GameCanvas.ActualHeight) Canvas.SetTop(aiPaddle, aiY + 4);

            // Движение мяча
            double ballX = Canvas.GetLeft(ball) + ballXSpeed;
            double ballYNew = Canvas.GetTop(ball) + ballYSpeed;

            Canvas.SetLeft(ball, ballX);
            Canvas.SetTop(ball, ballYNew);

            // Столкновение с верхом/низом
            if (ballYNew <= 0 || ballYNew + ball.Height >= GameCanvas.ActualHeight) ballYSpeed = -ballYSpeed;

            // Столкновение с игроком
            if (ballX <= Canvas.GetLeft(playerPaddle) + playerPaddle.Width &&
                ballYNew + ball.Height >= Canvas.GetTop(playerPaddle) &&
                ballYNew <= Canvas.GetTop(playerPaddle) + playerPaddle.Height)
            {
                ballXSpeed = Math.Abs(ballXSpeed);
            }

            // Столкновение с AI
            if (ballX + ball.Width >= Canvas.GetLeft(aiPaddle) &&
                ballYNew + ball.Height >= Canvas.GetTop(aiPaddle) &&
                ballYNew <= Canvas.GetTop(aiPaddle) + aiPaddle.Height)
            {
                ballXSpeed = -Math.Abs(ballXSpeed);
            }

            // Мяч за пределами
            if (ballX < 0 || ballX + ball.Width > GameCanvas.ActualWidth)
            {
                ballXSpeed = 4;
                ballYSpeed = 4;
                Canvas.SetLeft(ball, 290);
                Canvas.SetTop(ball, 190);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up) moveUp = true;
            if (e.Key == Key.Down) moveDown = true;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up) moveUp = false;
            if (e.Key == Key.Down) moveDown = false;
        }
    }
}
