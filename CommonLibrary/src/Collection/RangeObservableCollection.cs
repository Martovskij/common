using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace CommonLibrary
{
  /// <summary>
  /// Интерфейс коллекции, которая поддерживает массовую работу с элементами.
  /// При этом CollectionChanged райзится только один раз на всю массовую операцию.
  /// Так же есть возможность блокировать уведомления об изменении коллекции.
  /// </summary>
  /// <typeparam name="T">Тип элемента коллекции.</typeparam>
  public interface IRangeObservableCollection<T> : IList<T>, INotifyCollectionChanged
  {
    /// <summary>
    /// Заблокировать уведомления об изменении коллекции.
    /// </summary>
    void LockNotification();

    /// <summary>
    /// Разблокировать уведомления об изменении коллекции.
    /// </summary>
    /// <remarks>
    /// Если коллекция, в период заблокированности уведомлений, была изменена, то при разблокировки
    /// сработает уведомление об изменении, с действием NotifyCollectionChangedAction.Reset.
    /// </remarks>
    void UnlockNotification();

    /// <summary>
    /// Добавить список элементов.
    /// </summary>
    /// <param name="newItems">Список новых элементов.</param>
    void AddRange(IEnumerable<T> newItems);

    /// <summary>
    /// Удалить список элементов.
    /// </summary>
    /// <param name="deletedItems">Список удаляемых элементов.</param>
    void RemoveRange(IEnumerable<T> deletedItems);

    /// <summary>
    /// Очистить коллекцию, и заполнить ее новыми элементами.
    /// </summary>
    /// <param name="newItems">Новые элементы.</param>
    void Assign(IEnumerable<T> newItems);
  }

  /// <summary>
  /// Коллекции, которая поддерживает массовую работу с элементами.
  /// При этом CollectionChanged райзится только один раз на всю массовую операцию.
  /// Так же есть возможность блокировать уведомления об изменении коллекции.
  /// </summary>
  /// <typeparam name="T">Тип элемента коллекции.</typeparam>
  public class RangeObservableCollection<T> : ObservableCollection<T>, IRangeObservableCollection<T>
  {
    #region Поля и свойства

    /// <summary>
    /// Количество установленных блокировок (уровень вложенности блокировок).
    /// </summary>
    private int lockNotificationCount = 0;

    /// <summary>
    /// Признак того, что были подавлены уведомления.
    /// </summary>
    public bool HasSuppressedNotification { get; private set; }


    #endregion

    #region Базовые методы

    private NotifyCollectionChangedEventHandler collectionChanged;

    public override event NotifyCollectionChangedEventHandler CollectionChanged
    {
      add { collectionChanged += value; }
      remove { collectionChanged -= value; }
    }

    /// <summary>
    /// Сгенерировать событие на изменение коллекции.
    /// </summary>
    /// <param name="e">Аргументы события.</param>
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      if (this.lockNotificationCount > 0)
      {
        this.HasSuppressedNotification = true;
        return;
      }

      NotifyCollectionChangedEventHandler handlers = this.collectionChanged;
      if (handlers != null)
      {
        bool isRangeOperation = e != null &&
          ((e.NewItems != null && e.NewItems.Count > 1) || (e.OldItems != null && e.OldItems.Count > 1));

        foreach (NotifyCollectionChangedEventHandler handler in handlers.GetInvocationList())
        {
          // ANSEN: Хак, CollectionView не поддерживает range событий, ListCollectionView валится с ошибкой, поэтому просто обновляем если он подписан на событие.
          if (handler.Target is ICollectionView && isRangeOperation)
            handler(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
          else
            handler(this, e);
        }
      }
    }

    #endregion

    #region IRangeObservableCollection

    public void AddRange(IEnumerable<T> newItems)
    {
      if (newItems == null)
        throw new ArgumentNullException("newItems");
      if (!newItems.Any())
        return;

      this.LockNotification();
      try
      {
        foreach (T item in newItems)
          this.Add(item);
      }
      finally
      {
        this.UnlockNotification(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItems.ToList()));
      }
    }

    public void RemoveRange(IEnumerable<T> deletedItems)
    {
      if (deletedItems == null)
        throw new ArgumentNullException("deletedItems");
      if (!deletedItems.Any())
        return;

      this.LockNotification();
      try
      {
        foreach (T item in deletedItems)
          this.Remove(item);
      }
      finally
      {
        this.UnlockNotification(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, deletedItems.ToList()));
      }
    }

    public void Assign(IEnumerable<T> newItems)
    {
      if (newItems == null)
        throw new ArgumentNullException("newItems");

      this.LockNotification();
      try
      {
        this.Clear();
        this.AddRange(newItems);
      }
      finally
      {
        this.UnlockNotification();
      }
    }

    public void LockNotification()
    {
      this.lockNotificationCount++;
    }

    public void UnlockNotification()
    {
      // При типе действия Reset, передавать параметры old/new item не нужно.
      this.UnlockNotification(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }

    public void UnlockNotification(NotifyCollectionChangedEventArgs e)
    {
      if (this.lockNotificationCount == 0)
        throw new InvalidOperationException();

      this.lockNotificationCount--;

      if (this.lockNotificationCount == 0 && this.HasSuppressedNotification)
      {
        this.HasSuppressedNotification = false;
        this.OnCollectionChanged(e);
      }
    }

    #endregion

    /// <summary>
    /// Эта реализация сделана для DevExpress, чтобы он мог блокировать/разблокировать уведомлялку,
    /// через известный ему интерфейс.
    /// </summary>
    #region ILockable

    public void BeginUpdate()
    {
      this.LockNotification();
    }

    public void EndUpdate()
    {
      this.UnlockNotification();
    }

    public bool IsLockUpdate { get { return this.lockNotificationCount > 0; } }

    #endregion

    #region Конструкторы

    /// <summary>
    /// Конструктор.
    /// </summary>
    public RangeObservableCollection() { }

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="source">Элементы, которыми заполнить коллекцию.</param>
    public RangeObservableCollection(IEnumerable<T> source) : base(source) { }

    #endregion
  }
}
