using System;
using System.Windows;

namespace CommonLibrary.Utils
{
  /// <summary>
  /// Вспомогательный класс для работы со стилями.
  /// </summary>
  public static class StyleUtils
  {
    /// <summary>
    /// Объединить два стиля в один.
    /// </summary>
    /// <param name="targetStyle">Целевой стиль, в который будет выполнено объединение.</param>
    /// <param name="sourceStyle">Стиль-источник, с которым выполняем объединение.</param>
    /// <returns>Объединённый стиль.</returns>
    public static Style Merge(this Style targetStyle, Style sourceStyle)
    {
      if (targetStyle == null)
        throw new ArgumentNullException("targetStyle");
      if (sourceStyle == null)
        throw new ArgumentNullException("sourceStyle");
      
      if (sourceStyle.TargetType != null &&
        (targetStyle.TargetType == null || targetStyle.TargetType.IsAssignableFrom(sourceStyle.TargetType)))
      {
        targetStyle.TargetType = sourceStyle.TargetType;
      }

      if (targetStyle.BasedOn == null)
        targetStyle.BasedOn = sourceStyle.BasedOn;
      else if (sourceStyle.BasedOn != null && targetStyle.BasedOn != sourceStyle.BasedOn)
        Merge(targetStyle, sourceStyle.BasedOn);

      foreach (var trigger in sourceStyle.Triggers)
        targetStyle.Triggers.Add(trigger);
      foreach (var setter in sourceStyle.Setters)
        targetStyle.Setters.Add(setter);
      // Объединяем ключи используемых динамических ресурсов.
      foreach (var key in sourceStyle.Resources.Keys)
        targetStyle.Resources[key] = sourceStyle.Resources[key];

      return targetStyle;
    }
  }
}
