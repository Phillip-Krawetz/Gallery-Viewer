using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using Avalonia.Controls;
using Avalonia.Input;
using MediaPlayer.Domain.Models;
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

    public static KeyGesture NextKey
    {
      get => new KeyGesture(Key.Right, KeyModifiers.None);
    }

    public static KeyGesture PrevKey
    {
      get => new KeyGesture(Key.Left, KeyModifiers.None);
    }

    public static KeyGesture BackKey
    {
      get => new KeyGesture(Key.Left, KeyModifiers.Control);
    }

    public override void Next()
    {
      this.content.Next();
    }

    public override void Prev()
    {
      this.content.Prev();
    }
  }
}
