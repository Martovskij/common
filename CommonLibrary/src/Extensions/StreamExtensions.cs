using System.IO;
using System.IO.Compression;

namespace CommonLibrary
{
  /// <summary>
  /// Методы расширения для работы с потоковыми данными.
  /// </summary>
  public static class StreamExtensions
  {
    /// <summary>
    /// Получить сжатый поток.
    /// </summary>
    /// <param name="originalStream">Несжатый оригинальный поток.</param>
    /// <param name="compressionLevel">Уровень сжатия.</param>
    /// <returns>Сжатый поток.</returns>
    /// <remarks>Оригинальный поток может быть закрыт после выполнения метода, т.к. уже полностью прочитан в процессе сжатия.</remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Результирующий поток будет использован и закрыт в вызывающем коде.")]
    public static MemoryStream Compress(this Stream originalStream, CompressionLevel compressionLevel)
    {
      if (originalStream.CanSeek)
        originalStream.Position = 0;

      var result = new MemoryStream();
      var zipStream = new DeflateStream(result, compressionLevel, true);
      originalStream.CopyTo(zipStream);
      zipStream.Flush();
      zipStream.Close();
      result.Position = 0;
      return result;
    }

    /// <summary>
    /// Получить сжатый поток.
    /// </summary>
    /// <param name="originalStream">Несжатый оригинальный поток.</param>
    /// <returns>Сжатый поток.</returns>
    public static MemoryStream Compress(this Stream originalStream)
    {
      return originalStream.Compress(CompressionLevel.Optimal);
    }

    /// <summary>
    /// Распаковать сжатый поток.
    /// </summary>
    /// <param name="compressedStream">Сжатый поток.</param>
    /// <returns>Поток, предоставляющий доступ к распакованным данным.</returns>
    /// <remarks>Возвращается обертка, сжатый поток сразу после выполнения метода закрывать нельзя.</remarks>
    public static Stream Decompress(this Stream compressedStream)
    {
      if (compressedStream.CanSeek)
        compressedStream.Position = 0;
      return new DeflateStream(compressedStream, CompressionMode.Decompress);
    }
  }
}
