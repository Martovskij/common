using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace CommonLibrary.Collection
{
  /// <summary>
  /// ObservableCollection, которая правильно генерирует событие CollectionChanged при вызове метода Clear.
  /// </summary>
  /// <typeparam name="T">Тип объекта коллекции.</typeparam>
  public class NotifyOnClearObservableCollection<T> : ObservableCollection<T>
  {
    #region Базовый класс

    protected override void ClearItems()
    {
      var removedItems = this.Items.ToList();
      base.ClearItems();
      var args = new NotifyCollectionChangedEventArgs(
        NotifyCollectionChangedAction.Remove, removedItems);
      this.OnCollectionChanged(args);
    }

    #endregion

    #region Конструкторы

    /// <summary>
    /// Конструктор.
    /// </summary>
    public NotifyOnClearObservableCollection()
    {
    }

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="collection">Коллекция-источник.</param>
    public NotifyOnClearObservableCollection(IEnumerable<T> collection)
      : base(collection)
    {
    }

    #endregion
  }
}
