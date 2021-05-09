using MediaPlayer.Domain.Abstracts;

namespace MediaPlayer.Domain.Models
{
  public class Tag : AbstractObjectWithID
  {
    public string Name { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public Tag ParentTag { get; set; }
  }
}