using System.Windows.Media;

namespace CommonLibrary
{
  /// <summary>
  /// Вспомогательные утилиты для работы с цветами.
  /// </summary>
  public static class ColorCommonLibrary
  {
    /// <summary>
    /// Изменить яркость цвета.
    /// </summary>
    /// <param name="color">Цвет, для которого изменяется яркость.</param>
    /// <param name="correctionFactor">Коэффициент изменения яркости. Находится в диапазоне от -1 до +1.
    /// Отрицательное значение уменьшает яркость, положительное - увеличивает.</param>
    /// <returns>Цвет с измененной яркостью.</returns>
    /// <remarks>Взято с http://www.pvladov.com/2012/09/make-color-lighter-or-darker.html. </remarks>
    public static Color ChangeColorBrightness(Color color, float correctionFactor)
    {
      float red = (float)color.R;
      float green = (float)color.G;
      float blue = (float)color.B;

      if (correctionFactor < 0)
      {
        correctionFactor = 1 + correctionFactor;
        red *= correctionFactor;
        green *= correctionFactor;
        blue *= correctionFactor;
      }
      else
      {
        red = ((255 - red) * correctionFactor) + red;
        green = ((255 - green) * correctionFactor) + green;
        blue = ((255 - blue) * correctionFactor) + blue;
      }

      return Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
    }
  }
}