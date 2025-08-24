using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Collections.Generic;

namespace TractNote.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        File file = new File();
        for (int i = 0; i < file.Categories.Count; i++)
        {
            Button button = new Button
            {
                Content = file.Categories[i]
            };
            button.Click += RenderDb;
            recentFiles.Children.Add(button);
        }
    }

    public void RenderDb(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn)
        {
            string contentText = btn.Content?.ToString() ?? "";
            MainContent.Content = new DbView(contentText);
        }
    }

    public class File
    {
        public List<string>? Categories { get; private set; }

        public File()
        {
            Categories = ["Terms", "Terms2"];
        }
    }
}
