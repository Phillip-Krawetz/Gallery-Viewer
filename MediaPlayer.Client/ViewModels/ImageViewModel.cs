using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using MediaPlayer.Client.Abstracts;
using MediaPlayer.Domain.Models;
using MediaPlayer.Domain.Variables;
using MediaPlayer.Storing.Repositories;
using ReactiveUI;

namespace MediaPlayer.Client.ViewModels
{
  public class ImageViewModel : AbstractViewModelBaseWithTags
  {
    string imagePath;
    public string ImagePath { 
      get => imagePath;
      set => this.RaiseAndSetIfChanged(ref imagePath, value);
    }

    //private DirectoryItem GalleryDirectory;

    public ObservableCollection<Tag> Tags 
    { 
      get => new ObservableCollection<Tag>(SelectedDirectory.Tags);
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

    public ImageViewModel(DirectoryItem GalleryDirectory)
    {
      this.SelectedDirectory = GalleryDirectory;
      this.ImagePath = GalleryDirectory.StartPath;
      Initialize();
    }

    public ImageViewModel(string ImagePath){
      this.ImagePath = ImagePath;
      Initialize();
    }

    private void Initialize()
    {
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

      var continueExecution = IterateFiles(dir);

      currentIndex = 0;

      System.Console.WriteLine("Opened:" + dir);
    }

    public async Task IterateFiles(string dir)
    {
      var LoadTasks = new List<Task>();
      foreach(var item in Directory.EnumerateFiles(dir, "*.*", SearchOption.TopDirectoryOnly))
      {
        if(FileTypes.ValidImageTypes.Contains(Path.GetExtension(item).ToLowerInvariant()))
        {
          myFiles.Add(item);
          if(Options.Preload)
          {
            Images.TryAdd(myFiles.IndexOf(item), default(Bitmap));
            LoadTasks.Add(LoadBitmap(myFiles.IndexOf(item),item));
          }
        }
      }
      this.RaisePropertyChanged("LastPage");
      if(LoadTasks.Count > 0)
      {
        await Task.WhenAny(LoadTasks);
      }
    }

    private async Task LoadBitmap(int index, string path)
    {
      await Task.Run(() => Images[index] = new Bitmap(path));
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

    public void AddTag(DirectoryItem item, string tag)
    {
      if(!String.IsNullOrWhiteSpace(tag)){
        var newTag = tagRepo.GetOrNew(tag);
        directoryRepo.AddTag(item, newTag);
        this.RaisePropertyChanged("Tags");
      }
    }

    public void RemoveTag(DirectoryItem item, Tag tag)
    {
      directoryRepo.RemoveTag(item, tag);
      this.RaisePropertyChanged("Tags");
    }
  }
}