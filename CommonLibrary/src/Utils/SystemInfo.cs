using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using CommonLibrary.CommonLibrary;

namespace CommonLibrary
{
  /// <summary>
  /// Информация о системе.
  /// </summary>
  public static class SystemInfo
  {
    /// <summary>
    /// Метод для получения актуальной культуры пользователя (на уровне операционной системы).
    /// </summary>
    private static readonly MethodInfo GetFreshUserDefaultCultureMethod = typeof(CultureInfo).GetMethod("InitUserDefaultCulture", BindingFlags.Static | BindingFlags.NonPublic);

    /// <summary>
    /// Список сведений о системе. Дамп systeminfo.exe.
    /// </summary>
    private static string OSInfoCache { get; set; }

    /// <summary>
    /// Время ожидания завершения процесса.
    /// </summary>
    private static int waitProcessTimeout = 10000;

    /// <summary>
    /// Признак того, что код выполняется на машине с низкой графической производительностью (не поддерживается аппаратное ускорение и т.д.).
    /// </summary>
    public static bool IsLowGraphicPerformance
    {
      get
      {
        return System.Windows.Media.RenderCapability.Tier >> 16 < 2;
      }
    }

    private static Tuple<double, double> dpiFactors;

    /// <summary>
    /// Коэффициенты DPI системы (относительно стандартных 96dpi).
    /// </summary>
    private static Tuple<double, double> DpiFactors
    {
      get
      {
        if (dpiFactors == null)        
          using (var source = new HwndSource(new HwndSourceParameters()))
            dpiFactors = Tuple.Create(source.CompositionTarget.TransformToDevice.M11, source.CompositionTarget.TransformToDevice.M22);        
        return dpiFactors;
      }
    }

    /// <summary>
    /// Коэффициент DPI системы по горизонтали (относительно стандартных 96dpi).
    /// </summary>
    public static double DpiXFactor
    {
      get
      {
        var factors = DpiFactors;
        return factors != null ? factors.Item1 : 1;
      }
    }

    /// <summary>
    /// Коэффициент DPI системы по вертикали (относительно стандартных 96dpi).
    /// </summary>
    public static double DpiYFactor
    {
      get
      {
        var factors = DpiFactors;
        return factors != null ? factors.Item2 : 1;
      }
    }

    /// <summary>
    /// Актуальная культура пользователя (на уровне операционной системы).
    /// </summary>
    public static CultureInfo UserDefaultCulture
    {
      get { return (CultureInfo)GetFreshUserDefaultCultureMethod.ReflectionInvoke(null, null); }
    }

    /// <summary>
    /// Получить сведения о системе.
    /// </summary>
    /// <returns>Строка.</returns>
    private static string GetSystemInfo()
    {
      string info;
      using (var compiler = new Process())
      {
        compiler.StartInfo.FileName = "systeminfo.exe";
        compiler.StartInfo.UseShellExecute = false;
        compiler.StartInfo.CreateNoWindow = true;
        compiler.StartInfo.RedirectStandardOutput = true;
        // Для преобразования символов из cp866. 
        compiler.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(UserDefaultCulture.TextInfo.OEMCodePage);
        compiler.Start();
        info = compiler.StandardOutput.ReadToEnd();
        compiler.WaitForExit(waitProcessTimeout);
      }

      return info;
    }

    /// <summary>
    /// Сведения о системе.
    /// </summary>
    public static string OSInfo
    {
      get
      {
        if (OSInfoCache == null)
          OSInfoCache = GetSystemInfo();
        return OSInfoCache;
      }
    }
  }
}