using System.Collections.ObjectModel;
using MediaPlayer.Domain.Models;
using MediaPlayer.Storing.Repositories;
using ReactiveUI;

namespace MediaPlayer.Client.ViewModels
{
  public class TagWindowViewModel : ViewModelBase
  {
    private ObservableCollection<Category> categories;
    public ObservableCollection<Category> Categories 
    { 
      get => categories;
      set => this.RaiseAndSetIfChanged(ref categories, value); 
    }
    private Tag tag;
    public TagWindowViewModel(Tag tag)
    {
      this.tag = tag;
      Categories = new ObservableCollection<Category>(CategoryRepository.Categories);
    }

    public void SaveTag(string Name, Category category)
    {
      tag.Name = Name;
      tag.Category = category;
      tag.CategoryId = category.Id;
      var tagRepo = new TagRepository();
      tagRepo.UpdateTag(tag);
    }
  }
}