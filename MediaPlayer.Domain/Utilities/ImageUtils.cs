using System;
using System.Drawing;
using System.IO;

namespace MediaPlayer.Domain.Utilities
{
  public class ImageUtils
  {

    public static string ThumbnailCachePath()
    {
      return Path.GetFullPath(AppContext.BaseDirectory) + "Cache\\Thumbnails\\";
    }

    public static string ThumbnailCachePath(string filename)
    {
      return Path.GetFullPath(AppContext.BaseDirectory) + "Cache\\Thumbnails\\" + GetName(filename) + ".jpeg";
    }

    public static string GetName(string filename)
    {
      if(filename.Contains("\\"))
      {
        return filename.Substring(filename.LastIndexOf('\\') + 1);
      }
      return filename;
    }

    public static Size NewSize(Bitmap original, int maxSize)
    {
      if(original.Width < maxSize && original.Height < maxSize)
      {
        return original.Size;
      }
      var newHeight = 0;
      var newWidth = 0;
      var ratio = 0;
      if(original.Width > original.Height)
      {
        newWidth = maxSize;
        ratio = original.Width/maxSize;
        newHeight = original.Height / ratio;
      } else
      {
        newHeight = maxSize;
        ratio = original.Height / maxSize;
        newWidth = original.Width / ratio;
      }
      return new Size(newWidth, newHeight);
    }
  }
}