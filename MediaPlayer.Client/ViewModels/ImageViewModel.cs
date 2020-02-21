using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using MediaPlayer.Domain.Variables;
using ReactiveUI;

namespace MediaPlayer.Client.ViewModels
{
  public class ImageViewModel : ViewModelBase
  {
    string imagePath;
    public string ImagePath { 
      get => imagePath;
      set => this.RaiseAndSetIfChanged(ref imagePath, value);
    }

    private List<string> myFiles = new List<string>();

    int _index;

    public int currentIndex 
    {
      get => _index;
      set => this.RaiseAndSetIfChanged(ref _index, value);
    }

    public string LastPage 
    { 
      get => myFiles.Count.ToString(); 
    }

    public ImageViewModel(string ImagePath){
      this.ImagePath = ImagePath;      

      var dir = ImagePath.Substring(0, ImagePath.Length - ImagePath.Substring(ImagePath.LastIndexOf('\\')).Length + 1);
      
      myFiles = Directory
      .EnumerateFiles(dir, "*.*", SearchOption.AllDirectories)
      .Where(s => FileTypes.ValidImageTypes.Contains(Path.GetExtension(s).ToLowerInvariant())).ToList();

      currentIndex = myFiles.FindIndex(x => x == ImagePath);

      System.Console.WriteLine(dir);
    }


    public override void Prev()
    {
      if(currentIndex > 0)
      {
        currentIndex--;
      }
      this.ImagePath = myFiles[currentIndex];
    }

    public override void Next()
    {
      if(currentIndex < myFiles.Count - 1)
      {
        currentIndex++;
      }
      this.ImagePath = myFiles[currentIndex];
    }

    public void Back()
    {}

  }
}