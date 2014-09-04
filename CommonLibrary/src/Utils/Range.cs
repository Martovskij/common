namespace CommonLibrary
{
  /// <summary>
  /// Диапазон значений.
  /// </summary>
  /// <typeparam name="T">Тип значений.</typeparam>
  public sealed class Range<T>
  {
    #region Поля и свойства

    /// <summary>
    /// Нижняя граница диапазона.
    /// </summary>
    public T LowerBound { get; private set; }

    /// <summary>
    /// Верхняя граница диапазона.
    /// </summary>
    public T UpperBound { get; private set; }

    #endregion

    #region Конструкторы

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="lowerBound">Нижняя граница диапазона.</param>
    /// <param name="upperBound">Верхняя граница диапазона.</param>
    public Range(T lowerBound, T upperBound)
    {
      this.LowerBound = lowerBound;
      this.UpperBound = upperBound;
    }

    #endregion
  }
}
