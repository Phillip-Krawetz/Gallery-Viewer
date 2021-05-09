using System.Collections.ObjectModel;
using System.Linq;
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
    private ObservableCollection<Tag> tagList;
    public ObservableCollection<Tag> TagList
    { 
      get => tagList;
      set => this.RaiseAndSetIfChanged(ref tagList, value); 
    }
    private Tag tag;
    public TagWindowViewModel(Tag tag)
    {
      this.tag = tag;
      Categories = new ObservableCollection<Category>(CategoryRepository.Categories);
      TagList = new ObservableCollection<Tag>(TagRepository.Tags);
      TagList.Insert(0, new Tag(){Name = "None", Id = -1});
    }

    public void SaveTag(string Name, Category category, Tag parentTag)
    {
      tag.Name = Name;
      tag.Category = category;
      tag.CategoryId = category.Id;
      tag.ParentTag = null;
      if(parentTag.Id >= 0)
      {
        tag.ParentTag = parentTag;
      }
      var tagRepo = new TagRepository();
      tagRepo.UpdateTag(tag);
    }
  }
}