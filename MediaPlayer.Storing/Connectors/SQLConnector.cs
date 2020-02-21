using System.Collections.Generic;
using System.Linq;
using MediaPlayer.Domain.Models;

namespace MediaPlayer.Storing.Connectors
{
  public class SqlConnector
  {
    private static MediaPlayerDbContext db = new MediaPlayerDbContext();

    public SqlConnector()
    {
      db = new MediaPlayerDbContext();
    }
    public void AddItem<T>(object item) where T : class
    {
      db.Add(item as T);
      db.SaveChanges();
    }

    public void UpdateItem<T>(object item) where T : class
    {
      db.Update(item as T);
      db.SaveChanges();
    }

    public List<T> GetTable<T>() where T : class
    {
      return db.Set<T>().ToList();
    }
  }
}