using System.IO;
using System.IO.Compression;

namespace CommonLibrary
{
  /// <summary>
  /// Методы расширения для работы с массивами.
  /// </summary>
  public static class ArrayExtensions
  {
    /// <summary>
    /// Получить сжатый массив байт.
    /// </summary>
    /// <param name="source">Несжатый массив.</param>
    /// <param name="compressionLevel">Уровень сжатия.</param>
    /// <returns>Сжатый массив байт.</returns>
    public static byte[] Compress(this byte[] source, CompressionLevel compressionLevel)
    {
      if (source == null || source.Length == 0)
        return source;

      using (var sourceStream = new MemoryStream(source))
        return Compress(sourceStream, compressionLevel);
    }

    /// <summary>
    /// Получить сжатый массив байт.
    /// </summary>
    /// <param name="source">Несжатый массив.</param>
    /// <param name="index">Позиция в массиве, с которой нужно сжимать.</param>
    /// <param name="count">Количество элементов массива, которые нужно сжимать.</param>
    /// <param name="compressionLevel">Уровень сжатия.</param>
    /// <returns>Сжатый массив байт.</returns>
    public static byte[] Compress(this byte[] source, int index, int count, CompressionLevel compressionLevel)
    {
      if (source == null || source.Length == 0)
        return source;

      using (var sourceStream = new MemoryStream(source, index, count))
        return Compress(sourceStream, compressionLevel);
    }

    /// <summary>
    /// Получить сжатый массив байт.
    /// </summary>
    /// <param name="source">Несжатый массив.</param>
    /// <returns>Сжатый массив байт.</returns>
    public static byte[] Compress(this byte[] source)
    {
      return source.Compress(CompressionLevel.Optimal);
    }

    /// <summary>
    /// Получить сжатый массив байт.
    /// </summary>
    /// <param name="source">Несжатый массив.</param>
    /// <param name="index">Позиция в массиве, с которой нужно сжимать.</param>
    /// <param name="count">Количество элементов массива, которые нужно сжимать.</param>
    /// <returns>Сжатый массив байт.</returns>
    public static byte[] Compress(this byte[] source, int index, int count)
    {
      return source.Compress(index, count, CompressionLevel.Optimal);
    }

    /// <summary>
    /// Распаковать сжатый массив байт.
    /// </summary>
    /// <param name="source">Сжатый массив байт.</param>
    /// <returns>Распакованный массив байт.</returns>
    public static byte[] Decompress(this byte[] source)
    {
      if (source == null || source.Length == 0)
        return source;

      using (var sourceStream = new MemoryStream(source))
      {
        var decompressedStream = sourceStream.Decompress();
        using (var resultStream = new MemoryStream())
        {
          decompressedStream.CopyTo(resultStream);
          resultStream.Position = 0;
          var result = resultStream.ToArray();
          decompressedStream.Dispose();
          return result;
        }
      }
    }

    /// <summary>
    /// Сжать поток в массив байт.
    /// </summary>
    /// <param name="sourceStream">Поток с несжатыми данными.</param>
    /// <param name="compressionLevel">Уровень сжатия.</param>
    /// <returns>Сжатый массив байт.</returns>
    private static byte[] Compress(Stream sourceStream, CompressionLevel compressionLevel)
    {
      using (var compressedStream = sourceStream.Compress(compressionLevel))
        return compressedStream.ToArray();
    }
  }
}
