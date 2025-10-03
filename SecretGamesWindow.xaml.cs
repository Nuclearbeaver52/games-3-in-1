using System.Windows;

namespace MyApp
{
    public partial class SecretGamesWindow : Window
    {
        public SecretGamesWindow()
        {
            InitializeComponent();
        }

        private void OpenSnake_Click(object sender, RoutedEventArgs e)
        {
            SnakeWindow snake = new SnakeWindow();
            snake.Show();
        }

        private void OpenMario_Click(object sender, RoutedEventArgs e)
        {
            MarioWindow mario = new MarioWindow();
            mario.Show();
        }

        private void OpenPingPong_Click(object sender, RoutedEventArgs e)
        {
            PingPongWindow pong = new PingPongWindow();
            pong.Show();
        }

        private void OpenTetris_Click(object sender, RoutedEventArgs e)
        {
             TetrisWindow tetris = new TetrisWindow();
            tetris.Show();
        }
        private void OpenPacman_Click(object sender, RoutedEventArgs e)
        {
            PacmanWindow pacman = new PacmanWindow();
            pacman.Show();
        }

        // --- Новые игры ---
        private void OpenFlappyBird_Click(object sender, RoutedEventArgs e)
        {
            FlappyBirdWindow flappy = new FlappyBirdWindow();
            flappy.Show();
        }

        private void OpenMinesweeper_Click(object sender, RoutedEventArgs e)
        {
            MinesweeperWindow minesweeper = new MinesweeperWindow();
            minesweeper.Show();
        }

        


        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
