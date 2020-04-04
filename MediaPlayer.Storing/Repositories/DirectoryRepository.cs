using System;
using System.Collections.Concurrent;
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
    private static SqlConnector connector = new SqlConnector();
    private static ConcurrentBag<DirectoryItem> directories;
    public List<DirectoryItem> Directories { get => directories.OrderBy(x => x.FolderPath).ToList(); }

    public DirectoryItem GetOrNew(string path)
    {
      if(DirectoryExists(path))
      {
        return directories.First(x => x.FolderPath == path);
      }
      var d = NewDirectory(path);
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
        item.AddTag(tag);
        connector.AddItem<DirectoryTag>(dT);
      }
    }

    public void RemoveTag(DirectoryItem item, Tag tag)
    {
      if(item != null && tag != null)
      {
        item.Tags.Remove(tag);
        connector.RemoveItem<DirectoryTag>(new DirectoryTag(){DirectoryItemId = item.Id, TagId = tag.Id});
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
          subitem.AddTag(TagRepository.Tags.First(x => x.Id == item.TagId));
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

    private DirectoryItem NewDirectory(string path)
    {
      var temp = new DirectoryItem();
      try
      {
        foreach(var file in Directory
          .EnumerateFiles(path, "*.*", SearchOption.TopDirectoryOnly))
        {
          if(FileTypes.ValidImageTypes.Contains(Path.GetExtension(file).ToLowerInvariant()))
          {
            temp = new DirectoryItem(){FolderPath = path};
            temp.StartPath = file;

            temp.ThumbPath = CreateThumb(temp.Name, temp.StartPath);
            directories.Add(temp);
            connector.AddItem<DirectoryItem>(temp);
            break;
          }
        }
      }
      catch(UnauthorizedAccessException){}
      return temp;
    }

    public static void Initialize()
    {
      if(directories == null)
      {
        directories = new ConcurrentBag<DirectoryItem>(connector.GetTable<DirectoryItem>());
      }
    }

    public DirectoryRepository()
    {
      Initialize();
      MapTags();
      GetThumbs();
    }
  }
}