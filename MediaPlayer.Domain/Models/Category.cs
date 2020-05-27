using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Avalonia.Media;
using MediaPlayer.Domain.Abstracts;

namespace MediaPlayer.Domain.Models
{
  public class Category : AbstractObjectWithID
  {
    public string Name { get; set; }
    [NotMapped]
    public IBrush Color 
    { 
      get => new SolidColorBrush(){Color = new Color(0xff, R, G, B)};
    }

    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }

    public Category()
    {
      Name = "Tag";
      R = 255;
      G = 255;
      B = 255;
    }
  }
}