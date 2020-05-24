using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using MediaPlayer.Domain.Models;
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

    public List<int> FileIndexes
    { 
      get => Enumerable.Range(1, myFiles.Count).ToList();
    }

    private ObservableConcurrentDictionary<int, Bitmap> images;
    public ObservableConcurrentDictionary<int, Bitmap> Images
    {
      get => images;
      set => this.RaiseAndSetIfChanged(ref images, value);
    }

    public Bitmap CurrentImage
    {
      get => Images[currentIndex];
    }

    private int _index;

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

      if(Options.Preload)
      {
        this.Images = new ObservableConcurrentDictionary<int, Bitmap>();
        Images.ValueChanged += delegate(object sender, System.ComponentModel.PropertyChangedEventArgs args)
        {
          var temp = args as ObservableConcurrentDictionaryEventArgs;
          if((int)temp.Key == currentIndex && temp.OldValue == null)
          {
            this.RaisePropertyChanged("CurrentImage");
          }
        }; 
      }

      var dir = ImagePath.Substring(0, ImagePath.Length - ImagePath.Substring(ImagePath.LastIndexOf('\\')).Length + 1);

      IterateFiles(dir);

      currentIndex = 0;

      System.Console.WriteLine("Opened:" + dir);
    }

    public void IterateFiles(string dir)
    {
      foreach(var item in Directory.EnumerateFiles(dir, "*.*", SearchOption.TopDirectoryOnly))
      {
        if(FileTypes.ValidImageTypes.Contains(Path.GetExtension(item).ToLowerInvariant()))
        {
          myFiles.Add(item);
          if(Options.Preload)
          {
            Images.TryAdd(myFiles.IndexOf(item), default(Bitmap));
            new Thread(() =>
            {
              Thread.CurrentThread.IsBackground = true;
              Images[myFiles.IndexOf(item)] = new Bitmap(item);
            }).Start();
          }
        }
      }
      this.RaisePropertyChanged("LastPage");
    }

    public void JumpToPage(int page)
    {
      if(page >= 0 && page <= myFiles.Count)
      {
        currentIndex = page - 1;
        this.ImagePath = myFiles[page - 1];
        this.RaisePropertyChanged("CurrentImage");
      }
    }


    public override void Prev()
    {
      if(currentIndex > 0)
      {
        currentIndex--;
      }
      this.ImagePath = myFiles[currentIndex];
      this.RaisePropertyChanged("CurrentImage");
    }

    public override void Next()
    {
      if(currentIndex < myFiles.Count - 1)
      {
        currentIndex++;
      }
      this.ImagePath = myFiles[currentIndex];
      this.RaisePropertyChanged("CurrentImage");
    }
  }
}