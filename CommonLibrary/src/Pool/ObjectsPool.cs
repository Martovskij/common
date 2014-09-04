using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common.Logging;

namespace CommonLibrary
{
  /// <summary>
  /// Пул объектов.
  /// </summary>
  /// <typeparam name="TEntry">Тип запуленных объектов.</typeparam>
  public class ObjectsPool<TEntry>
    where TEntry : class
  {
    #region Поля и свойства

    private static readonly ILog log = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Элементы пула в порядке использования (сначала редко используемые).
    /// </summary>
    private readonly List<TEntry> mruEntries = new List<TEntry>();

    /// <summary>
    /// Элементы пула.
    /// </summary>
    public ObservableCollection<TEntry> Entries { get; private set; }

    /// <summary>
    /// Максимальное количество объектов в пуле.
    /// </summary>
    public int MaxCount { get; private set; }

    #endregion

    #region Методы

    /// <summary>
    /// Добавить элемент в пул.
    /// </summary>
    /// <param name="entry">Добавляемый элемент.</param>
    /// <returns>Список изъятых элементов.</returns>
    public IEnumerable<TEntry> Push(TEntry entry)
    {
      var poppedEntries = new List<TEntry>();
      IPoolEntryWrapper entryWrapper;
      if (this.Entries.Count >= this.MaxCount)
      {
        for (int i = this.mruEntries.Count - this.MaxCount; i >= 0; i--)
        {
          var removedEntry = this.mruEntries[i];
          this.mruEntries.RemoveAt(i);
          this.Entries.Remove(removedEntry);
          entryWrapper = removedEntry as IPoolEntryWrapper;
          log.TraceFormat("{0} is removed from pool. New pool size: {1}",
            entryWrapper != null ? entryWrapper.TargetName : removedEntry.GetType().Name,
            this.Entries.Count);
          if (entryWrapper != null)
            entryWrapper.Release();
          poppedEntries.Add(removedEntry);
        }
      }
      this.mruEntries.Add(entry);
      this.Entries.Add(entry);
      entryWrapper = entry as IPoolEntryWrapper;
      log.TraceFormat("{0} is pushed to pool. New pool size: {1}",
        entryWrapper != null ? entryWrapper.TargetName : entry.GetType().Name,
        this.Entries.Count);
      return poppedEntries;
    }

    /// <summary>
    /// Изъять элемент из пула, соответствующий критерию.
    /// </summary>
    /// <param name="match">Критерий, которому должен соответствовать элемент.</param>
    /// <returns>Элемент пула (null если не найден).</returns>
    public TEntry Pop(Predicate<TEntry> match)
    {
      for (int i = this.mruEntries.Count - 1; i >= 0; i--)
      {
        if (match(this.mruEntries[i]))
        {
          var result = this.mruEntries[i];
          this.mruEntries.RemoveAt(i);
          this.Entries.Remove(result);
          var entryWrapper = result as IPoolEntryWrapper;
          log.TraceFormat("{0} is popped from pool. New pool size: {1}",
            entryWrapper != null ? entryWrapper.TargetName : result.GetType().Name,
            this.Entries.Count);
          return result;
        }
      }
      return null;
    }

    /// <summary>
    /// Получить элемент из пула, соответствующий критерию.
    /// </summary>
    /// <param name="match">Критерий, которому должен соответствовать элемент.</param>
    /// <returns>Элемент пула (null если не найден).</returns>
    public TEntry Peek(Predicate<TEntry> match)
    {
      for (int i = this.mruEntries.Count - 1; i >= 0; i--)
      {
        if (match(this.mruEntries[i]))
        {
          var result = this.mruEntries[i];
          this.mruEntries[i] = this.mruEntries[this.mruEntries.Count - 1];
          this.mruEntries[this.mruEntries.Count - 1] = result;
          var entryWrapper = result as IPoolEntryWrapper;
          log.TraceFormat("{0} is picked from pool. Pool size {1}, is not changed",
            entryWrapper != null ? entryWrapper.TargetName : result.GetType().Name,
            this.Entries.Count);
          return result;
        }
      }
      return null;
    }

    /// <summary>
    /// Очистить пул.
    /// </summary>
    public void Clear()
    {
      foreach (var entryWrapper in this.Entries.OfType<IPoolEntryWrapper>())
        entryWrapper.Release();
      this.mruEntries.Clear();
      this.Entries.Clear();
    }

    #endregion

    #region Конструкторы

    public ObjectsPool(int maxCount)
    {
      this.MaxCount = maxCount;
      this.Entries = new ObservableCollection<TEntry>();
    }

    #endregion
  }
}
