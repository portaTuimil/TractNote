using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using TractNote.ViewModels;

namespace TractNote;

public partial class MainMenuView : UserControl
{
    private ContentControl maincontent;

    public MainMenuView(ContentControl MainContent)
    {
        InitializeComponent();
        this.DataContext = new MainWindowViewModel();
        maincontent = MainContent;
    }


    //Handles the clicking in the Add button. Calls AddFileToSavedAdresses and adds the selected file.
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
            if (this.DataContext is MainWindowViewModel vm)
            {
                vm.AddFileToSavedAdresses(files[0].Path.LocalPath);
            }
        }
    }


    //Opens db views in the window.
    public void RenderDb(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn)
        {
            string fullPath = btn.Tag?.ToString() ?? "";
            maincontent.Content = new DbView(fullPath, maincontent);
        }
    }
}