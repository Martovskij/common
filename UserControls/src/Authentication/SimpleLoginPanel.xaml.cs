using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace UserControls.Authentication
{
    /// <summary>
    /// Interaction logic for SimpleLoginPanel.xaml.
    /// </summary>
    public partial class SimpleLoginPanel : UserControl
    {
      public static readonly RoutedEvent AuthenticateEvent = EventManager
        .RegisterRoutedEvent("Authenticate", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SimpleLoginPanel));

      public event RoutedEventHandler Authenticate
      {
        add { AddHandler(AuthenticateEvent, value); }
        remove { RemoveHandler(AuthenticateEvent, value); }
      }

      #region Обработчики событий

      private void OkButtonClickHandler(object sender, RoutedEventArgs e)
      {
        if (string.IsNullOrEmpty(this.LoginEdit.Text))
        {
          this.LoginEdit.Background = new SolidColorBrush(Colors.Red);
          return;
        }
        if (string.IsNullOrEmpty(this.PasswordEdit.Password))
        {
          this.PasswordEdit.Background = new SolidColorBrush(Colors.Red);
          return;
        }
        var eventArgs = new AuthenticationEventArgs(SimpleLoginPanel.AuthenticateEvent)
        {
          Login = this.LoginEdit.Text,
          Password = this.PasswordEdit.Password
        };
        RaiseEvent(eventArgs);
      }

      private void InputPasswordHandler(object sender, TextCompositionEventArgs e)
      {
        ((PasswordBox)sender).Background = new SolidColorBrush(Colors.White);
      }

      private void InputLoginHandler(object sender, TextChangedEventArgs e)
      {
        ((TextBox)sender).Background = new SolidColorBrush(Colors.White);
      }

      #endregion

      public SimpleLoginPanel()
      {
        InitializeComponent();
      }
    }
}
