using System.Collections.Generic;
using System.Linq;

namespace CommonLibrary
{
  /// <summary>
  /// Парсер гиперссылок.
  /// </summary>
  public class HyperlinkParser
  {
    #region Поля и свойства

    /// <summary>
    /// Валидатор гиперссылок.
    /// </summary>
    public HyperlinkValidator Validator { get; set; }

    /// <summary>
    /// Символы-разделители.
    /// </summary>
    private static readonly char[] SeparatorChars = 
    { 
      ' ', 
      '\t', 
      '\r',
      '\n',
      ',', 
      ';' 
    };

    /// <summary>
    /// Символы, не допустимые в конце URI.
    /// Некоторые символы не вошли в этот список намеренно.
    /// # - Разделитель глав. Используется в документах google при назначении прав, в википедии в оглавлении.
    /// №, / - Пример: http://ru.wikipedia.org/wiki/№.
    /// </summary>
    private static readonly char[] NotAllowedUriEndChars =
    { // Недопустимые по спецификации URI.
      '.', 
      ':',
      '%', 
      '<', 
      '>', 
      '|', 
      '\\', 
      '[', 
      ']', 
      '{', 
      '}', 
      // Допустимые по спецификации URI, но редкоиспользуемые.
      // Преимущества: можно вставлять гиперссылки совместно со знаками пунктуации.
      // Ограничения: невозможно вставить статью о символе из википедии, например: http://ru.wikipedia.org/wiki/!
      '(', 
      ')',
      '!',
      '?',
      '"',
      '\'',
      '`',
      '@',
      '&',
      '~',
      '^'
    }; 

    #endregion

    #region Методы

    /// <summary>
    /// Парсинг текста на куски.
    /// </summary>
    /// <param name="text">Текст.</param>
    /// <returns>Список кусков.</returns>
    public TextChunk[] Parse(string text)
    {
      var result = new List<TextChunk>();
      // Вместо null всегда возвращаем пустой массив. 
      if (string.IsNullOrEmpty(text))
        return result.ToArray();

      int leftIndex = 0;
      TextChunk previousChunk = null; 
      do
      {
        int rightIndex;
        var token = GetNextToken(text, leftIndex, out rightIndex);
        bool isHyperlink = this.Validator.IsValidUriString(token);
        bool isNewLine = token == "\n";
        if (previousChunk != null && !previousChunk.IsHyperlink && !previousChunk.IsNewLine && !isHyperlink && !isNewLine)
          previousChunk.Text = previousChunk.Text + token;
        else
        {
          var chunk = new TextChunk(token, isHyperlink, isNewLine);
          result.Add(chunk);
          previousChunk = chunk;
        }

        leftIndex = rightIndex >= leftIndex ? rightIndex + 1 : text.Length;
      }
      while (leftIndex < text.Length);
      return result.ToArray();
    }

    /// <summary>
    /// Получить следующий токен.
    /// </summary>
    /// <param name="text">Исходный текст документа.</param>
    /// <param name="leftIndex">Лвая граница токена.</param>
    /// <param name="rightIndex">Правая граница токена.</param>
    /// <returns>Содержимое токена.</returns>
    private static string GetNextToken(string text, int leftIndex, out int rightIndex)
    {
      rightIndex = text.IndexOfAny(SeparatorChars, leftIndex);
      // Часть строки до первого разделителя.
      var word = rightIndex >= leftIndex ? text.Substring(leftIndex, rightIndex - leftIndex) : text.Substring(leftIndex, text.Length - leftIndex);

      // Пропускаем спецсимволы.
      if (SkipChars(text, NotAllowedUriEndChars, leftIndex, ref rightIndex, ref word))
        return word;

      // Пропускаем разделители.
      if (SkipChar(text, SeparatorChars, leftIndex, ref rightIndex, ref word))
        return word;

      word = word.TrimEnd(NotAllowedUriEndChars);
      rightIndex = leftIndex + word.Length - 1;
      return word;
    }

    /// <summary>
    /// Пропустить массив символов в строке.
    /// </summary>
    /// <param name="text">Строка.</param>
    /// <param name="skippedChars">Массив символов, которые надо пропуститью.</param>
    /// <param name="leftIndex">Левый индексю.</param>
    /// <param name="rightIndex">Правый индекс.</param>
    /// <param name="skippedString">Подстрока пропущеных символов.</param>
    /// <returns>True, если хотя бы один символ был пропущен.</returns>
    private static bool SkipChars(string text, char[] skippedChars, int leftIndex, ref int rightIndex, ref string skippedString)
    {
      if (skippedChars.Contains(text[leftIndex]))
      {
        int index = leftIndex;
        while (index < text.Length && skippedChars.Contains(text[index]))
          index++;

        skippedString = text.Substring(leftIndex, index - leftIndex);
        rightIndex = index - 1;
        return true;
      }
      return false;
    }

    /// <summary>
    /// Пропустить один символ в строке.
    /// </summary>
    /// <param name="text">Строка.</param>
    /// <param name="skippedChars">Массив символов, которые надо пропуститью.</param>
    /// <param name="leftIndex">Левый индексю.</param>
    /// <param name="rightIndex">Правый индекс.</param>
    /// <param name="skippedString">Подстрока пропущеных символов.</param>
    /// <returns>True, если хотя бы один символ был пропущен.</returns>
    private static bool SkipChar(string text, char[] skippedChars, int leftIndex, ref int rightIndex, ref string skippedString)
    {
      if (skippedChars.Contains(text[leftIndex]))
      {
        skippedString = text.Substring(leftIndex, 1);
        rightIndex = leftIndex;
        return true;
      }
      return false;
    } 

    #endregion

    #region Конструкторы

    public HyperlinkParser(HyperlinkValidator validator)
    {
      this.Validator = validator;
    } 

    #endregion
  }
}
