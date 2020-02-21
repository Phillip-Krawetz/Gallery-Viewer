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

namespace MediaPlayer.Client.ViewModels
{
  public class DirectoryViewModel : ViewModelBase
  {
    private ObservableCollection<DirectoryItem> items;
    public ObservableCollection<DirectoryItem> Items 
    { 
      get => items;
      set => this.RaiseAndSetIfChanged(ref items, value); 
    }

    private TagRepository tagRepo = new TagRepository();
    private DirectoryRepository directoryRepo = new DirectoryRepository();

    public string CurrentDirectory { get; set; }

    public DirectoryViewModel(){
      Items = new ObservableCollection<DirectoryItem>(directoryRepo.Directories);
    }

    public async void SetDirectoryAsync(string directory){
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

      var tasks = new List<Task<DirectoryItem>>();
      foreach(var item in directories)
      {
        tasks.Add(CheckDirectory(item));
      }
      Items = new ObservableCollection<DirectoryItem>((await Task.WhenAll<DirectoryItem>(tasks))
        .Where(x => x.FolderPath != null));

      if(Items.Count == 0)
      {
        Items.Add(new DirectoryItem(){
          FolderPath = "None",
          StartPath = "None"
        });
      }
      CurrentDirectory = directory;
    }

    private Task<DirectoryItem> CheckDirectory(string item)
    {
      System.Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
      var temp = new DirectoryItem();
      try
      {
        var myFiles = Directory
          .EnumerateFiles(item, "*.*", SearchOption.TopDirectoryOnly)
          .Where(s => FileTypes.ValidImageTypes.Contains(Path.GetExtension(s).ToLowerInvariant())).ToList();
        if(myFiles.Count() > 0)
        {
          temp = directoryRepo.GetOrNew(item);
          if(!File.Exists(ImageUtils.ThumbnailCachePath(temp.Name)))
          {
            var bitmap = new System.Drawing.Bitmap(myFiles[0]);
            var newSize = ImageUtils.NewSize(bitmap, 100);
            var newBitmap = new System.Drawing.Bitmap(bitmap, newSize);
            newBitmap.Save(ImageUtils.ThumbnailCachePath(temp.Name), System.Drawing.Imaging.ImageFormat.Jpeg);
          }

          temp.ThumbPath = new Bitmap(ImageUtils.ThumbnailCachePath(temp.Name));
          temp.StartPath = myFiles[0];
        }
      }
      catch(UnauthorizedAccessException)
      {
      }
      return Task.FromResult(temp);
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