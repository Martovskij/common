using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace CommonLibrary
{
  /// <summary>
  /// Хэлпер для обработки строк.
  /// </summary>
  public static class StringUtils
  {
    #region Константы
    
    /// <summary>
    /// Значение, которое добавляется в конец строки при её тримминге.
    /// </summary>
    private const string TrimmingEndValue = "...";

    #endregion

    /// <summary>
    /// Расширенный метод который применяется к строке-шаблону и подставляет в него аргументы.
    /// </summary>
    /// <param name="format">Строка-шаблон.</param>
    /// <param name="args">Аргументы которые нужно подставить в строку.</param>
    /// <returns></returns>
    public static string Parameters(this string format, params object[] args)
    {
      return string.Format(format, args);
    }

    /// <summary>
    /// Закончить строку точкой, если она не оканчивается на какой-либо знак конца предложения.
    /// </summary>
    /// <param name="value">Исходная строка.</param>
    /// <returns>Строка с точкой на конце.</returns>
    public static string EndWithPeriod(this string value)
    {
      if (string.IsNullOrWhiteSpace(value))
        return value;

      const string Period = ".";
      string[] excludeSymbols = new[] { Period, "?", "!", ":" };
      string trimmedValue = value.TrimEnd();
      return excludeSymbols.Any(s => trimmedValue.EndsWith(s, StringComparison.OrdinalIgnoreCase)) ? trimmedValue : trimmedValue + Period;
    }

    /// <summary>
    /// Преобразование строки в SecureString.
    /// </summary>
    /// <param name="value">Строка для преобразования.</param>
    /// <returns>Преобразованная SecureString.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
      Justification = "Нельзя уничтожать строку, поскольку она используется во внешнем по отношению к функции коде.")]
    public static SecureString ToSecureString(this string value)
    {
      SecureString secureStr = new SecureString();
      foreach (char ch in value.ToCharArray())
        secureStr.AppendChar(ch);
      secureStr.MakeReadOnly();
      return secureStr;
    }

    /// <summary>
    /// Преобразование SecureString в небезопасную строку.
    /// </summary>
    /// <param name="value">Безопасная строка.</param>
    /// <returns>Небезопасная строка.</returns>
    public static string ToUnsecuredString(this SecureString value)
    {
      if (value == null)
        throw new ArgumentNullException("value");

      IntPtr unmanagedString = IntPtr.Zero;
      try
      {
        unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(value);
        return Marshal.PtrToStringUni(unmanagedString);
      }
      finally
      {
        Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
      }
    }

    /// <summary>
    /// Вычислить MD5-хеш для строки.
    /// </summary>
    /// <param name="value">Строка.</param>
    /// <returns>Хеш.</returns>
    public static string GetMD5Hash(this string value)
    {
      if (value == null)
        throw new ArgumentNullException("value");
      
      return Encoding.UTF8.GetBytes(value).GetMD5Hash();
    }

    /// <summary>
    /// Вычисление склонения существительного, идущего после числительного.
    /// </summary>
    /// <param name="number">Число.</param>
    /// <param name="nominative">Именительный падеж существительного.</param>
    /// <param name="genitiveSingular">Родительный падеж едиенственного числа существительного.</param>
    /// <param name="genitivePlural">Родительный падеж множественного числа существительного.</param>
    /// <returns>Ceotcndительное в нужной форме.</returns>
    /// <example>
    /// NumberDeclension(1, "час", "часа", "часов") // час.
    /// NumberDeclension(2, "час", "часа", "часов") // часа.
    /// NumberDeclension(5, "час", "часа", "часов") // часов.
    /// </example>
    public static string NumberDeclension(int number, string nominative, string genitiveSingular, string genitivePlural)
    {
      int lastDigit = number % 10;
      int lastTwoDigits = number % 100;
      if (lastDigit == 1 && lastTwoDigits != 11)
        return nominative;

      if ((lastDigit == 2 && lastTwoDigits != 12) || (lastDigit == 3 && lastTwoDigits != 13) || (lastDigit == 4 && lastTwoDigits != 14))
        return genitiveSingular;

      return genitivePlural;
    }

    /// <summary>
    /// Удалить из строки все символы не являющиеся буквами.
    /// </summary>
    /// <param name="value">Строка.</param>
    /// <returns>Строка содержащая только буквы.</returns>
    public static string RemoveNonLetters(this string value)
    {
      if (value == null)
        return value;
      var newString = new char[value.Length];
      int length = 0;
      foreach (var currentChar in value)
        if (char.IsLetter(currentChar))
        {
          newString[length] = currentChar;
          length++;
        }
      return new string(newString, 0, length);
    }

    /// <summary>
    /// Обрезать строку многоточием, если её длина превышает заданное максимальное количество символов.
    /// </summary>
    /// <param name="value">Строка.</param>
    /// <param name="maxLength">Ограничение по количеству символов в строке.</param>
    /// <returns>Строка, ограниченная заданным количеством символов.</returns>
    public static string TrimEnd(this string value, int maxLength)
    {
      string normalizedValue = value.TrimEnd();
      return normalizedValue.Length > maxLength ? normalizedValue.Substring(0, Math.Max(0, maxLength)) + TrimmingEndValue : normalizedValue;
    }
  }
}
