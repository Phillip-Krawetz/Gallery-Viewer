using System;
using System.Collections.Generic;
using System.IO;
using MediaPlayer.Domain.Variables;
using MediaPlayer.Storing.Converters;
using Newtonsoft.Json;

namespace MediaPlayer.Storing.Connectors
{
  public class FileSystemConnector
  {
    private static string configPath = Path.GetFullPath(AppContext.BaseDirectory)+"\\Config.json";
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
  }
}