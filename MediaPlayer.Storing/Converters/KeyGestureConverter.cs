using System;
using Avalonia.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MediaPlayer.Storing.Converters
{
  public class KeyGestureConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return (objectType == typeof(KeyGesture));
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      JObject jo = JObject.Load(reader);
      string key = (string)jo["Key"];
      Key realKey = (Key)Enum.Parse(typeof(Key),key);
      string modifiers = (string)jo["Modifiers"];
      string keymodifiers = (string)jo["KeyModifiers"];
      KeyModifiers realKeyMods = (KeyModifiers)Enum.Parse(typeof(KeyModifiers),keymodifiers);
      return new KeyGesture(realKey, realKeyMods);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }
  }
}