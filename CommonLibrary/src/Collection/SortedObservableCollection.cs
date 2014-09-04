using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CommonLibrary.Collection
{
  /// <summary>
  /// Отслеживаемая коллекция, которая всегда поддерживается в отсортированном состоянии.
  /// </summary>
  /// <typeparam name="T">Тип элементов в коллекции.</typeparam>
  public class SortedObservableCollection<T> : ObservableCollection<T>
  {
    #region Константы

    /// <summary>
    /// Верхняя граница количества элементов в списке, 
    /// для которого можно выполнять линейный поиск места вставки нового элемента.
    /// </summary>
    private const int LinearSearchMaxItemsCount = 2;

    #endregion

    #region Поля и свойства

    private IComparer<T> comparer; 

    /// <summary>
    /// Компаратор для сравнения элементов при сортировке.
    /// </summary>
    public IComparer<T> Comparer 
    {
      get { return this.comparer ?? Comparer<T>.Default; }
    }

    #endregion

    #region Методы

    /// <summary>
    /// Выполнить бинарный поиск места вставки элемента в отсортированный список.
    /// </summary>
    /// <param name="lowIndex">Индекс нижней границы участка списка, среди которого ищем место для вставки.</param>
    /// <param name="highIndex">Индекс верхней границы участка списка, среди которого ищем место для вставки.</param>
    /// <param name="item">Элемент, место вставки которого нужно найти.</param>
    /// <returns>Индекс для вставки нового элемента.</returns>
    private int BinarySearchInsertionIndex(int lowIndex, int highIndex, T item)
    {
      if (highIndex - lowIndex <= LinearSearchMaxItemsCount)
        return this.LinearSearchInsertionIndex(lowIndex, highIndex, item);

      var compareIndex = (lowIndex + highIndex) / 2;
      var comparisionResult = this.Comparer.Compare(item, this[compareIndex]);

      if (comparisionResult < 0)
      {
        // Новый элемент нужно вставить в левую половину отсортированного списка.
        return this.BinarySearchInsertionIndex(lowIndex, compareIndex, item);
      }
      else
      {
        // Новый элемент нужно вставить в правую половину отсортированного списка.
        return this.BinarySearchInsertionIndex(compareIndex, highIndex, item);
      }
    }

    /// <summary>
    /// Выполнить линейный поиск места вставки элемента в отсортированный список.
    /// </summary>
    /// <param name="lowIndex">Индекс нижней границы участка списка, среди которого ищем место для вставки.</param>
    /// <param name="highIndex">Индекс верхней границы участка списка, среди которого ищем место для вставки.</param>
    /// <param name="item">Элемент, место вставки которого нужно найти.</param>
    /// <returns>Индекс для вставки нового элемента.</returns>
    private int LinearSearchInsertionIndex(int lowIndex, int highIndex, T item)
    {
      for (int i = highIndex; i >= lowIndex; i--)
      {
        if (this.Comparer.Compare(item, this[i]) >= 0)
          return i + 1;
      }
      return lowIndex;
    }

    #endregion

    #region Базовый класс

    protected override void InsertItem(int index, T item)
    {
      base.InsertItem(this.Count != 0 ? this.BinarySearchInsertionIndex(0, this.Count - 1, item) : 0, item);
    }

    protected override void SetItem(int index, T item)
    {
      this.InsertItem(index, item);
    }

    #endregion

    #region Конструкторы

    /// <summary>
    /// Создать пустую коллекцию.
    /// </summary>
    /// <param name="comparer">Компаратор для сравнения элементов при сортировке.</param>
    public SortedObservableCollection(IComparer<T> comparer)
      : this(Enumerable.Empty<T>(), comparer)
    {
    }

    /// <summary>
    /// Создать первоначально заполненную коллекцию.
    /// </summary>
    /// <param name="collection">Начальное содержимое коллекции.</param>
    /// <param name="comparer">Компаратор для сравнения элементов при сортировке.</param>
    public SortedObservableCollection(IEnumerable<T> collection, IComparer<T> comparer)
    {
      this.comparer = comparer;
      collection.ForEach(item => this.InsertItem(0, item));
    }

    #endregion
  }
}
