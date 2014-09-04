using System.Collections.Generic;
using Microsoft.Win32;

namespace CommonLibrary
{
  /// <summary>
  /// Возвращает список протоколов из реестра.
  /// </summary>
  public class HyperlinkRegistryProtocols : IHyperlinkProtocols
  {
    // Список протоколов. Кеш для LoadFromRegistry().
    private static string[] ProtocolsCache { get; set; }

    /// <summary>
    ///  Загрузка списка протоколов из реестра.
    /// </summary>
    /// <returns>Список протоколов.</returns>
    private string[] LoadFromRegistry()
    {
      List<string> registryProtocols = new List<string>();
      foreach (string key in Registry.ClassesRoot.GetSubKeyNames())
      {
        var subkey = Registry.ClassesRoot.OpenSubKey(key);
        var urlProtocolValue = subkey.GetValue("URL Protocol");
        if (urlProtocolValue != null)
          registryProtocols.Add(key);
      }
      return registryProtocols.ToArray();
    }

    public IEnumerable<string> Protocols
    {
      get
      {
        if (ProtocolsCache == null)
          ProtocolsCache = this.LoadFromRegistry();
        return ProtocolsCache;
      }
    }
  }

  /// <summary>
  /// Список протоколов основанных на http.
  /// </summary>
  public class HttpProtocols : IHyperlinkProtocols
  {
    public IEnumerable<string> Protocols
    {
      get { return new string[] { "http", "https" }; }
    }
  }
}
