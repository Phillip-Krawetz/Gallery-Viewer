using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using MediaPlayer.Domain.Abstracts;

namespace MediaPlayer.Domain.Models
{
  public class DirectoryItem : AbstractObjectWithID
  {
    public string FolderPath { get; set; }
    [NotMapped]
    public Bitmap Thumb { get; set; }
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
        TextWrapping = TextWrapping.Wrap
      };
    }
    private ObservableCollection<Tag> tags;
    [NotMapped]
    public ObservableCollection<Tag> Tags 
    { 
      get => tags;
      set => tags = value;
    }

    [NotMapped]
    private ObservableCollection<Tag> parentTags
    {
      get
      {
        var temp = new ObservableCollection<Tag>();
        foreach(var tag in Tags)
        {
          var iterativeTag = tag;
          while(iterativeTag.ParentTag != null && !Tags.Contains(iterativeTag.ParentTag)){
            temp.Add(iterativeTag.ParentTag);
            iterativeTag = iterativeTag.ParentTag;
          }
        }
        return temp;
      }
    }
    [NotMapped]
    public ObservableCollection<Tag> TagsWithParents
    {
      get => new ObservableCollection<Tag>(Tags.Union(parentTags));
    }

    public void AddTag(Tag tag)
    {
      if(tags.Contains(tag))
      {
        return;
      }
      tags.Add(tag);
      SortTags();
    }

    public void SortTags(bool ForceUpdate = false)
    {
      if(ForceUpdate)
      {
        this.ForceUpdate();
      }
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
      for(int i = 0; i < tags.Count; i++)
      {
        for(int j = 0; j < tags.Count - 1; j++)
        {
          if(tags[j].Category.Name.CompareTo(tags[j + 1].Category.Name) > 0)
          {
            tags.Move(j, j+1);
          }
        }
      }
      for(int i = 0; i < tags.Count; i++)
      {
        for(int j = 0; j < tags.Count - 1; j++)
        {
          if(tags[j].Category.Priority.CompareTo(tags[j + 1].Category.Priority) < 0)
          {
            tags.Move(j, j+1);
          }
        }
      }
    }

    private void ForceUpdate()
    {
      var temp = new Tag();
      tags.Add(temp);
      tags.Remove(temp);
    }

    public DirectoryItem()
    {
      tags = new ObservableCollection<Tag>();
    }
  }
}