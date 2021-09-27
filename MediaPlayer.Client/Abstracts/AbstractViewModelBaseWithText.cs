using MediaPlayer.Client.ViewModels;

namespace MediaPlayer.Client.Abstracts
{
  public abstract class AbstractViewModelBaseWithText : ViewModelBase
  {
    private string searchtext;
    public string searchText
    { 
      get{
        return searchtext ?? "";
      }
      set => this.searchtext = value;
    }
  }
}