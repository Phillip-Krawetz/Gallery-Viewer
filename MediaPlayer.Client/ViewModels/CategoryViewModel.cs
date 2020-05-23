using System.Collections.Generic;
using MediaPlayer.Domain.Models;
using MediaPlayer.Storing.Repositories;

namespace MediaPlayer.Client.ViewModels
{
  public class CategoryViewModel : ViewModelBase
  {
    public List<Category> Categories { get; set; }
    private CategoryRepository categoryRepository = new CategoryRepository();
    public CategoryViewModel()
    {
      Categories = CategoryRepository.Categories;
    }
    public void UpdateCategory(Category category)
    {
      if(category != null)
      {
        categoryRepository.UpdateOrAdd(category);
      }
    }
  }
}