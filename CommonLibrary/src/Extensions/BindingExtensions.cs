using System.Reflection;
using System.Windows;
using System.Windows.Data;

namespace CommonLibrary
{
  /// <summary>
  /// Расширения для биндингов.
  /// </summary>
  public static class BindingExtensions
  {
    /// <summary>
    /// Reflection-ссылка на internal-свойство NeedsUpdate типа BindingExpressionBase.
    /// </summary>
    private static readonly PropertyInfo needsUpdateProperty = typeof(BindingExpressionBase).GetProperty("NeedsUpdate", BindingFlags.Instance | BindingFlags.NonPublic);

    /// <summary>
    /// Reflection-ссылка на private-поле признака асинхронного биндинга.
    /// </summary>
    private static readonly FieldInfo isAsyncField = typeof(Binding).GetField("_isAsync", BindingFlags.Instance | BindingFlags.NonPublic);

    /// <summary>
    /// Вычислить target-значение биндинга.
    /// </summary>
    /// <typeparam name="T">Тип результата.</typeparam>
    /// <param name="binding">Объект биндинга.</param>
    /// <returns>Результат вычисления.</returns>
    public static T Evaluate<T>(this BindingBase binding)
    {
      var evaluator = new BindingEvaluator();
      BindingOperations.SetBinding(evaluator, BindingEvaluator.TargetProperty, binding);
      return (T)evaluator.Target;
    }

    /// <summary>
    /// Установить свойство SetNeedsUpdate объекта BindingExpressionBase.
    /// </summary>
    /// <param name="bindingExpression">The binding expression.</param>
    /// <param name="value">Значение свойства SetNeedsUpdate.</param>
    public static void SetNeedsUpdate(this BindingExpressionBase bindingExpression, bool value)
    {
      needsUpdateProperty.SetReflectionPropertyValue(bindingExpression, value);
    }

    /// <summary>
    /// Обновить источник биндинга синхронно.
    /// </summary>
    /// <param name="bindingExpression">The binding expression.</param>
    public static void UpdateSourceSync(this BindingExpression bindingExpression)
    {
      var parentBinding = bindingExpression.ParentBinding;
      if (parentBinding == null)
        return;

      if (!parentBinding.IsAsync)
        bindingExpression.UpdateSource();
      else
      {
        isAsyncField.SetValue(parentBinding, false);
        try
        {
          bindingExpression.UpdateSource();
        }
        finally
        {
          isAsyncField.SetValue(parentBinding, true);
        }
      }
    }
  }

  /// <summary>
  /// Вспомогательный класс для вычисления значения биндинга.
  /// </summary>
  /// <remarks>
  /// Источник: http://stackoverflow.com/questions/877171/is-there-a-way-to-get-a-property-value-of-an-object-using-propertypath-class.
  /// </remarks>
  internal class BindingEvaluator : DependencyObject
  {
    public static readonly DependencyProperty TargetProperty = DependencyProperty.Register("Target", typeof(object), typeof(BindingEvaluator));

    public object Target
    {
      get { return GetValue(TargetProperty); }
      set { SetValue(TargetProperty, value); }
    }
  }
}