using MediaPlayer.Storing.Connectors;
using MediaPlayer.Storing.Repositories;
using ReactiveUI;

namespace MediaPlayer.Client.ViewModels
{
  public class MainWindowViewModel : ViewModelBase
  {
    ViewModelBase content;

    public MainWindowViewModel(){
      Content = new HomeMenuViewModel();
      CategoryRepository.Initialize();
      TagRepository.Initialize();
      DirectoryRepository.Initialize();
      FileSystemConnector.EnsureCreated();
    }

    public ViewModelBase Content
    {
      get => content;
      set => this.RaiseAndSetIfChanged(ref content, value);
    }

    public void DirectorySelect()
    {
      var vm = new DirectoryViewModel();
      Content = vm;
    }

    public override void NextEventHandler()
    {
      this.content.NextEventHandler();
    }

    public override void PrevEventHandler()
    {
      this.content.PrevEventHandler();
    }

    public override void ConfirmEventHandler()
    {
      this.content.ConfirmEventHandler();
    }

    public override void SearchEventHandler()
    {
      this.content.SearchEventHandler();
    }
  }
}
