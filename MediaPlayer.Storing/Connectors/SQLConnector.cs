using System.Collections.Generic;
using System.Linq;
using MediaPlayer.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaPlayer.Storing.Connectors
{
  public class SqlConnector
  {
    public void AddItem<T>(object item) where T : class
    {
      using(var db = new MediaPlayerDbContext())
      {
        db.Add(item as T);
        db.SaveChanges();
      }
    }

    public void UpdateItem<T>(object item) where T : class
    {
      using(var db = new MediaPlayerDbContext())
      {
        db.Update(item as T);
        db.SaveChanges();
      }
    }

    public void RemoveItem<T>(object item) where T : class
    {
      using(var db = new MediaPlayerDbContext())
      {
        db.Remove(item as T);
        db.SaveChanges();
      }
    }

    public List<T> GetTable<T>() where T : class
    {
      using(var db = new MediaPlayerDbContext())
      {
        return db.Set<T>().ToList();
      }
    }
  }
}