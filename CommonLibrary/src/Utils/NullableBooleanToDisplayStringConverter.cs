using System;
using System.Windows.Data;

namespace CommonLibrary
{
  /// <summary>
  /// Конвертер логического значения в отображаемое строковое.
  /// </summary>
  [ValueConversion(typeof(bool?), typeof(string))]
  public class NullableBooleanToDisplayStringConverter : IValueConverter
  {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
      Justification = "Класс неизменяемый")]
    public static readonly IValueConverter Instance = new NullableBooleanToDisplayStringConverter();

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      var source = value as bool?;
      return (source != null && source.HasValue && source.Value) ? "+" : null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      return Binding.DoNothing;
    }
  }
}