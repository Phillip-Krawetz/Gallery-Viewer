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
using MediaPlayer.Client.Abstracts;

namespace MediaPlayer.Client.ViewModels
{
  public class DirectoryViewModel : AbstractViewModelBaseWithTags
  {
    private ObservableCollection<DirectoryItem> items;
    private ObservableCollection<Filter> filters;
    public ObservableCollection<DirectoryItem> Items 
    { 
      get
      {
        if(filters.Count < 1)
        {
          return new ObservableCollection<DirectoryItem>(items.Where(x => x.Name.ToLower().Contains(searchText.ToLower())));
        }
        return new ObservableCollection<DirectoryItem>(
          items.Where(x => 
          {
            x.Name.ToLower().Contains(searchText.ToLower());
            var temp = new HashSet<Tag>(x.TagsWithParents);
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
    
    private string searchtext;
    private string searchText
    { 
      get{
        return searchtext ?? "";
      }
      set => this.searchtext = value;
    }

    public string CurrentDirectory { get; set; }

    public DirectoryViewModel(){
      Items = new ObservableCollection<DirectoryItem>(directoryRepo.Directories.OrderBy(x => x.FolderPath));
      filters = new ObservableCollection<Filter>();
      searchText = "";
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

    public void SearchNames(string searchtext)
    {
      this.searchText = searchtext;
      this.RaisePropertyChanged("Items");
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
      var directories = Directory.GetDirectories(directory).Except(items.Select(y => y.FolderPath));
      
      if(directories.Count() > 0 && !Directory.Exists(ImageUtils.ThumbnailCachePath()))
      {
        Directory.CreateDirectory(ImageUtils.ThumbnailCachePath());
      }

      var tasks = new ConcurrentBag<DirectoryItem>();
      Parallel.ForEach(directories, (item) =>
      {
        tasks.Add(CheckDirectory(item));
      });
      var newItems = new ObservableCollection<DirectoryItem>(tasks.Where(x => x.FolderPath != null).OrderBy(x => x.FolderPath));

      CurrentDirectory = directory;
      Items = new ObservableCollection<DirectoryItem>(Items.Union(newItems));

      if(Items.Count == 0)
      {
        Items.Add(new DirectoryItem(){
          FolderPath = "None",
          StartPath = "None"
        });
      }
    }

    private DirectoryItem CheckDirectory(string item)
    {
      System.Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
      var temp = directoryRepo.GetOrNew(item);
      return temp;
    }

    public override void AddTag(DirectoryItem item, string tag)
    {
      if(!String.IsNullOrWhiteSpace(tag)){
        if(Items.Contains(item))
        {
          var newTag = tagRepo.GetOrNew(tag);
          directoryRepo.AddTag(item, newTag);
          this.RaisePropertyChanged("TagNameList");
        }
      }
    }

    public void RemoveTag(DirectoryItem item, Tag tag)
    {
      directoryRepo.RemoveTag(item, tag);
    }

    public void LoadBackup(DirectoryItem item = null)
    {
      directoryRepo.ReadFromBackups(item);
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