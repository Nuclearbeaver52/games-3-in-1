using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MyApp
{
    public partial class CrosswordWindow : Window
    {
        private const int Rows = 10;
        private const int Cols = 10;

        private char?[,] solution = new char?[Rows, Cols];
        private TextBox[,] cells = new TextBox[Rows, Cols];

        private List<Word> words = new List<Word>();

        public CrosswordWindow()
        {
            InitializeComponent();
            BuildPuzzle();
            RenderBoard();
            RenderClues();
        }

        // ---------- ПАЗЛ (все слова связаны) ----------
        private void BuildPuzzle()
        {
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    solution[r, c] = null;
            words.Clear();

            // 1. Процессов(Across) – центральная горизонталь
            PlaceWordAcross(4, 1, "ПРОЦЕССОР", " Как называется Мозг компьютера.", 1);

            // Вертикальные слова стартуют на буквах Процессор(все пересекаются с 1)
            // 2. Пираты (Down)  — стартует на букве 'К'
            PlaceWordDown(4, 1, "ПИРАТЫ", "Кто грабил карибское море", 2);
            // 3. ОКОНО (Down)    — стартует на букве 'О'
            PlaceWordDown(4, 3, "ОКНО", "Проём в стене для света и воздуха.", 4);
            // 4. РАДУГА (Down)   — стартует на букве 'М'
            PlaceWordDown(4, 2, "РАДУГА", "Многокрасочная дуга на небе после дождя.", 3);
            // 5. СОБАКА (Down)    — стартует на букве 'Н'
            PlaceWordDown(4, 6, "СОБАКА", "Верный друг человека.", 5);
            // 6. СЛОН (Down)  — стартует на букве 'А'
            PlaceWordDown(4, 7, "СЛОН", "Кто боиться мышей.", 6);
           
            
         
        }

        // ---------- Размещение слов ----------
        private void PlaceWordAcross(int r, int c, string word, string clue, int number)
        {
            var coords = new List<Tuple<int, int>>();
            for (int i = 0; i < word.Length; i++)
            {
                solution[r, c + i] = word[i];
                coords.Add(Tuple.Create(r, c + i));
            }
            words.Add(new Word { Number = number, Clue = clue, Cells = coords });
        }

        private void PlaceWordDown(int r, int c, string word, string clue, int number)
        {
            var coords = new List<Tuple<int, int>>();
            for (int i = 0; i < word.Length; i++)
            {
                solution[r + i, c] = word[i];
                coords.Add(Tuple.Create(r + i, c));
            }
            words.Add(new Word { Number = number, Clue = clue, Cells = coords });
        }

        // ---------- Отрисовка ----------
        private void RenderBoard()
        {
            Board.Children.Clear();

            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (solution[r, c] == null)
                    {
                        Board.Children.Add(new Border
                        {
                            Background = Brushes.LightGray,
                            BorderBrush = Brushes.White,
                            BorderThickness = new Thickness(0.5)
                        });
                        continue;
                    }

                    // несколько слов могут стартовать в одной клетке -> "1,2"
                    var starts = words.Where(w => w.Cells.Count > 0 &&
                                                  w.Cells[0].Item1 == r &&
                                                  w.Cells[0].Item2 == c)
                                      .Select(w => w.Number.ToString())
                                      .ToList();

                    if (starts.Count > 0)
                        AddNumberedCell(r, c, string.Join(",", starts));
                    else
                        AddNormalCell(r, c);
                }
            }
        }

        private void AddNormalCell(int r, int c)
        {
            var tb = new TextBox
            {
                MaxLength = 1,
                CharacterCasing = CharacterCasing.Upper,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            tb.PreviewTextInput += OnlyCyrillic;
            cells[r, c] = tb;
            Board.Children.Add(tb);
        }

        private void AddNumberedCell(int r, int c, string numberText)
        {
            var panel = new Grid();

            var tb = new TextBox
            {
                MaxLength = 1,
                CharacterCasing = CharacterCasing.Upper,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            tb.PreviewTextInput += OnlyCyrillic;
            cells[r, c] = tb;

            var num = new TextBlock
            {
                Text = numberText,
                FontSize = 10,
                Margin = new Thickness(2, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            panel.Children.Add(tb);
            panel.Children.Add(num);
            Board.Children.Add(panel);
        }

        private void RenderClues()
        {
            // единый список с номерами
            Clues.ItemsSource = words.OrderBy(w => w.Number)
                                     .Select(w => w.Number + ". " + w.Clue)
                                     .ToList();
        }

        // ---------- Ввод / проверка ----------
        private void OnlyCyrillic(object sender, TextCompositionEventArgs e)
        {
            string s = e.Text.ToUpper(new CultureInfo("ru-RU"));
            e.Handled = !(s.Length == 1 && (s[0] == 'Ё' || (s[0] >= 'А' && s[0] <= 'Я')));
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            int correct = 0;
            foreach (var w in words)
            {
                bool ok = true;
                foreach (var pos in w.Cells)
                {
                    var tb = cells[pos.Item1, pos.Item2];
                    if (tb == null || string.IsNullOrEmpty(tb.Text) ||
                        tb.Text[0] != solution[pos.Item1, pos.Item2])
                    {
                        ok = false; break;
                    }
                }
                if (ok) correct++;
            }
            Status.Text = "Правильных слов: " + correct + " из " + words.Count;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            foreach (var tb in cells)
                if (tb != null) tb.Clear();
            Status.Text = "Очищено.";
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow().Show();
            Close();
        }
    }

    public class Word
    {
        public int Number { get; set; }
        public string Clue { get; set; }
        public List<Tuple<int, int>> Cells { get; set; }
    }
}