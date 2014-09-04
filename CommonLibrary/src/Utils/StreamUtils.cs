using System.IO;
using System.Security.Cryptography;

namespace CommonLibrary
{
  /// <summary>
  /// Класс-расширение для работы с потоками.
  /// </summary>
  public static class StreamUtils
  {
    /// <summary>
    /// Получить строковое представление MD5-хеша.
    /// </summary>
    /// <param name="hash">MD5-хеш.</param>
    /// <returns>Строковое представление MD5-хеша.</returns>
    private static string GetHashString(byte[] hash)
    {
      string resultHashString = string.Empty;
      for (int i = 0; i <= hash.Length - 1; i++)
        resultHashString += hash[i].ToString("x2");
      return resultHashString;
    }

    /// <summary>
    /// Метод-расширение для получение MD5-хэша потока.
    /// </summary>
    /// <param name="stream">Поток.</param>
    /// <returns>MD5-хэш потока.</returns>
    public static string GetMD5Hash(this Stream stream)
    {
      // TODO: Возможно лучше использовать классы из ICSharpCode.SharpZipLib.Checksums, т.к. все равно используем эту библиотеку.
      using (HashAlgorithm algorythmMD5 = (HashAlgorithm)new MD5CryptoServiceProvider())
        return GetHashString(algorythmMD5.ComputeHash(stream));
    }

    /// <summary>
    /// Метод-расширение для получения MD5-хеша массива байт.
    /// </summary>
    /// <param name="data">Массив байт.</param>
    /// <returns>MD5-хеш массива байт.</returns>
    public static string GetMD5Hash(this byte[] data)
    {
      using (HashAlgorithm algorythmMD5 = (HashAlgorithm)new MD5CryptoServiceProvider())
        return GetHashString(algorythmMD5.ComputeHash(data));
    }

    /// <summary>
    /// Считать поток от начала до конца.
    /// </summary>
    /// <param name="stream">Экземпляр потока.</param>
    /// <param name="bufferSize">Размер буффера.</param>
    /// <returns>Массив байт потока.</returns>
    public static byte[] ReadToEnd(this Stream stream, int bufferSize)
    {
      byte[] result = null;

      using (MemoryStream memoryStream = new MemoryStream())
      {
        stream.CopyTo(memoryStream, bufferSize);
        result = memoryStream.ToArray();
      }

      return result;
    }
  }
}
