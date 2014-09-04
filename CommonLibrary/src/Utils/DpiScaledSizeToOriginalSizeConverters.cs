using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CommonLibrary
{
  /// <summary>
  /// Конвертер ширины к значению, позволяющему восстановить оригинальный визаульный размер, не зависящий от системного DPI.
  /// </summary>
  public class DpiScaledWidthToOriginalWidthConverter : IValueConverter
  {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
      Justification = "Класс неизменяемый")]
    public static readonly IValueConverter Instance = new DpiScaledWidthToOriginalWidthConverter();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var result = System.Convert.ToDouble(value) / SystemInfo.DpiXFactor;
      return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var result = System.Convert.ToDouble(value) * SystemInfo.DpiXFactor;
      return result;
    }
  }

  /// <summary>
  /// Конвертер высоты к значению, позволяющему восстановить оригинальный визуальный размер, не зависящий от системного DPI.
  /// </summary>
  public class DpiScaledHeightToOriginalHeightConverter : IValueConverter
  {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
      Justification = "Класс неизменяемый")]
    public static readonly IValueConverter Instance = new DpiScaledHeightToOriginalHeightConverter();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var result = System.Convert.ToDouble(value) / SystemInfo.DpiYFactor;
      return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var result = System.Convert.ToDouble(value) * SystemInfo.DpiYFactor;
      return result;
    }
  }

  /// <summary>
  /// Конвертер толщины к значению, позволяющему восстановить оригинальный визуальный размер, не зависящий от системного DPI.
  /// </summary>
  public class DpiScaledThicknessToOriginalThicknessConverter : IValueConverter
  {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
      Justification = "Класс неизменяемый")]
    public static readonly IValueConverter Instance = new DpiScaledThicknessToOriginalThicknessConverter();    

    /// <summary>
    /// Однопиксельная ширина с учетом DPI.
    /// </summary>
    public static readonly Thickness OnePixel = Convert(new Thickness(1));    

    /// <summary>
    /// Преобразовать.
    /// </summary>
    /// <param name="thickness">Исходная толщина.</param>
    /// <returns>Преобразованная толщина.</returns>
    private static Thickness Convert(Thickness thickness)
    {
      return new Thickness(
        thickness.Left / SystemInfo.DpiXFactor,
        thickness.Top / SystemInfo.DpiYFactor,
        thickness.Right / SystemInfo.DpiXFactor,
        thickness.Bottom / SystemInfo.DpiYFactor);
    }

    /// <summary>
    /// Преобразовать обратно.
    /// </summary>
    /// <param name="thickness">Преобразованная толщина.</param>
    /// <returns>Исходная толщина.</returns>
    private static object ConvertBack(Thickness thickness)
    {
      return new Thickness(
        thickness.Left * SystemInfo.DpiXFactor,
        thickness.Top * SystemInfo.DpiYFactor,
        thickness.Right * SystemInfo.DpiXFactor,
        thickness.Bottom * SystemInfo.DpiYFactor);
    }

    #region IValueConverter

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return Convert((Thickness)value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return ConvertBack((Thickness)value);
    }

    #endregion
  }
}