using System;
using System.Collections.Generic;
using System.IO;
using MediaPlayer.Domain.Models;
using MediaPlayer.Domain.Variables;
using MediaPlayer.Storing.Converters;
using Newtonsoft.Json;

namespace MediaPlayer.Storing.Connectors
{
  public class FileSystemConnector
  {
    private static string configPath = Path.GetFullPath(AppContext.BaseDirectory)+"\\Options.json";
    public static bool EnsureCreated()
    {
      if(!File.Exists(configPath))
      {
        InitializeSettings();
        return false;
      }
      SetOptions();
      return true;
    }

    private static void InitializeSettings()
    {
      var serializer = new JsonSerializer();
      var opt = new InitialOptions();
      opt.SetDefaults();
      opt.MapToOptions();
      using(StreamWriter sw = new StreamWriter(configPath))
      using(JsonWriter writer = new JsonTextWriter(sw))
      {
        serializer.Serialize(writer, opt);
      }
    }

    public static void WriteOptions()
    {
      var serializer = new JsonSerializer();
      var opt = new InitialOptions();
      opt.CopyCurrent();
      using(StreamWriter sw = new StreamWriter(configPath))
      using(JsonWriter writer = new JsonTextWriter(sw))
      {
        serializer.Serialize(writer, opt);
      }
    }

    private static void SetOptions()
    {
      var settings = new JsonSerializerSettings();
      settings.Converters.Add(new KeyGestureConverter());
      var serializer = JsonSerializer.Create(settings);
      var opt = new InitialOptions();
      using(StreamReader sr = new StreamReader(configPath))
      {
        opt = (InitialOptions)serializer.Deserialize(sr, typeof(InitialOptions));
        opt.MapToOptions();
      }
    }

    public List<T> OpenFile<T>(string path) where T : class
    {
      var temp = new List<T>();
      JsonConvert.DeserializeObject<T>(path);
      return temp;
    }

    public void UpdateTags(DirectoryItem item)
    {
      using(StreamWriter sw = new StreamWriter(item.FolderPath + "\\tags.txt"))
      {
        foreach(var tag in item.Tags)
        {
          var ParentTag = "";
          if(tag.ParentTag != null)
          {
            ParentTag = tag.ParentTag.Name;
          }
          sw.WriteLine(tag.Category.Name + ":" + tag.Name + ":" + ParentTag);
        }
      }
    }

    public Dictionary<string,List<(string, string)>> ReadTags(string path)
    {
      path += "\\tags.txt";
      var tagList = new Dictionary<string,List<(string, string)>>();
      var temp = "";
      if(!File.Exists(path))
      {
        return tagList;
      }
      using(StreamReader sr = new StreamReader(path))
      {
        while(!sr.EndOfStream){
          temp = sr.ReadLine();
          var split = temp.Split(':');
          var split2 = "";
          if(split.Length > 2)
          {
            split2 = split[2];
          }
          if(tagList.ContainsKey(split[0]))
          {
            tagList[split[0]].Add((split[1], split2));
          }
          else
          {
            tagList.TryAdd(split[0],new List<(string, string)>(){(split[1], split2)});
          }
        }
      }
      return tagList;
    }
  }
}