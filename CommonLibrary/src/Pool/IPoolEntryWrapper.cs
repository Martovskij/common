namespace CommonLibrary
{
  /// <summary>
  /// Интерфейс элемента-обертки пула.
  /// </summary>
  public interface IPoolEntryWrapper
  {
    /// <summary>
    /// Освободить ресурсы элемента пула.
    /// </summary>
    void Release();

    /// <summary>
    /// Имя запуленного объекта.
    /// </summary>
    string TargetName { get; }
  }
}
