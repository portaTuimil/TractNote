using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TractNote.ViewModels;

//A class responsible of managing the Settings/SavedAdresses.csv
public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
{
    public ObservableCollection<string>? FileTitles { get; private set; }

    public MainWindowViewModel() 
    {
        GetSavedAdresses();
    }

    public void GetSavedAdresses()
    {
        FileTitles = ["Terms", "Terms2"];
        OnPropertyChanged(nameof(FileTitles));
    }


    //INotifyPropertyChanged Implementation
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

