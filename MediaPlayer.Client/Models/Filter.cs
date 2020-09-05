using MediaPlayer.Domain.Models;

namespace MediaPlayer.Client.Models
{
  public class Filter
  {
    public bool Exclude { get; set; }
    public Tag Tag { get; set; }

    public Filter(Tag tag, bool exclude)
    {
      Exclude = exclude;
      Tag = tag;
    }
  }
}