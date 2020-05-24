using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MediaPlayer.Domain.Models;
using MediaPlayer.Storing.Connectors;

namespace MediaPlayer.Storing.Repositories
{
  public class CategoryRepository
  {
    private static SqlConnector connector = new SqlConnector();

    private static ConcurrentDictionary<int,Category> categories;

    public static List<Category> Categories 
    { 
      get => categories.Values.OrderBy(x => x.Id).ToList();
    }

    public Category Default 
    { 
      get
      {
        if(categories.Count < 1)
        {
          var cat = new Category();
          categories.TryAdd(cat.Id, cat);
        }
        return categories.ElementAtOrDefault(0).Value;
      }
    }

    public CategoryRepository()
    {
      Initialize();
    }

    public static void Initialize()
    {
      if(categories == null)
      {
        categories = new ConcurrentDictionary<int, Category>();
        foreach(var item in connector.GetTable<Category>())
        {
          categories.TryAdd(item.Id, item);
        }
      }
    }

    public void AddCategory(string category)
    {
      
    }

    public Category GetOrNew(string category)
    {
      if(CategoryExists(category))
      {
        return categories.First(x => x.Value.Name == category).Value;
      }
      var c = new Category{Name = category};
      categories.TryAdd(c.Id, c);
      connector.AddItem<Category>(c);
      return c;
    }

    public void UpdateOrAdd(Category category)
    {
      if(CategoryExists(category.Id))
      {
        var cat = categories.FirstOrDefault(x => x.Value.Id == category.Id).Value;
        cat.B = category.B;
        cat.G = category.G;
        cat.R = category.R;
        connector.UpdateItem<Category>(cat);
      }
      else
      {
        category.Id = connector.AddItem<Category>(category);
        categories.TryAdd(category.Id, category);
      }
    }

    public void RemoveCategory(Category category)
    {
      if(categories.Values.Contains(category))
      {
        var tagRepo = new TagRepository();
        foreach(var item in TagRepository.Tags.Where(x => x.CategoryId == category.Id))
        {
          item.Category = categories[1] ?? categories.OrderBy(kvp => kvp.Key).First().Value;
          item.CategoryId = item.Category.Id;
          tagRepo.UpdateTag(item);
        }
        category.Tags = null;
        connector.RemoveItem<Category>(category);
        categories.Remove(category.Id, out category);
      }
    }

    public bool CategoryExists(string category)
    {
      if(categories.Any(x => x.Value.Name == category))
      {
        return true;
      }
      return false;
    }

    public bool CategoryExists(int id)
    {
      if(categories.Any(x => x.Value.Id == id))
      {
        return true;
      }
      return false;
    }
  }
}