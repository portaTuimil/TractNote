using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using TractNote.ViewModels;

namespace TractNote.Views;

public partial class MainWindow : Window
{
    private readonly StringBuilder _inputBuffer = new();
    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = new MainWindowViewModel();
        MainContent.Content = new MainMenuView(MainContent);
        this.LostFocus += (_, __) =>
        {
            this.Focus(); // always regain focus
        };
    }

    private void OnTextInput(object? sender, TextInputEventArgs e)
    {
        _inputBuffer.Append(e.Text);
        Debug.WriteLine("Buffer: " + _inputBuffer.ToString());
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Back)
        {
            if(MainContent.Content is UserControl currentControl)
            { 
                string typeName = currentControl.GetType().Name;
                Debug.WriteLine(typeName);
                if (typeName == "DbView")
                {
                    Debug.WriteLine("hi");
                    MainContent.Content = new MainMenuView(MainContent);
                } else if(_inputBuffer.Length > 0)
                {
                    _inputBuffer.Remove(_inputBuffer.Length - 1, 1);
                }
            }
        }
        else if (e.Key == Key.Enter)
        {
            if (_inputBuffer.ToString() == "rm .")
            {
                if (this.DataContext is MainWindowViewModel vm)
                {
                    vm.CleanSavedAdresses();
                    MainContent.Content = new MainMenuView(MainContent);
                }
            }
            _inputBuffer.Clear();
        }
    }
}
