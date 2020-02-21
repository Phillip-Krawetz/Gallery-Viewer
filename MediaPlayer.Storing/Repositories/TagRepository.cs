using System.Collections.Generic;
using MediaPlayer.Domain.Models;
using System.Linq;
using MediaPlayer.Storing.Connectors;

namespace MediaPlayer.Storing.Repositories
{
  public class TagRepository
  {
    private SqlConnector connector = new SqlConnector();

    private static List<Tag> tags;

    public static List<Tag> Tags 
    { 
      get => tags;
    }

    public TagRepository()
    {
      tags = connector.GetTable<Tag>();
    }

    public void AddTag(string tag)
    {
      if(!TagExists(tag) && tag != "")
      {
        tags.Add(new Tag{Name = tag});
      }
    }

    public Tag GetOrNew(string tag)
    {
      if(TagExists(tag))
      {
        return tags.First(x => x.Name == tag);
      }
      var t = new Tag{Name = tag};
      tags.Add(t);
      connector.AddItem<Tag>(t);
      return t;
    }

    private bool TagExists(string tag)
    {
      if(tags.Any(x => x.Name == tag))
      {
        return true;
      }
      return false;
    }
  }
}