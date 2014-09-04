using System;
using Common.Logging;
using Newtonsoft.Json;

namespace CommonLibrary.Json
{
  /// <summary>
  ///  
  /// </summary>
  public static class JsonHelper
  {

    private static ILog log = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string Serialize(object value)
    {
      return JsonConvert.SerializeObject(value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static object Deserialize(string value)
    {
      return JsonConvert.DeserializeObject(value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T Deserialize<T>(string value)
    {
      return JsonConvert.DeserializeObject<T>(value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static object TryDeserialize(string value)
    {
      try
      {
        return JsonConvert.DeserializeObject(value);
      }
      catch(Exception ex)
      {
        log.Error(ex);
        return null;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
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
