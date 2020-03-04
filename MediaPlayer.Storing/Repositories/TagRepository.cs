using System.Collections.Generic;
using MediaPlayer.Domain.Models;
using System.Linq;
using MediaPlayer.Storing.Connectors;
using System.Collections.Concurrent;

namespace MediaPlayer.Storing.Repositories
{
  public class TagRepository
  {
    private static SqlConnector connector = new SqlConnector();

    private CategoryRepository categoryRepository = new CategoryRepository();

    private static ConcurrentBag<Tag> tags;

    public static List<Tag> Tags 
    { 
      get => tags.ToList();
    }

    public TagRepository()
    {
      Initialize();
    }

    public static void Initialize()
    {
      if(tags == null)
      {
        tags = new ConcurrentBag<Tag>(connector.GetTable<Tag>());
        MapCategories();
      }
    }

    public static void MapCategories()
    {
      var cats = connector.GetTable<Category>();
      foreach(var item in cats)
      {
        var a = tags.Where(x => x.CategoryId == item.Id);
        foreach(var subitem in a)
        {
          subitem.Category = item;
        }
      }
    }

    public void AddTag(string tag)
    {
      if(!TagExists(tag) && tag != "")
      {
        tags.Add(new Tag{Name = tag, Category = categoryRepository.Default});
      }
    }

    public Tag GetOrNew(string tag)
    {
      if(TagExists(tag))
      {
        return tags.First(x => x.Name == tag);
      }
      var t = new Tag{Name = tag, Category = categoryRepository.Default};
      tags.Add(t);
      connector.UpdateItem<Tag>(t);
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