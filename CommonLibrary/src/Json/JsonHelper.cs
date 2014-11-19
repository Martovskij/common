using System;
using Common.Logging;
using Newtonsoft.Json;

namespace CommonLibrary.Json
{
  /// <summary>
  ///  Хэлпер для десериализации JSON.
  /// </summary>
  public static class JsonHelper
  {
    /// <summary>
    /// Логгер.
    /// </summary>
    private static ILog log = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Сериализоват объект в строку.
    /// </summary>
    /// <param name="value">Объект.</param>
    /// <returns>Строка.</returns>
    public static string Serialize(object value)
    {
      return JsonConvert.SerializeObject(value);
    }

    /// <summary>
    /// Десериализовать строку в объект.
    /// </summary>
    /// <param name="value">Строка.</param>
    /// <returns>Объект.</returns>
    public static object Deserialize(string value)
    {
      return JsonConvert.DeserializeObject(value);
    }

    /// <summary>
    /// Десериализовать строку в объект указанного типа.
    /// </summary>
    /// <typeparam name="T">Требуемый тип.</typeparam>
    /// <param name="value">Строка.</param>
    /// <returns>Экземпляр объекта требуемого типа.</returns>
    public static T Deserialize<T>(string value)
    {
      return JsonConvert.DeserializeObject<T>(value);
    }

    /// <summary>
    /// Попытаться десериализовать строку в объект.
    /// </summary>
    /// <param name="value">Строка.</param>
    /// <returns>Экземпляр объекта.</returns>
    public static object TryDeserialize(string value)
    {
      try
      {
        return JsonConvert.DeserializeObject(value);
      }
      catch (Exception ex)
      {
        log.Error(ex);
        return null;
      }
    }

    /// <summary>
    /// Попытаться десериализовать строку в объект указанного типа.
    /// </summary>
    /// <typeparam name="T">Требуемый тип.</typeparam>
    /// <param name="value">Строка.</param>
    /// <returns>Экземпляр объекта требуемого типа.</returns>
    public static T TryDeserialize<T>(string value) where T : class
    {
      try
      {
        return JsonConvert.DeserializeObject<T>(value);
      }
      catch
      {
        return null;
      }
    }
  }
}
