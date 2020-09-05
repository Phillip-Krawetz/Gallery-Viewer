using System;
using System.IO;
using MediaPlayer.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace MediaPlayer.Storing
{
  public class MediaPlayerDbContext : DbContext
  {
    public DbSet<Tag> Tags { get; set; }
    public DbSet<DirectoryItem> Directories { get; set; }
    public DbSet<DirectoryTag> DirectoryTags { get; set; }
    public DbSet<Category> Categories { get; set; }
    private static string directoryPath = Path.GetFullPath(AppContext.BaseDirectory) + "Cache\\";
    private static string dbPath = directoryPath + "MediaPlayer.db";

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
      options.UseSqlite("Data Source="+dbPath);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      builder.Entity<DirectoryTag>().HasKey(x => new {x.DirectoryItemId, x.TagId});
      builder.Entity<Category>().HasData(
        new Category(){Id = 1}
      );
    }

    public MediaPlayerDbContext()
    {
      if(!Directory.Exists(directoryPath))
      {
        Directory.CreateDirectory(directoryPath);
      }
      Database.EnsureCreated();
    }
  }
}