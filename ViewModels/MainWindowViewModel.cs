using Avalonia.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TractNote.ViewModels;

//A class responsible of managing the Settings/SavedAdresses.csv
public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
{
    public ObservableCollection<FileEntry>? Files { get; private set; }

    private readonly string Path = System.IO.Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "TractNote", "Settings", "SavedAdresses.csv");

    public MainWindowViewModel() 
    {
        GetSavedAdresses();
    }


    //Retrieves the content of SavedAdresses. Should be called eveytime that file is modified.
    public void GetSavedAdresses()
    {
        try
        {
            string readText = File.ReadAllText(Path);
            if (!string.IsNullOrWhiteSpace(readText))
            {
                string[] values = readText.Split(',');

                Files = new ObservableCollection<FileEntry>(
                    values.Select(v => new FileEntry(v.Trim('"')))
                );
                OnPropertyChanged(nameof(Files));
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error reading addresses: {ex.Message}");
            Files = new ObservableCollection<FileEntry>();
        }
    }


    //The class that stores fullpaths (to render) and filenames (to display). A collection of them is passed to MainWindow.Axaml through binding.
    public class FileEntry
    {
        public string FullPath { get; }
        public string FileName => FullPath.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries).Last().Trim('"');

        public FileEntry(string fullPath)
        {
            FullPath = fullPath;
        }
    }


    //Gets called in SaveFileButton_Clicked and adds the value passed to SaveAdresses.csv
    public void AddFileToSavedAdresses(string filePath)
    {
        if (!File.Exists(Path) || string.IsNullOrWhiteSpace(File.ReadAllText(Path)))
        {
            File.WriteAllText(Path, filePath);
            GetSavedAdresses();
        }
        else
        {
            var lines = File.ReadAllLines(Path).ToList();
            lines[0] = lines[0] + ", " + filePath;
            File.WriteAllLines(Path, lines);
            GetSavedAdresses();
        }
            
    }


    //INotifyPropertyChanged Implementation
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

