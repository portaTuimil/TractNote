using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TractNote;

public partial class DbView : UserControl
{
    public DbView(string DbAdress)
    {
        InitializeComponent();
        Db db = new(DbAdress);
        Visualizer visualizer = new(db, tableGrid);
    }


    public class Visualizer
    {
        private Db Db { get; set; }
        private List<float>? ColumnLengths { get; set; }
        private bool _isSyncingScroll = false;

        public Visualizer(Db db, Grid tableGrid)
        {
            Db = db;
            ColumnLengths = GetColumnWidths();
            PromptCategories(tableGrid);
            PromptValues(tableGrid);
        }

        private List<float>? GetColumnWidths()
        {
            if (Db.Values == null || Db.Values.Count == 0)
                return null;

            var columns = new List<float>();
            var rowLength = Db.Values[0].Count;
            var colLength = Db.Values.Count;
            for (int i = 0; i < rowLength; i++)
            {
                float avLength = 0;
                for (int j = 0; j < Math.Min(20, colLength); j++)
                {
                    avLength += Db.Values[j][i].Length;
                }
                avLength = avLength / Math.Min(20, colLength);
                columns.Add(avLength);
            }
            return columns;
        }


        private void SyncScrollViewer(ScrollViewer source, List<ScrollViewer> targets)
        {
            source.ScrollChanged += (s, e) =>
            {
                if (_isSyncingScroll) return;

                _isSyncingScroll = true;
                foreach (var target in targets)
                {
                    if (target != source)
                    {
                        target.Offset = new Vector(target.Offset.X, source.Offset.Y);
                    }
                }
                _isSyncingScroll = false;
            };
        }


        private void PromptCategories(Grid tableGrid)
        {
            if (Db.Categories == null || Db.Categories.Count == 0 || Db.Values == null || Db.Values.Count == 0 || ColumnLengths == null)
                return;

            int categoryCount = Db.Categories.Count;
            int columnCount = categoryCount * 2 - 1;

            for (int i = 0; i < columnCount; i++)
            {
                if (i % 2 == 0)
                {
                    int logicalIndex = i / 2;

                    tableGrid.ColumnDefinitions.Add(new ColumnDefinition
                    {
                        Width = new GridLength(ColumnLengths[logicalIndex] / ColumnLengths.Sum(), GridUnitType.Star)
                    });
                }
                else // Odd columns: GridSplitters
                {
                    tableGrid.ColumnDefinitions.Add(new ColumnDefinition
                    {
                        Width = new GridLength(0) // fixed width for splitter
                    });
                }
            }

            tableGrid.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(60),
            });
            tableGrid.RowDefinitions.Add(new RowDefinition())
;
            // Add children (TextBlocks and GridSplitters)
            for (int i = 0; i < columnCount; i++)
            {
                if (i % 2 == 0) // TextBlock
                {
                    int logicalIndex = i / 2;
                    Border border = new Border
                    {
                        Background = new SolidColorBrush(Color.FromArgb(10, 255, 255, 255)),
                        Height = 40,
                    };
                    var textBlock = new TextBlock
                    {
                        Text = Db.Categories[logicalIndex],
                        FontFamily = (FontFamily)Application.Current.Resources["RobotoSemiBold"],
                        FontSize = 16,
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                        Padding = new Thickness(7, 5, 0, 5),
                    };

                    if (i == 0)
                    {
                        border.CornerRadius = new CornerRadius(10, 0, 0, 10);
                    }
                    else if (i == columnCount - 1)
                    {
                        border.CornerRadius = new CornerRadius(0, 10, 10, 0);
                    }
                    border.Child = textBlock;
                    Grid.SetColumn(border, i);
                    Grid.SetRow(border, 0);
                    tableGrid.Children.Add(border);
                }
                else // GridSplitter
                {
                    var splitter = new GridSplitter
                    {
                        Width = 0,
                        ResizeDirection = GridResizeDirection.Columns,
                        ResizeBehavior = GridResizeBehavior.PreviousAndNext,
                        Background = Brushes.Transparent,
                    };
                    Grid.SetColumn(splitter, i);
                    Grid.SetRow(splitter, 0);
                    tableGrid.Children.Add(splitter);
                }
            }
        }


        private void PromptValues(Grid tableGrid)
        {
            if (Db.Values == null || Db.Values.Count == 0 || Db.Categories == null)
                return;

            int categoryCount = Db.Categories.Count;

            List<ScrollViewer> innerViewers = new List<ScrollViewer>();
            for (int i = 0; i < categoryCount; i++)
            {
                ScrollViewer scrollviewer;
                if (i == Db.Values[i].Count - 1)
                {
                    scrollviewer = new ScrollViewer
                    {
                        VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                        Padding = new Thickness(0,0,5,0)
                    };
                }
                else
                {
                    scrollviewer = new ScrollViewer
                    {
                        VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Hidden,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
                        Padding = new Thickness(0,0,5,0)
                    };
                }

                Grid grid = new Grid();

                for (int r = 0; r < Db.Values.Count + 1; r++)
                {
                    grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40) });
                }

                scrollviewer.Content = grid;
                innerViewers.Add(scrollviewer);
            }

            foreach (var viewer in innerViewers)
            {
                SyncScrollViewer(viewer, innerViewers);
            }


            for (int row = 0; row < Db.Values.Count + 1; row++)
            {
                for (int col = 0; col < Db.Values[row == Db.Values.Count ? 0 : row].Count; col++)
                {
                    Border border = new Border
                    {
                        BorderThickness = col == Db.Values[0].Count - 1
                            ?
                            (row == Db.Values.Count
                                ? new Thickness(0)
                                : new Thickness(0, 0, 0, 0.4))
                            :
                            (row == Db.Values.Count
                                ? new Thickness(0, 0, 0, 0)
                                : new Thickness(0, 0, 0.4, 0.4)),
                        BorderBrush = new SolidColorBrush(Color.Parse("#66AAAACC")),
                        Padding = new Thickness(7, 5),
                    };

                    TextBlock textBlock;

                    if (row == Db.Values.Count)
                    {
                        if (col == 0)
                        {
                            textBlock = new TextBlock
                            {
                                Text = "Total: " + Db.Values.Count,
                                FontFamily = (FontFamily)Application.Current.Resources["RobotoSemiBold"],
                                FontSize = 16,
                                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                                HorizontalAlignment = row == Db.Values.Count - 1
                                    ? Avalonia.Layout.HorizontalAlignment.Left
                                    : Avalonia.Layout.HorizontalAlignment.Right
                            };

                            Grid.SetColumnSpan(border, Db.Values[0].Count);
                        }
                        else
                        {
                            textBlock = new TextBlock { Text = "" };
                        }
                    }
                    else
                    {
                        textBlock = new TextBlock
                        {
                            Text = Db.Values[row][col],
                            TextWrapping = TextWrapping.Wrap,
                            Height = 20,
                            FontFamily = (FontFamily)Application.Current.Resources["RobotoRegular"]
                        };
                    }

                    border.Child = textBlock;

                    Grid innerGrid = innerViewers[col].Content as Grid;
                    if (innerGrid != null)
                    {
                        Grid.SetRow(border, row);
                        Grid.SetColumn(border, 0);
                        innerGrid.Children.Add(border);
                    }
                }
            }

            for (int i = 0; i < innerViewers.Count; i++)
            {
                Grid.SetColumn(innerViewers[i], 2 * i);
                Grid.SetRow(innerViewers[i], 1);
                tableGrid.Children.Add(innerViewers[i]);
            }
        }
    }



    public class Db
    {
        public string Name { get; private set; }
        private readonly string? Text;
        private readonly List<List<List<string>>>? ContentList;
        public List<string>? Categories { get; private set; }
        public List<List<string>>? Values { get; private set; }


        public Db(string name)
        {
            Name = name;
            Text = GetContent(name + ".csv");
            if (Text != null)
            {
                ContentList = SeparateContent(Text);
                Categories = ContentList[0][0];
                Values = ContentList[1];
            }
        }

        private static string? GetContent(string file)
        {
            try
            {
                string baseDir = AppContext.BaseDirectory;
                string filePath = System.IO.Path.Combine(baseDir, "..", "..", "..", "..", "TableViewer", "Db", file);
                return File.ReadAllText(filePath);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading file: {ex.Message}");
                return null;
            }
        }


        private static List<List<List<string>>> SeparateContent(string Text)
        {
            using var reader = new StringReader(Text);
            string? categories = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(categories))
            {
                return [[new List<string>(), []]];
            }
            return [[categories.Split(',').Select(s => s.Trim()).ToList()], GetValues(reader)];
        }


        private static List<List<string>> GetValues(StringReader reader)
        {
            var lines = new List<List<string>>();
            using (reader)
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    List<string> row = [];
                    if (line.Contains('"'))
                    {
                        string word = "";
                        bool inside = false;

                        foreach (char c in line)
                        {
                            if (c == '"')
                            {
                                inside = !inside;

                            }
                            else if (c == ',')
                            {
                                if (!inside)
                                {
                                    row.Add(word);
                                    word = "";
                                }
                                else
                                {
                                    word += ',';
                                }
                            }
                            else
                            {
                                word += c;
                            }
                        }
                        row.Add(word);
                        lines.Add(row);
                    }
                    else
                    {
                        row = line.Split(',').Select(c => c.Trim()).ToList();
                        lines.Add(row);
                    }
                }
            }
            return lines;
        }
    }
}