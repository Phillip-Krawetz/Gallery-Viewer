using MediaPlayer.Domain.Variables;
using MediaPlayer.Storing.Connectors;
using ReactiveUI;

namespace MediaPlayer.Client.ViewModels
{
  public class OptionsViewModel : ViewModelBase
  {
    private InitialOptions currentOptions;
    public InitialOptions CurrentOptions
    { 
      get => currentOptions;
      set => this.RaiseAndSetIfChanged(ref currentOptions, value);
    }
    public OptionsViewModel()
    {
      CurrentOptions = new InitialOptions();
      CurrentOptions.CopyCurrent();
    }

    public void SaveOptions()
    {
      CurrentOptions.MapToOptions();
      FileSystemConnector.WriteOptions();
    }
  }
}