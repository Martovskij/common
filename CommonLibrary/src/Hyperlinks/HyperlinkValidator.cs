using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonLibrary
{
  /// <summary>
  /// Валидатор гиперссылок.
  /// </summary>
  public class HyperlinkValidator
  {        
    /// <summary>
    /// Список протоколов.
    /// </summary>
    private IHyperlinkProtocols protocols;

    /// <summary>
    /// Проверка URI на корректность. Базируется на IsWellFormedUriString.
    /// </summary>
    /// <param name="token">Текст ссылки.</param>
    /// <returns>True, если ссылка корректная.</returns>
    public bool IsValidUriString(string token)
    {
      if (!Uri.IsWellFormedUriString(token, UriKind.Absolute))
        return false;
      var separatorIndex = token.IndexOf(':');
      if (separatorIndex == -1)
        return false;
      return this.protocols.Protocols.Contains(token.Substring(0, separatorIndex), StringComparer.OrdinalIgnoreCase);
    }

    public HyperlinkValidator(IHyperlinkProtocols protocols)
    {
      this.protocols = protocols;           
    }
  }
}
