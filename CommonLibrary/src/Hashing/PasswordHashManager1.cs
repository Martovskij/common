using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;

namespace CommonLibrary.Hashing
{
  /// <summary>
  /// Методы расширения для работы с паролем.
  /// </summary>
  public static class PasswordHashExtensions
  {
    /// <summary>
    /// Получить хеш пароля пользователя.
    /// </summary>
    /// <param name="password">Пароль пользователя.</param>
    /// <returns>Хеш пароля пользователя.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Отдается наружу для использования в проверке пароля, поэтому Dispose вызывать нельзя.")]
    public static SecureString GetPasswordHash(this SecureString password)
    {
      byte[] result = PasswordHashManager.Instance.GenerateHash(password);
      SecureString passwordHash = new SecureString();
      foreach (char c in Convert.ToBase64String(result))
        passwordHash.AppendChar(c);
      return passwordHash;
    }
  }

  /// <summary>
  /// Менеджер хэширования.
  /// http://rsdn.ru/article/files/Classes/passwd.xml.
  /// </summary>
  public class PasswordHashManager
  {
    #region Singleton

    /// <summary>
    /// Экземпляр менеджера хэширования.
    /// </summary>
    public static PasswordHashManager Instance
    {
      get
      {
        if (instance == null)
          instance = new PasswordHashManager();
        return instance;
      }
    }

    private static PasswordHashManager instance;

    /// <summary>
    /// Закрытый конструктор.
    /// </summary>
    private PasswordHashManager() { }

    #endregion

    #region Методы

    /// <summary>
    /// Генерация соли.
    /// </summary>
    /// <returns>Массив сисмволов для соли.</returns>
    public byte[] GenerateSalt()
    {
      byte[] random = new byte[9];
      using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
      {
        randomNumberGenerator.GetBytes(random);
      }
      return random;
    }

    /// <summary>
    /// Генерация хэша пароля.
    /// </summary>
    /// <param name="password">Хэш пароля.</param>
    /// <returns>Хэш пароля.</returns>
    public byte[] GenerateHash(SecureString password)
    {
      IntPtr bstr = IntPtr.Zero;
      try
      {
        byte[] ssBytes = new byte[password.Length * 2];
        bstr = Marshal.SecureStringToBSTR(password);
        Marshal.Copy(bstr, ssBytes, 0, ssBytes.Length);
        return this.GenerateHash(ssBytes);
      }
      finally
      {
        Marshal.ZeroFreeBSTR(bstr);
      }
    }

    /// <summary>
    /// Генерация хэша пароля.
    /// </summary>
    /// <param name="password">Хэш пароля.</param>
    /// <returns>Хэш пароля.</returns>
    private byte[] GenerateHash(byte[] password)
    {
      SHA512Managed sha = null;
      try
      {
        sha = new SHA512Managed();
        byte[] hashed = sha.ComputeHash(password);
        return hashed;
      }
      finally
      {
        if (sha != null)
          sha.Dispose();
      }
    }

    /// <summary>
    /// Генерация хэша пароля c солью.
    /// </summary>
    /// <param name="passwordHash">Хэш пароля.</param>
    /// <param name="salt">Соль.</param>
    /// <returns>Хэш пароля.</returns>
    public byte[] AddSaltToHash(byte[] passwordHash, byte[] salt)
    {
      return this.GenerateHash(passwordHash.Union(salt).ToArray());
    }

    /// <summary>
    /// Проверка хеша пароля.
    /// </summary>
    /// <param name="passwordHash">Проверяемый хеш пароля, закодированный в base64.</param>
    /// <param name="sourceHash">Исходный хеш.</param>
    /// <param name="salt">Соль, подмешаная к исходному хешу.</param>
    /// <returns></returns>
    public bool CheckHash(SecureString passwordHash, byte[] sourceHash, byte[] salt)
    {
      // расшифруем хеш пароля из base64 представления.
      byte[] hash = Convert.FromBase64String(passwordHash.ToUnsecuredString());

      // соберем хеш для проверки.
      byte[] generatedPasswordHash = this.AddSaltToHash(hash, salt);

      // проверим что пароль верен
      if (generatedPasswordHash.SequenceEqual(sourceHash))
        return true;

      return false;
    }

    #endregion
  }
}
