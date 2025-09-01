using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TractNote.ViewModels;

//A class responsible of managing the Settings/SavedAdresses.csv
public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
{
    public ObservableCollection<string>? FileTitles { get; private set; }
    private readonly string Path = System.IO.Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "TractNote", "Settings", "SavedAdresses.csv");

    public MainWindowViewModel() 
    {
        GetSavedAdresses();
    }

    public void GetSavedAdresses()
    {
        string readText = File.ReadAllText(Path);
        string[] values = readText.Split(',');
        FileTitles = new ObservableCollection<string>(values);
        OnPropertyChanged(nameof(FileTitles));
    }


    //INotifyPropertyChanged Implementation
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

