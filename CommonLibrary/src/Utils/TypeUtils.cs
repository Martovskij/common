namespace CommonLibrary
{
  /// <summary>
  /// Вспомогательные функции для работы с типами.
  /// </summary>
  public static class TypeCommonLibrary
  {
    /// <summary>
    /// Получить квалифицированное имя типа (без сборки).
    /// </summary>
    /// <param name="namespaceName">Пространство имен.</param>
    /// <param name="typeName">Имя типа.</param>
    /// <returns>Квалифицированное имя типа (без сборки).</returns>
    public static string GetQualifiedTypeName(string namespaceName, string typeName)
    {
      return string.Format("{0}.{1}", namespaceName, typeName);
    }

    /// <summary>
    /// Получить квалифицированное имя типа с указанием сборки.
    /// </summary>
    /// <param name="namespaceName">Пространство имен.</param>
    /// <param name="typeName">Имя типа.</param>
    /// <param name="assemblyName">Имя сборки.</param>
    /// <returns>Квалифицированное имя типа с указанием сборки.</returns>
    public static string GetAssemblyQualifiedTypeName(string namespaceName, string typeName, string assemblyName)
    {
      return string.Format("{0}.{1}, {2}", namespaceName, typeName, assemblyName);
    }

    /// <summary>
    /// Проверить, является ли переданное имя типа кавлифицированным.
    /// </summary>
    /// <param name="typeName">Имя типа.</param>
    /// <returns>Резальтат проверки.</returns>
    public static bool IsQualifiedTypeName(string typeName)
    {
      return typeName.Contains(".");
    }
  }
}
