using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using MediaPlayer.Domain.Models;
using MediaPlayer.Storing.Repositories;
using ReactiveUI;
using System.Threading;
using MediaPlayer.Domain.Utilities;
using System.Collections.Concurrent;
using MediaPlayer.Client.Models;

namespace MediaPlayer.Client.ViewModels
{
  public class DirectoryViewModel : ViewModelBase
  {
    private ObservableCollection<DirectoryItem> items;
    private ObservableCollection<Filter> filters;
    public ObservableCollection<DirectoryItem> Items 
    { 
      get
      {
        if(filters.Count < 1)
        {
          return items;
        }
        var temp = new HashSet<Tag>();
        return new ObservableCollection<DirectoryItem>(
          items.Where(x => 
          {
            var temp = new HashSet<Tag>(x.Tags);
            var inclusions = Filters.Where(x => !x.Exclude).Select(x => x.Tag);
            if(temp.IsSupersetOf(inclusions))
            {
              if(inclusions.Count() != Filters.Count())
              {
                if(!temp.Intersect(Filters.Where(x => x.Exclude).Select(x => x.Tag)).Any()){
                  return true;
                }
              }
              else
              {
                return true;
              }
            }
            return false;
          }));
      }
      set => this.RaiseAndSetIfChanged(ref items, value); 
    }
    public ObservableCollection<Filter> Filters 
    { 
      get => filters;
      set => this.RaiseAndSetIfChanged(ref filters, value);
    }

    public ObservableCollection<Tag> TagList
    { 
      get => new ObservableCollection<Tag>(TagRepository.Tags);
    }

    public List<string> TagNameList
    {
      get => TagList.Select(x => x.Name).ToList();
    }

    private TagRepository tagRepo = new TagRepository();
    private DirectoryRepository directoryRepo = new DirectoryRepository();

    public string CurrentDirectory { get; set; }

    public DirectoryViewModel(){
      Items = new ObservableCollection<DirectoryItem>(directoryRepo.Directories.OrderBy(x => x.FolderPath));
      filters = new ObservableCollection<Filter>();
    }

    public void AddFilter(Tag tag, bool exclude = false)
    {
      Filters.Add(new Filter(tag, exclude));
      this.RaisePropertyChanged("Items");
    }

    public void ManualFilter(string tag, bool exclude)
    {
      var temp = new Tag();
      if(tagRepo.TryGetTag(tag, ref temp))
      {
        AddFilter(temp, exclude);
      }
    }

    public void RemoveFilter(Filter filter)
    {
      if(Filters.Contains(filter))
      {
        Filters.Remove(filter);
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
      if(!String.IsNullOrWhiteSpace(tag)){
        if(Items.Contains(item))
        {
          var newTag = tagRepo.GetOrNew(tag);
          directoryRepo.AddTag(ref item, newTag);
          this.RaisePropertyChanged("TagNameList");
        }
      }
    }

    public void RemoveTag(DirectoryItem item, Tag tag)
    {
      directoryRepo.RemoveTag(item, tag);
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