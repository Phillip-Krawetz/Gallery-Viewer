using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using MediaPlayer.Domain.Models;
using MediaPlayer.Storing.Repositories;
using ReactiveUI;
using System.Threading;
using MediaPlayer.Domain.Utilities;
using MediaPlayer.Domain.Variables;
using System.Collections.Concurrent;

namespace MediaPlayer.Client.ViewModels
{
  public class DirectoryViewModel : ViewModelBase
  {
    private ObservableCollection<DirectoryItem> items;
    private ObservableCollection<Tag> filters;
    public ObservableCollection<DirectoryItem> Items 
    { 
      get
      {
        if(filters.Count < 1)
        {
          return items;
        }
        return new ObservableCollection<DirectoryItem>(items.Where(x => x.Tags.Any(y => filters.Contains(y))));
      }
      set => this.RaiseAndSetIfChanged(ref items, value); 
    }
    public ObservableCollection<Tag> Filters 
    { 
      get => filters;
      set => this.RaiseAndSetIfChanged(ref filters, value);
    }

    public ObservableCollection<Tag> TagList
    { 
      get => new ObservableCollection<Tag>(TagRepository.Tags);
    }

    private TagRepository tagRepo = new TagRepository();
    private DirectoryRepository directoryRepo = new DirectoryRepository();

    public string CurrentDirectory { get; set; }

    public DirectoryViewModel(){
      Items = new ObservableCollection<DirectoryItem>(directoryRepo.Directories.OrderBy(x => x.FolderPath));
      filters = new ObservableCollection<Tag>();
    }

    public void AddFilter(Tag tag)
    {
      Filters.Add(tag);
      this.RaisePropertyChanged("Items");
    }

    public void RemoveFilter(Tag tag)
    {
      if(Filters.Contains(tag))
      {
        Filters.Remove(tag);
        this.RaisePropertyChanged("Items");
      }
    }

    public void SetDirectoryAsync(string directory){
      if(!Directory.Exists(directory))
      {
        return;
      }
      if(this.items == null)
      {
        this.items = new ObservableCollection<DirectoryItem>();
      }
      var directories = Directory.GetDirectories(directory);
      
      if(directories.Count() > 0 && !Directory.Exists(ImageUtils.ThumbnailCachePath()))
      {
        Directory.CreateDirectory(ImageUtils.ThumbnailCachePath());
      }

      var tasks = new ConcurrentBag<DirectoryItem>();
      Parallel.ForEach(directories, (item) =>
      {
        tasks.Add(CheckDirectory(item));
      });
      Items = new ObservableCollection<DirectoryItem>(tasks.Where(x => x.FolderPath != null).OrderBy(x => x.FolderPath));

      if(Items.Count == 0)
      {
        Items.Add(new DirectoryItem(){
          FolderPath = "None",
          StartPath = "None"
        });
      }
      CurrentDirectory = directory;
    }

    private DirectoryItem CheckDirectory(string item)
    {
      System.Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
      var temp = directoryRepo.GetOrNew(item);
      return temp;
    }

    public void AddTag(DirectoryItem item, string tag)
    {
      if(tag != ""){
        if(Items.Contains(item))
        {
          var newTag = tagRepo.GetOrNew(tag);
          directoryRepo.AddTag(ref item, newTag);
        }
      }
    }

    public async void ChangeDirectory()
    {
      var select = new OpenFolderDialog();
      select.Directory = CurrentDirectory;
      var choice = await select.ShowAsync(new Window());
      SetDirectoryAsync(choice);
    }
  }
}