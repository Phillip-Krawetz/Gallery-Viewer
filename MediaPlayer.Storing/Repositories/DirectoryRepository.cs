using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using MediaPlayer.Domain.Models;
using MediaPlayer.Domain.Utilities;
using MediaPlayer.Domain.Variables;
using MediaPlayer.Storing.Connectors;

namespace MediaPlayer.Storing.Repositories
{
  public class DirectoryRepository
  {
    private SqlConnector connector = new SqlConnector();
    private static List<DirectoryItem> directories;
    public List<DirectoryItem> Directories { get => directories; }

    public DirectoryItem GetOrNew(string path)
    {
      if(DirectoryExists(path))
      {
        return directories.First(x => x.FolderPath == path);
      }
      var d = new DirectoryItem{FolderPath = path};
      connector.AddItem<DirectoryItem>(d);
      return d;
    }

    private bool DirectoryExists(string path)
    {
      if(directories.Any(x => x.FolderPath == path))
      {
        return true;
      }
      return false;
    }

    public void AddTag(ref DirectoryItem item, Tag tag)
    {
      if(!item.Tags.Any(x => x.Id == tag.Id))
      {
        var dT = new DirectoryTag
        {
          TagId = tag.Id,
          DirectoryItemId = item.Id,
        };
        item.Tags.Add(tag);
        connector.AddItem<DirectoryTag>(dT);
      }
    }

    private void MapTags()
    {
      var tags = connector.GetTable<DirectoryTag>();
      foreach(var item in tags)
      {
        var a = directories.Where(x => x.Id == item.DirectoryItemId);
        foreach(var subitem in a)
        {
          subitem.Tags.Add(TagRepository.Tags.First(x => x.Id == item.TagId));
        }
      }
    }

    private void GetThumbs()
    {
      foreach(var item in directories)
      {
        if(item.ThumbPath == null && item.StartPath != null)
        {
         item.ThumbPath = CreateThumb(item.Name, item.StartPath);
        }
      }
    }

    private Bitmap CreateThumb(string name, string path)
    {
      if(!File.Exists(ImageUtils.ThumbnailCachePath(name)))
      {
        var bitmap = new System.Drawing.Bitmap(path);
        var newSize = ImageUtils.NewSize(bitmap, 100);
        var newBitmap = new System.Drawing.Bitmap(bitmap, newSize);
        newBitmap.Save(ImageUtils.ThumbnailCachePath(name), System.Drawing.Imaging.ImageFormat.Jpeg);
      }
      return new Bitmap(ImageUtils.ThumbnailCachePath(name));
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
          temp = GetOrNew(item);
          temp.StartPath = myFiles[0];

          temp.ThumbPath = CreateThumb(temp.Name, temp.StartPath);
        }
      }
      catch(UnauthorizedAccessException)
      {
      }
      return Task.FromResult(temp);
    }

    public DirectoryRepository()
    {
      directories = connector.GetTable<DirectoryItem>();
      MapTags();
      GetThumbs();
    }
  }
}