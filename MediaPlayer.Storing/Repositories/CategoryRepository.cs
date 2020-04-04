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

    private static ConcurrentBag<Category> categories;

    public static List<Category> Categories 
    { 
      get => categories.ToList();
    }

    public Category Default 
    { 
      get
      {
        if(categories.Count < 1)
        {
          categories.Add(new Category());
        }
        return categories.ElementAtOrDefault(0);
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
        categories = new ConcurrentBag<Category>(connector.GetTable<Category>());
      }
    }

    public void AddCategory(string category)
    {
      
    }

    public Category GetOrNew(string category)
    {
      if(CategoryExists(category))
      {
        return categories.First(x => x.Name == category);
      }
      var c = new Category{Name = category};
      categories.Add(c);
      connector.AddItem<Category>(c);
      return c;
    }

    public bool CategoryExists(string category)
    {
      if(categories.Any(x => x.Name == category))
      {
        return true;
      }
      return false;
    }
  }
}