using System;
using System.IO;

namespace CommonLibrary
{
  /// <summary>
  /// Класс-расширение для работы с каталогами.
  /// </summary>
  public static class DirectoryUtils
  {
    #region Методы

    /// <summary>
    /// Удаление пустого каталога с обработкой исключения доступа.
    /// </summary>
    /// <param name="path">Имя каталога, который необходимо удалить.</param>
    public static void TryDeleteDirectory(string path)
    {
      TryDeleteDirectory(path, false);
    }

    /// <summary>
    /// Удаление каталога с обработкой исключения доступа.
    /// </summary>
    /// <param name="path">Имя каталога, который необходимо удалить.</param>
    /// <param name="recursive">Значение true позволяет удалить каталоги, подкаталоги и файлы по заданному path, в противном случае — значение false.</param>
    /// <returns>true, если удаление прошло успешно.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Семантика метода не предполагает выброса исключений.")]
    public static bool TryDeleteDirectory(string path, bool recursive)
    {
      try
      {
        if (recursive)
          RemoveAllFileAttributeInDirectory(path, true);
        Directory.Delete(path, recursive);
        return true;
      }
      catch
      {
        try
        {
          Directory.Delete(path, recursive);
          return true;
        }
        catch
        {
          // Просто гасим исключение.
        }
      }
      return false;
    }

    /// <summary>
    /// Удалить атрибуты у всех файлов в каталоге.
    /// </summary>
    /// <param name="path">Имя каталога.</param>
    /// <param name="recursive">Признак, нужно ли удалять атрибуты файлов во вложенных каталогах.</param>
    private static void RemoveAllFileAttributeInDirectory(string path, bool recursive)
    {
      Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
        .ForEach(file => FileUtils.RemoveFileAttribute(file, FileAttributes.ReadOnly));
      if (recursive)
        Directory.EnumerateDirectories(path).ForEach(sub => RemoveAllFileAttributeInDirectory(sub, recursive));
    }

    /// <summary>
    /// Копирование папки с вложениями.
    /// </summary>
    /// <param name="sourcePath">Путь до источника.</param>
    /// <param name="destinationPath">Путь папки назначения.</param>
    /// <param name="overwrite">Перезаписывать ли файл.</param>
    /// <param name="copyFileCallback">Метод, который вызывается при копировании каждого файла.</param>
    public static void CopyFilesRecursively(string sourcePath, string destinationPath, bool overwrite, Action<string, bool> copyFileCallback)
    {
      foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));

      foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
      {
        string destPath = newPath.Replace(sourcePath, destinationPath);
        bool fileWasExists = File.Exists(destPath);

        File.Copy(newPath, destPath, overwrite);
        if (copyFileCallback != null)
          copyFileCallback(destPath, fileWasExists);
      }
    }

    #endregion
  }
}