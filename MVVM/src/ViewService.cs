using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace MVVM
{
  /// <summary>
  /// Сервис управления окнами.
  /// </summary>
  public static class ViewService
  {
    #region Поля и свойства

    /// <summary>
    /// Контейнер сопоставления типов моделей-представления с типами представления (View to ViewModel).
    /// </summary>
    private static Dictionary<Type, Type> windowAssociateContainer = new Dictionary<Type, Type>(); 


    /// <summary>
    /// Коллекция открытых окон.
    /// </summary>
    private static readonly Collection<Window> openedWindows = new Collection<Window>();

    #endregion

    #region Методы

    /// <summary>
    /// Привязать тип представления к типу модели представления.
    /// </summary>
    /// <param name="viewType">Тип представления.</param>
    /// <param name="viewModelType">Тип модели-представления.</param>
    public static void BindViewToViewModel(Type viewType, Type viewModelType)
    {
      if (!windowAssociateContainer.ContainsKey(viewType))
        windowAssociateContainer.Add(viewModelType, viewType);
    }


    /// <summary>
    /// Открыть представление.
    /// </summary>
    /// <param name="viewModel">Модель представления.</param>
    /// <param name="width">Ширина окна.</param>
    /// <param name="height">Высота окна.</param>
    /// <param name="mode">Режим отображения.</param>
    public static Window OpenViewModel(ViewModelBase viewModel, double width, double height, ViewMode mode)
    {
      var openedWindow = openedWindows.SingleOrDefault(window => window.DataContext.Equals(viewModel));
      if (openedWindow == null)
      {
        var result = windowAssociateContainer.ContainsKey(viewModel.GetType());
        if (!result)
        {
          openedWindow = new Window();
          openedWindow.Width = width;
          openedWindow.Height = height;
        }
        else
          openedWindow = (Window)Activator.CreateInstance(windowAssociateContainer[viewModel.GetType()]);
        openedWindow.Title = viewModel.Name;
        openedWindow.DataContext = viewModel;
        openedWindow.Closed += OpenedWindowClosed;
        openedWindows.Add(openedWindow);
        if (mode == ViewMode.Modal)
          openedWindow.ShowDialog();
        else
          openedWindow.Show();
      }
      else
        openedWindow.Activate();
      SetMainWindowIfNeed();
      return openedWindow;
    }

    private static void SetMainWindowIfNeed()
    {
      if (openedWindows.Count == 1)
        Application.Current.MainWindow = openedWindows.Single();
    }

    /// <summary>
    /// Открыть представление.
    /// </summary>
    /// <param name="viewModel">Модель представления.</param>
    public static Window OpenViewModel(ViewModelBase viewModel)
    {
      return OpenViewModel(viewModel, 640, 480, ViewMode.Unmodal);
    }

    /// <summary>
    /// Открыть представление как модальное.
    /// </summary>
    /// <param name="viewModel">Модель представления.</param>
    public static Window OpenViewModelAsModal(ViewModelBase viewModel)
    {
      return OpenViewModel(viewModel, 640, 480, ViewMode.Modal);
    }

    /// <summary>
    /// Обработчик события на закрытие окна.
    /// </summary>
    /// <param name="sender">Источник события.</param>
    /// <param name="e">Параметры события.</param>
    private static void OpenedWindowClosed(object sender, EventArgs e)
    {
      var window = sender as Window;
      if (window != null && openedWindows.Contains(window))
      {
        openedWindows.Remove(window);
        window.Closed -= OpenedWindowClosed;
      }
      SetMainWindowIfNeed();
    }

     

    #endregion
  }
}
