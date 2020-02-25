using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace MediaPlayer.Domain.Models
{
  public class DirectoryItem
  {
    public int Id { get; set; }
    public string FolderPath { get; set; }
    [NotMapped]
    public Bitmap ThumbPath { get; set; }
    public string StartPath { get; set; }
    public string Name 
    { 
      get => FolderPath.Contains("\\")? FolderPath.Substring(FolderPath.LastIndexOf('\\') + 1) : FolderPath;
    }
    public FormattedText FormattedName 
    { 
      get => new FormattedText()
      {
        Text = Name,
        TextAlignment = TextAlignment.Center,
        Wrapping = TextWrapping.Wrap
      };
    }
    [NotMapped]
    public ObservableCollection<Tag> Tags 
    { 
      get;
      set; 
    }

    public DirectoryItem()
    {
      Tags = new ObservableCollection<Tag>();
    }
  }
}