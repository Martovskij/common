using System;
using System.IO;
using System.Linq;
using Common.Logging;

namespace CommonLibrary
{
  /// <summary>
  /// Класс-расширение для работы с файлами.
  /// </summary>
  public static class FileCommonLibrary
  {
    #region

    /// <summary>
    /// Символ, на который заменяются все недопустимые символы в имени файла.
    /// </summary>
    private const string InvalidCharInFileNameReplacement = "_";

    #endregion

    #region Поля и свойства

    /// <summary>
    /// Зарезервированные имена в Windows.
    /// </summary>
    private static readonly string[] reservedWords = new[]
    {
      "CON", "PRN", "AUX", "NUL", 
      "COM0", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
      "LPT0", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
    };

    private static readonly ILog log = LogManager.GetCurrentClassLogger();
    
    #endregion

    #region Методы

    /// <summary>
    /// Удаление файла с обработкой исключения доступа к файлу.
    /// </summary>
    /// <param name="fileName">Путь до файла.</param>
    /// <returns>true, если удаление файла прошло успешно.</returns>
    public static bool TryDeleteFile(string fileName)
    {
      return TryDeleteFile(fileName, false);
    }

    /// <summary>
    /// Удаление файла с игнорированием любых исключений при доступе к файлу.
    /// </summary>
    /// <param name="fileName">Путь до файла.</param>
    /// <param name="silentMode">Тихий режим без вывода сообщений в лог.</param>
    /// <returns>true, если удаление файла прошло успешно.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
      Justification = "Метод Delete генерирует исключений разных классов, нам нужно перехватить все.")]
    public static bool TryDeleteFile(string fileName, bool silentMode)
    {
      try
      {
        // Если файла нет - то нет смысла пытаться его удалять.
        if (!File.Exists(fileName))
          return true;
        RemoveFileAttribute(fileName, FileAttributes.ReadOnly);
        File.Delete(fileName);
        return true;
      }
      catch (Exception)
      {
        try
        {
          File.Delete(fileName);
          return true;
        }
        catch (Exception ex)
        {
          if (!silentMode)
            log.ErrorFormat("Can't delete file '{0}': occurs exception", ex, fileName);
        }
      }
      return false;
    }

    /// <summary>
    /// Установить атрибут файла.
    /// </summary>
    /// <param name="filePath">Путь к файлу.</param>
    /// <param name="attributes">Атрибут, который необходимо установить.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Перехват исключений в данном случае необходим.")]
    public static void SetFileAttribute(string filePath, FileAttributes attributes)
    {
      try
      {
        File.SetAttributes(filePath, File.GetAttributes(filePath) | attributes);
      }
      catch (Exception ex)
      {
        log.Error(ex.Message, ex);
      }
    }

    /// <summary>
    /// Убрать атрибут файла.
    /// </summary>
    /// <param name="filePath">Путь к файлу.</param>
    /// <param name="attributesToRemove">Атрибут, который необходимо удалить.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Перехват исключений в данном случае необходим.")]
    public static void RemoveFileAttribute(string filePath, FileAttributes attributesToRemove)
    {
      try
      {
        File.SetAttributes(filePath, File.GetAttributes(filePath) & ~attributesToRemove);
      }
      catch (Exception ex)
      {
        log.Error(ex.Message, ex);
      }
    }
    
    /// <summary>
    /// Очищает некорректные символы в имени файла.
    /// </summary>
    /// <param name="fileName">Имя файла.</param>
    /// <returns>Корректное для файловой системы имя файла.</returns>
    public static string NormalizeFileName(string fileName)
    {
      // Вырежем некорретные символы и проверим на совпадение с зарезервированными именами.
      // Источник: http://stackoverflow.com/questions/309485/c-sharp-sanitize-file-name
      var invalidFileNameChars = Path.GetInvalidFileNameChars();
      var validFileName = string.Join(InvalidCharInFileNameReplacement, fileName.Split(invalidFileNameChars, StringSplitOptions.RemoveEmptyEntries));
      if (reservedWords.Any(w => string.Equals(w, validFileName, StringComparison.OrdinalIgnoreCase)))
        validFileName = string.Format("{0}{1}", validFileName, InvalidCharInFileNameReplacement);
      return validFileName;
    }

    /// <summary>
    /// Возвращает полное уникальное имя файла на основе пути к папке и имени файла. Если файл занят, то добавляет цифру в конце.
    /// </summary>
    /// <param name="directoryPath">Папка.</param>
    /// <param name="name">Имя.</param>
    /// <param name="useExistingFile">Использовать существующий файл.</param>
    /// <returns>Полное имя файла.</returns>
    public static string GetUniqueFilePath(string directoryPath, string name, bool useExistingFile)
    {
      int attemptsCount = 1;
      // Максимальное количество попыток. На случай зависания.
      int maxAttemptsCount = 100000;
      if (string.IsNullOrWhiteSpace(name))
        name = Guid.NewGuid().ToString().Substring(0, 8);
      var normalizedFileName = NormalizeFileName(name);
      var fileExt = Path.GetExtension(normalizedFileName);
      var fileName = Path.GetFileNameWithoutExtension(normalizedFileName);
      var filePath = Path.Combine(directoryPath, normalizedFileName);
      while (attemptsCount < maxAttemptsCount)
      {
        if (useExistingFile)
          TryDeleteFile(filePath, true);
        if (!File.Exists(filePath))
          return filePath;
        filePath = Path.Combine(directoryPath, string.Format("{0}({1}){2}", fileName, attemptsCount, fileExt));
        attemptsCount++;
      }
      return null;
    }

    #endregion
  }
}
