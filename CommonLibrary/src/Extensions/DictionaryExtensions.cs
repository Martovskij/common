using System.Collections.Generic;

namespace CommonLibrary
{
  /// <summary>
  /// Расширения для работы со словарем.
  /// </summary>
  public static class DictionaryExtensions
  {
    /// <summary>
    /// Получить значение по ключу.
    /// </summary>
    /// <typeparam name="TKey">Тип ключа.</typeparam>
    /// <typeparam name="TValue">Тип значения.</typeparam>
    /// <param name="dictionary">Словарь.</param>
    /// <param name="key">Ключ.</param>
    /// <returns>Найденное по ключу значение или "пустое" значение для данного типа, если по ключу ничего не найдено.</returns>
    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
      return dictionary.GetValueOrDefault(key, default(TValue));
    }

    /// <summary>
    /// Получить значение по ключу.
    /// </summary>
    /// <typeparam name="TKey">Тип ключа.</typeparam>
    /// <typeparam name="TValue">Тип значения.</typeparam>
    /// <param name="dictionary">Словарь.</param>
    /// <param name="key">Ключ.</param>
    /// <param name="defaultValue">Значение по-умолчанию, если по ключу значения не нашлось.</param>
    /// <returns>Найденное по ключу значение или значение по-умолчанию, если по ключу ничего не найдено.</returns>
    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
    {
      TValue resultValue;
      if (dictionary.TryGetValue(key, out resultValue))
        return resultValue;
      return defaultValue;
    }
  }
}
