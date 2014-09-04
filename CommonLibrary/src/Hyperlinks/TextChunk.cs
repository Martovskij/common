namespace CommonLibrary
{
  /// <summary>
  /// Кусок текста.
  /// </summary>
  public class TextChunk
  {
    /// <summary>
    /// Кусок текста является гиперссылкой.
    /// </summary>
    public bool IsHyperlink { get; private set; }

    /// <summary>
    /// Кусок текста является символом перевода строки.
    /// </summary>
    public bool IsNewLine { get; private set; }

    /// <summary>
    /// Текст.
    /// </summary>
    public string Text { get; internal set; }

    public TextChunk(string text, bool isHyperlink, bool isNewLine)
    {
      this.Text = text;
      this.IsHyperlink = isHyperlink;
      this.IsNewLine = isNewLine;
    }
  }
}
