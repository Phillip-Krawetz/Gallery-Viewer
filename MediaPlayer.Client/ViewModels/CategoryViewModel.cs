using System.Collections.ObjectModel;
using System.Linq;
using MediaPlayer.Domain.Models;
using MediaPlayer.Storing.Repositories;
using ReactiveUI;

namespace MediaPlayer.Client.ViewModels
{
  public class CategoryViewModel : ViewModelBase
  {
    private ObservableCollection<Category> categories;
    public ObservableCollection<Category> Categories
    {
      get => categories; 
      set => this.RaiseAndSetIfChanged(ref categories, value);
    }
    private CategoryRepository categoryRepository = new CategoryRepository();
    public CategoryViewModel()
    {
      Categories = new ObservableCollection<Category>(CategoryRepository.Categories);
    }
    public void UpdateCategory(Category category)
    {
      var existingCategory = categories.Where(x => x.Id == category.Id).FirstOrDefault();
      if(category != null)
      {
        categoryRepository.UpdateOrAdd(category);
        if(existingCategory == null)
        {
          categories.Add(category);
        }
        else
        {
          existingCategory = category;
        }
        this.RaisePropertyChanged("Categories");
      }
    }

    public void RemoveCategory(Category category)
    {
      if(Categories.Contains(category))
      {
        categories.Remove(category);
        categoryRepository.RemoveCategory(category);
        this.RaisePropertyChanged("Categories");
      }
    }
  }
}