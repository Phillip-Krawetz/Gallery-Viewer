using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Avalonia.Media;
using MediaPlayer.Domain.Interfaces;

namespace MediaPlayer.Domain.Models
{
  public class Tag
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
  }
}