using System;
using System.CodeDom.Compiler;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace CommonLibrary
{
  /// <summary>
  /// Обертка для нативных вызовов.
  /// </summary>
  [GeneratedCode("Обертка над WinAPI", "")]
  public static class NativeMethods
  {
    #region Вложенные типы

    /// <summary>
    /// Информация о мерцании. Передается в нативную функцию.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FLASHWINFO
    {
      /// <summary>
      /// Размер структуры.
      /// </summary>
      public uint cbSize;

      /// <summary>
      /// Хендлер окна.
      /// </summary>
      public IntPtr hwnd;

      /// <summary>
      /// Статус мерцания.
      /// </summary>
      public uint dwFlags;

      /// <summary>
      /// Количество повторений мерцания.
      /// </summary>
      public uint uCount;

      /// <summary>
      /// Частота, с которой окно будет мерцать.
      /// </summary>
      public uint dwTimeout;
    }

    public delegate bool EnumWindowsCallback(IntPtr hwnd, int lParam);

    /// <summary>
    /// Содержит информацию об окне.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WindowInfo
    {
      /// <summary>
      /// Размер структуры в байтах. Задаем в конструкторе. 
      /// </summary>
      public uint cbSize;

      /// <summary>
      /// Координаты окна.
      /// </summary>
      public Rect rcWindow;

      /// <summary>
      /// Координаты пользовательской области окна.
      /// </summary>
      public Rect rcClient;

      /// <summary>
      /// Стиль окна http://msdn.microsoft.com/en-us/library/windows/desktop/ms632600(v=vs.85).aspx.
      /// </summary>
      public uint dwStyle;

      /// <summary>
      /// Расширенный стиль окна http://msdn.microsoft.com/en-us/library/windows/desktop/ff700543(v=vs.85).aspx. 
      /// </summary>
      public uint dwExStyle;

      /// <summary>
      /// Статус окна. Если флаг WS_ACTIVECAPTION (0x0001), то окно считается активным. Иначе значение флага 0.
      /// </summary>
      public uint dwWindowStatus;

      /// <summary>
      /// Ширина бордюра окна в пикселях.
      /// </summary>
      public uint cxWindowBorders;

      /// <summary>
      /// Высота бордюра окна в пикселях.
      /// </summary>
      public uint cyWindowBorders;

      /// <summary>
      /// Atom окна. 
      /// </summary>
      public ushort atomWindowType;

      /// <summary>
      /// Версия Windows приложения, которое создало окно. 
      /// </summary>
      public ushort wCreatorVersion;

      public WindowInfo(bool? fill)
        : this()
      {
        this.cbSize = (uint)Marshal.SizeOf(typeof(WindowInfo));
      }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
      public int Left, Top, Right, Bottom;

      public Rect(int left, int top, int right, int bottom)
      {
        this.Left = left;
        this.Top = top;
        this.Right = right;
        this.Bottom = bottom;
      }

      public Rect(System.Drawing.Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) { }

      public int X
      {
        get
        {
          return this.Left;
        }

        set
        {
          this.Right -= this.Left - value;
          this.Left = value;
        }
      }

      public int Y
      {
        get
        {
          return this.Top;
        }

        set
        {
          this.Bottom -= this.Top - value;
          this.Top = value;
        }
      }

      public int Height
      {
        get { return this.Bottom - this.Top; }
        set { this.Bottom = value + this.Top; }
      }

      public int Width
      {
        get { return this.Right - this.Left; }
        set { this.Right = value + this.Left; }
      }

      public Point Location
      {
        get
        {
          return new Point(this.Left, this.Top);
        }

        set
        {
          this.X = value.X;
          this.Y = value.Y;
        }
      }

      public Size Size
      {
        get
        {
          return new Size(this.Width, this.Height);
        }

        set
        {
          this.Width = value.Width;
          this.Height = value.Height;
        }
      }

      public static implicit operator Rectangle(Rect r)
      {
        return new Rectangle(r.Left, r.Top, r.Width, r.Height);
      }

      public static implicit operator Rect(System.Drawing.Rectangle r)
      {
        return new Rect(r);
      }

      public static bool operator ==(Rect r1, Rect r2)
      {
        return r1.Equals(r2);
      }

      public static bool operator !=(Rect r1, Rect r2)
      {
        return !r1.Equals(r2);
      }

      public bool Equals(Rect r)
      {
        return r.Left == this.Left && r.Top == this.Top && r.Right == this.Right && r.Bottom == this.Bottom;
      }

      public override bool Equals(object obj)
      {
        if (obj is Rect)
          return Equals((Rect)obj);
        else if (obj is System.Drawing.Rectangle)
          return Equals(new Rect((System.Drawing.Rectangle)obj));
        return false;
      }

      public override int GetHashCode()
      {
        return ((System.Drawing.Rectangle)this).GetHashCode();
      }

      public override string ToString()
      {
        return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", this.Left, this.Top, this.Right, this.Bottom);
      }
    }

    #endregion

    #region Константы

    /// <summary>
    /// Признак остановки мерцания.
    /// </summary>
    public const uint FLASHW_STOP = 0;

    /// <summary>
    /// Признак необходимости включить мерцание заголовка окна.
    /// </summary>
    public const uint FLASHW_CAPTION = 1;

    /// <summary>
    /// Признак необходимости включить мерцание в панели задач.
    /// </summary>
    public const uint FLASHW_TRAY = 2;

    /// <summary>
    /// Признак необходимости включить мерцание заголовка окна и значка в панели задач.
    /// </summary>
    public const uint FLASHW_ALL = 3;

    /// <summary>
    /// Признак необходимости включить мерцание до явного отключения мерцания.
    /// </summary>
    public const uint FLASHW_TIMER = 4;

    /// <summary>
    /// Признак необходимости включить мерцание до тех пор, пока окно не станет активным.
    /// </summary>
    public const uint FLASHW_TIMERNOFG = 12;

    /// <summary>
    /// Расположить окно позади всех окон.
    /// </summary>
    public const int HWND_BOTTOM = 1;

    /// <summary>
    /// Расположить окно поверх всех окон.
    /// </summary>
    public const int HWND_TOPMOST = -1;

    /// <summary>
    /// Расположить окно выше всех не Topmost окон.
    /// </summary>
    public const int HWND_NOTOPMOST = -2;

    /// <summary>
    /// Не менять размеры окна.
    /// </summary>
    public const int SWP_NOSIZE = 0x0001;

    /// <summary>
    /// Не менять позицию окна по X, Y.
    /// </summary>
    public const int SWP_NOMOVE = 0x0002;

    /// <summary>
    /// Не активировать окно.
    /// </summary>
    public const int SWP_NOACTIVATE = 0x0010;

    /// <summary>
    /// Код сообщения окна о двойном клике левой кнопкой мыши.
    /// </summary>
    public const int WM_LBUTTONDBLCLK = 0x00A3;

    /// <summary>
    /// Код сообщения окна о нажатии левой кнопкой мыши.
    /// </summary>
    public const int WM_LBUTTONDOWN = 0x0201;

    /// <summary>
    /// Код сообщения окна о смене курсора мыши.
    /// </summary>
    public const int WM_SETCURSOR = 0x0020;

    /// <summary>
    /// Заголовок окна.
    /// </summary>
    public const int HTCAPTION = 2;

    /// <summary>
    /// Смещение для параметра стиля окна.
    /// </summary>
    public const int GWL_STYLE = -16;

    /// <summary>
    /// Флажок системной команды максимизации окна.
    /// </summary>
    public const int WS_MAXIMIZEBOX = 0x10000;

    /// <summary>
    /// Флажок системной команды сворачивания окна.
    /// </summary>
    public const int WS_MINIMIZEBOX = 0x20000;

    #endregion

    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

    [DllImport("user32", EntryPoint = "SetWindowPos")]
    public static extern int SetWindowPos(IntPtr hWnd, int hwndInsertAfter, int x, int y, int cx, int cy, int wFlags);

    [DllImport("user32.dll")]
    public static extern bool LockWindowUpdate(IntPtr hWndLock);

    [DllImport("Shlwapi.dll", EntryPoint = "StrFormatByteSizeW", CharSet = CharSet.Unicode)]
    public static extern long StrFormatByteSize(long fileSize, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder buffer, int bufferSize);

    [DllImport("iphlpapi.dll", SetLastError = true)]
    public static extern int GetBestInterface(uint destAddr, out uint bestIfIndex);

    [DllImport("user32.dll")]
    public static extern int EnumWindows(EnumWindowsCallback callPtr, int lPar);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowInfo(IntPtr hwnd, out WindowInfo pwi);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsIconic(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    public static extern uint AttachThreadInput(uint attachTo, uint attachFrom, bool attach);

    [DllImport("kernel32.DLL")]
    public static extern uint GetCurrentThreadId();
  }
}