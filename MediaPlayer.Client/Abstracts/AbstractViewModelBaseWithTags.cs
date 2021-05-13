using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MediaPlayer.Client.ViewModels;
using MediaPlayer.Domain.Models;
using MediaPlayer.Storing.Repositories;
using ReactiveUI;

namespace MediaPlayer.Client.Abstracts
{
  public abstract class AbstractViewModelBaseWithTags : ViewModelBase
  {
    public ObservableCollection<Tag> TagList
    { 
      get => new ObservableCollection<Tag>(TagRepository.Tags);
    }
    public List<string> TagNameList
    {
      get => TagList.Select(x => x.Name).ToList();
    }
  }
}