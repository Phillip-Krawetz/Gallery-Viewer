using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MediaPlayer.Client.ViewModels;
using MediaPlayer.Domain.Models;
using MediaPlayer.Storing.Repositories;

namespace MediaPlayer.Client.Abstracts
{
  public abstract class AbstractViewModelBaseWithTags : ViewModelBase
  {
    protected TagRepository tagRepo = new TagRepository();

    public ObservableCollection<Tag> TagList
    { 
      get => new ObservableCollection<Tag>(TagRepository.Tags);
    }

    protected DirectoryRepository directoryRepo = new DirectoryRepository();

    public List<string> TagNameList
    {
      get => TagList.Select(x => x.Name).OrderBy(x => x).ToList();
    }

    public DirectoryItem SelectedDirectory;

    public abstract void AddTag(DirectoryItem item, string tag);
  }
}