using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UserControls.Authentication
{
  public class AuthenticationEventArgs : RoutedEventArgs
  {
    /// <summary>
    /// Логин.
    /// </summary>
    public string Login { get; set; }

    /// <summary>
    /// Пароль.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ev"></param>
    public AuthenticationEventArgs(RoutedEvent ev) : base(ev) { }
  }
}
