using System.Collections.Generic;
using MediaPlayer.Domain.Models;
using System.Linq;
using MediaPlayer.Storing.Connectors;
using System.Collections.Concurrent;
using System.Collections.Specialized;

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
      var cats = CategoryRepository.Categories;
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

    public Tag GetOrNew(string tag, string category = null)
    {
      if(TagExists(tag))
      {
        return tags.First(x => x.Name == tag);
      }
      var t = new Tag{Name = tag, Category = categoryRepository.GetOrNew(category) ?? categoryRepository.Default};
      tags.Add(t);
      connector.UpdateItem<Tag>(t);
      return t;
    }

    public bool TryGetTag(string tagName, ref Tag tag)
    {
      if(TagExists(tagName))
      {
        tag = tags.First(x => x.Name == tagName);
        return true;
      }
      return false;
    }

    public void UpdateTag(Tag tag)
    {
      if(TagExists(tag.Id))
      {
        var temp = tags.First(x => x.Id == tag.Id);
        temp.Name = tag.Name;
        temp.Category = tag.Category;
        temp.CategoryId = tag.CategoryId;
        connector.UpdateItem<Tag>(temp);
      }
    }

    private bool TagExists(string tag)
    {
      if(tags.Any(x => x.Name == tag))
      {
        return true;
      }
      return false;
    }

    private bool TagExists(int id)
    {
      if(tags.Any(x => x.Id == id))
      {
        return true;
      }
      return false;
    }
  }
}