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

    public override void Next()
    {
      this.content.Next();
    }

    public override void Prev()
    {
      this.content.Prev();
    }

    public override void ConfirmEventHandler()
    {
      this.content.ConfirmEventHandler();
    }
  }
}
