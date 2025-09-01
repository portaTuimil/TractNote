using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TractNote.ViewModels;

namespace TractNote.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = new MainWindowViewModel();
    }

    public async void SaveFileButton_Clicked(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Text File",
            AllowMultiple = false
        });

        if (files.Count >= 1)
        {
            await using var stream = await files[0].OpenReadAsync();
            using var reader = new StreamReader(stream);
            var fileContent = await reader.ReadToEndAsync();
        }
        if (files.Count >= 1)
        {
            var pickedFile = files[0];
            string fileName = pickedFile.Name;
            string? fullPath = pickedFile.Path?.LocalPath;
            Debug.WriteLine($"Selected file: {fileName}");
            Debug.WriteLine($"Full path: {fullPath}");
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

   
}
