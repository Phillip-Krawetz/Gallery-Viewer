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
    private ObservableCollection<Tag> tags;
    [NotMapped]
    public ObservableCollection<Tag> Tags 
    { 
      get => tags;
      set => tags = value;
    }

    public void AddTag(Tag tag)
    {
      tags.Add(tag);
      for(int i = 0; i < tags.Count; i++)
      {
        for(int j = 0; j < tags.Count - 1; j++)
        {
          if(tags[j].Name.CompareTo(tags[j + 1].Name) > 0)
          {
            tags.Move(j, j+1);
          }
        }
      }
    }

    public DirectoryItem()
    {
      tags = new ObservableCollection<Tag>();
    }
  }
}