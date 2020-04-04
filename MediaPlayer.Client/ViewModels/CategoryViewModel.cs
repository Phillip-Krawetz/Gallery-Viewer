using System.Collections.Generic;
using MediaPlayer.Domain.Models;
using MediaPlayer.Storing.Repositories;

namespace MediaPlayer.Client.ViewModels
{
  public class CategoryViewModel : ViewModelBase
  {
    public List<Category> Categories { get; set; }
    public CategoryViewModel()
    {
      Categories = CategoryRepository.Categories;
    }
  }
}