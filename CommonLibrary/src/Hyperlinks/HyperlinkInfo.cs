using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace CommonLibrary
{
  /// <summary>
  /// Информация о гиперссылке.
  /// </summary>
  public class HyperlinkInfo
  {
    #region Поля и свойства

    /// <summary>
    /// Имя ключа параметра отвечающего за тип сущности, указанного в гиперссылке.
    /// </summary>
    public const string TypeKeyName = "type";

    /// <summary>
    /// Имя ключа параметра отвечающего за идентификатор сущности.
    /// </summary>
    public const string IdKeyName = "id";

    /// <summary>
    /// Параметры запроса в гиперссылке.
    /// </summary>
    private readonly NameValueCollection parameters;

    private readonly Uri httpUri;

    /// <summary>
    /// Получить гиперссылку со схемой HTTP.
    /// </summary>
    public Uri HttpUri { get { return this.httpUri; } }

    private readonly Uri hyperlinkServer;

    /// <summary>
    /// Получить адрес сервера гиперссылок.
    /// </summary>
    public Uri HyperlinkServer { get { return this.hyperlinkServer; } }

    /// <summary>
    /// Получить количество параметров запроса в гиперссылке.
    /// </summary>
    public int ParametersCount { get { return this.parameters.AllKeys.Length; } }

    /// <summary>
    /// Получить значение параметра по ключу, указанных в гиперссылке.
    /// </summary>
    /// <param name="index">Имя ключа параметра.</param>
    /// <returns>Значение параметра.</returns>
    public string this[string index]
    {
      get
      {
        var key = this.parameters.AllKeys.Single(k => k != null && k.Equals(index, StringComparison.OrdinalIgnoreCase));
        return this.parameters[key];
      }
    }

    #endregion

    #region Методы

    /// <summary>
    /// Проверить имеется ли параметр в запросе в гиперссылке.
    /// </summary>
    /// <param name="key">Ключ.</param>
    /// <returns>True, если указанный параметр существует, иначе - false.</returns>
    public bool HasParameter(string key)
    {
      return this.parameters.AllKeys.Any(k => k != null && k.Equals(key, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Проверить валидность гиперссылки.
    /// </summary>
    /// <param name="hyperlink">Ссылка на объект системы.</param>
    /// <returns>True, если гиперссылка валидна, иначе - false.</returns>
    public static bool IsValid(string hyperlink)
    {
      return Uri.IsWellFormedUriString(hyperlink, UriKind.Absolute);
    }

    /// <summary>
    /// Проверить является ли гиперссылка ссылкой на сущность.
    /// </summary>
    /// <param name="hyperlink">Гиперссылка.</param>
    /// <returns>True если является, иначе - false.</returns>
    public static bool IsEntityHyperlink(HyperlinkInfo hyperlink)
    {
      Guid typeGuid;
      int id;
      return hyperlink.ParametersCount == 2 && hyperlink.HasParameter(TypeKeyName) && hyperlink.HasParameter(IdKeyName)
        && Guid.TryParse(hyperlink[TypeKeyName], out typeGuid) && int.TryParse(hyperlink[IdKeyName], out id);
    }

    /// <summary>
    /// Проверить является ли гиперссылка ссылкой на тип сущности.
    /// </summary>
    /// <param name="hyperlink">Гиперссылка.</param>
    /// <returns>True если является, иначе - false.</returns>
    public static bool IsEntityTypeHyperlink(HyperlinkInfo hyperlink)
    {
      Guid typeGuid;
      return hyperlink.ParametersCount == 1 && hyperlink.HasParameter(TypeKeyName) && Guid.TryParse(hyperlink[TypeKeyName], out typeGuid);
    }

    #endregion

    #region Конструктор

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="hyperlink">Гиперссылка.</param>
    public HyperlinkInfo(string hyperlink)
    {
      var uri = new Uri(HttpUtility.HtmlDecode(HttpUtility.UrlDecode(hyperlink)));
      var builder = new UriBuilder(uri);
      builder.Path = builder.Path.EndsWith(@"/", StringComparison.OrdinalIgnoreCase) ? builder.Path.Substring(0, builder.Path.Length - 1) : builder.Path;
      this.httpUri = builder.Uri;
      builder.Query = null;
      this.hyperlinkServer = builder.Uri;
      this.parameters = HttpUtility.ParseQueryString(uri.Query);
    }

    #endregion
  }
}
