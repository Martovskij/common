using System.Security;

namespace Utils
{
  /// <summary>
  /// Аггрегатор аутентификационных данных.
  /// </summary>
  public class AuthenticationContext
  {
    /// <summary>
    /// Логин.
    /// </summary>
    public string Login { get; set; }

    /// <summary>
    /// Пароль.
    /// </summary>
    public SecureString Password { get; set; }
  }
}
