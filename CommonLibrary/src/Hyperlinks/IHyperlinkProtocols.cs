using System.Collections.Generic;

namespace CommonLibrary
{
  /// <summary>
  /// Протоколы гиперссылок.
  /// </summary>
  public interface IHyperlinkProtocols
  {
    /// <summary>
    /// Cписок протоколов.
    /// </summary>
    IEnumerable<string> Protocols { get; }
  }
}
