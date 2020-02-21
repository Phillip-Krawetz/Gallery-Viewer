using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MediaPlayer.Domain.Models
{
  public class Tag
  {
    public int Id { get; set; }
    public string Name { get; set; }
  }
}