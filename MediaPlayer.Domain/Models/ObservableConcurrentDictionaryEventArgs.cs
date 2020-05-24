using System.ComponentModel;

namespace MediaPlayer.Domain.Models
{
  public class ObservableConcurrentDictionaryEventArgs : PropertyChangedEventArgs
  {
    public ObservableConcurrentDictionaryEventArgs(string propertyName) : base(propertyName)
    {
    }

    public override string PropertyName => base.PropertyName;

    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public override string ToString()
    {
      return base.ToString();
    }

    public object OldValue;
    public object NewValue;
    public object Key;
  }
}