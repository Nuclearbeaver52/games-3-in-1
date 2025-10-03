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
    public partial class TetrisWindow : Window
    {
        private readonly int rows = 20;
        private readonly int cols = 10;
        private readonly int blockSize = 25;

        private Rectangle[,] grid;
        private Brush[,] field; // фиксированные блоки

        private DispatcherTimer timer = new DispatcherTimer();
        private List<Point> currentPiece;
        private Brush currentColor;
        private Point piecePosition;
        private Random rand = new Random();

        private bool isDownPressed = false;

        // Набор фигур
        private readonly List<List<Point>> figures = new List<List<Point>>
        {
            new List<Point>{ new Point(0,0), new Point(1,0), new Point(0,1), new Point(1,1) }, // O
            new List<Point>{ new Point(0,0), new Point(0,1), new Point(0,2), new Point(0,3) }, // I
            new List<Point>{ new Point(0,0), new Point(1,0), new Point(1,1), new Point(2,1) }, // S
            new List<Point>{ new Point(1,0), new Point(2,0), new Point(0,1), new Point(1,1) }, // Z
            new List<Point>{ new Point(0,0), new Point(0,1), new Point(0,2), new Point(1,2) }, // L
            new List<Point>{ new Point(1,0), new Point(1,1), new Point(1,2), new Point(0,2) }, // J
            new List<Point>{ new Point(1,0), new Point(0,1), new Point(1,1), new Point(2,1) }  // T
        };

        public TetrisWindow()
        {
            InitializeComponent();
            InitGame();
        }

        private void InitGame()
        {
            grid = new Rectangle[cols, rows];
            field = new Brush[cols, rows];
            GameCanvas.Children.Clear();

            // Сетка
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    Rectangle rect = new Rectangle
                    {
                        Width = blockSize - 1,
                        Height = blockSize - 1,
                        Fill = Brushes.Black,
                        Stroke = Brushes.Gray,
                        StrokeThickness = 0.5
                    };
                    Canvas.SetLeft(rect, x * blockSize);
                    Canvas.SetTop(rect, y * blockSize);
                    GameCanvas.Children.Add(rect);
                    grid[x, y] = rect;
                    field[x, y] = Brushes.Black;
                }
            }

            SpawnPiece();

            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void SpawnPiece()
        {
            currentPiece = new List<Point>(figures[rand.Next(figures.Count)]);
            currentColor = new SolidColorBrush(Color.FromRgb(
                (byte)rand.Next(50, 256),
                (byte)rand.Next(50, 256),
                (byte)rand.Next(50, 256)));
            piecePosition = new Point(cols / 2 - 1, 0);

            if (!IsValidPosition(piecePosition, currentPiece))
            {
                timer.Stop();
                MessageBox.Show("Game Over!");
                Close();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (isDownPressed)
                MovePiece(0, 1);
            else
                MovePiece(0, 1);
        }

        private void MovePiece(int dx, int dy)
        {
            var newPos = new Point(piecePosition.X + dx, piecePosition.Y + dy);

            if (!IsValidPosition(newPos, currentPiece))
            {
                if (dy == 1)
                {
                    // фиксируем фигуру
                    foreach (var p in currentPiece)
                    {
                        int x = (int)(p.X + piecePosition.X);
                        int y = (int)(p.Y + piecePosition.Y);
                        if (y >= 0)
                            field[x, y] = currentColor;
                    }

                    ClearLines();
                    SpawnPiece();
                }
                return;
            }

            piecePosition = newPos;
            DrawGrid();
        }

        private void DrawGrid()
        {
            // сначала рисуем поле
            for (int x = 0; x < cols; x++)
                for (int y = 0; y < rows; y++)
                    grid[x, y].Fill = field[x, y];

            // рисуем текущую фигуру
            foreach (var p in currentPiece)
            {
                int x = (int)(p.X + piecePosition.X);
                int y = (int)(p.Y + piecePosition.Y);
                if (x >= 0 && x < cols && y >= 0 && y < rows)
                    grid[x, y].Fill = currentColor;
            }
        }

        private bool IsValidPosition(Point pos, List<Point> piece)
        {
            foreach (var p in piece)
            {
                int x = (int)(p.X + pos.X);
                int y = (int)(p.Y + pos.Y);
                if (x < 0 || x >= cols || y < 0 || y >= rows)
                    return false;
                if (field[x, y] != Brushes.Black)
                    return false;
            }
            return true;
        }

        private void ClearLines()
        {
            for (int y = rows - 1; y >= 0; y--)
            {
                bool full = true;
                for (int x = 0; x < cols; x++)
                {
                    if (field[x, y] == Brushes.Black)
                    {
                        full = false;
                        break;
                    }
                }

                if (full)
                {
                    // опускаем все строки вниз
                    for (int i = y; i > 0; i--)
                        for (int x = 0; x < cols; x++)
                            field[x, i] = field[x, i - 1];

                    for (int x = 0; x < cols; x++)
                        field[x, 0] = Brushes.Black;

                    y++; // пересканировать ту же строку
                }
            }
            DrawGrid();
        }

        private void RotatePiece()
        {
            var rotated = new List<Point>();
            foreach (var p in currentPiece)
                rotated.Add(new Point(-p.Y, p.X)); // поворот 90°

            if (IsValidPosition(piecePosition, rotated))
                currentPiece = rotated;

            DrawGrid();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    MovePiece(-1, 0);
                    break;
                case Key.Right:
                    MovePiece(1, 0);
                    break;
                case Key.Down:
                    isDownPressed = true;
                    break;
                case Key.Up:
                    RotatePiece();
                    break;
                case Key.Escape:
                    Close();
                    break;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
                isDownPressed = false;
        }
    }
}
