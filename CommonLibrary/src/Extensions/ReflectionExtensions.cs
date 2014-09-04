using System;
using System.IO;
using System.Reflection;

namespace CommonLibrary
{
  /// <summary>
  /// Расширения для работы с reflection.
  /// </summary>
  public static class ReflectionExtensions
  {
    #region Поля и свойства

    /// <summary>
    /// Информация о методе для сохранения оригинального стека исключений.
    /// </summary>
    private static readonly MethodInfo ExceptionInternalPreserveStackTrace =
      typeof(Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);

    #endregion

    #region Методы

    /// <summary>
    /// Получить значение составного (вложенного) свойства.
    /// </summary>
    /// <typeparam name="T">Тип значения составного (вложенного) свойства.</typeparam>
    /// <param name="source">Исходный объект.</param>
    /// <param name="compositePropertyName">Имя составного свойства (части разделены точкой).</param>
    /// <param name="compositePropertyValue">Значение составного (вложенного) свойства.</param>
    /// <returns>Признак того, что значение удалось получить.</returns>
    /// <remarks>Если что-то не так (в середине получился null или не нашлось свойство), возвращается false.</remarks>
    public static bool TryGetCompositePropertyValue<T>(this object source, string compositePropertyName, out T compositePropertyValue)
    {
      if (source == null)
        throw new ArgumentNullException("source");

      if (compositePropertyName == null)
        throw new ArgumentNullException("compositePropertyName");

      compositePropertyValue = default(T);

      object currValue = source;
      foreach (string propertyName in compositePropertyName.Split('.'))
      {
        if (currValue == null)
          return false;

        PropertyInfo property = currValue.GetType().GetMostSpecificProperty(propertyName);
        if (property == null)
          return false;

        currValue = property.GetReflectionPropertyValue(currValue);
      }

      if (currValue is T)
      {
        compositePropertyValue = (T)currValue;
        return true;
      }
      else
        return false;
    }

    /// <summary>
    /// Получить наиболее конкретное публичное свойство с заданным именем (при использовании в потомках одноименного свойства с new).
    /// </summary>
    /// <param name="objectType">Тип объекта.</param>
    /// <param name="propertyName">Имя свойства.</param>
    /// <returns>Свойство.</returns>
    public static PropertyInfo GetMostSpecificProperty(this Type objectType, string propertyName)
    {
      return objectType.GetMostSpecificProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
    }

    /// <summary>
    /// Получить наиболее конкретное свойство с заданным именем (при использовании в потомках одноименного свойства с new).
    /// </summary>
    /// <param name="objectType">Тип объекта.</param>
    /// <param name="propertyName">Имя свойства.</param>
    /// <param name="bindingAttributes">Атрибуты поиска свойства.</param>
    /// <returns>Свойство.</returns>
    public static PropertyInfo GetMostSpecificProperty(this Type objectType, string propertyName, BindingFlags bindingAttributes)
    {
      PropertyInfo result = null;
      Type currentType = objectType;

      while (currentType != null)
      {
        result = currentType.GetProperty(propertyName, bindingAttributes | BindingFlags.DeclaredOnly);
        if (result != null)
          break;

        currentType = currentType.BaseType;
      }

      return result;
    }

    /// <summary>
    /// Получить значение свойства через Reflection с сохранением оригинального стека исключения.
    /// </summary>
    /// <param name="propertyInfo">Информация о свойстве.</param>
    /// <param name="target">Объект, для которого получаем значение свойства или null, если свойство статическое.</param>
    /// <returns>Значение свойства.</returns>
    public static object GetReflectionPropertyValue(this PropertyInfo propertyInfo, object target)
    {
      return ExecuteWithPreservedStackTrace(() => propertyInfo.GetValue(target, null));
    }

    /// <summary>
    /// Получить значение свойства через Reflection с сохранением оригинального стека исключения.
    /// </summary>
    /// <param name="propertyInfo">Информация о свойстве.</param>
    /// <param name="target">Объект, для которого получаем значение свойства или null, если свойство статическое.</param>
    /// <param name="index">Значение индекса для свойства-индексатора.</param>
    /// <returns>Значение свойства.</returns>
    public static object GetReflectionPropertyValue(this PropertyInfo propertyInfo, object target, object[] index)
    {
      return ExecuteWithPreservedStackTrace(() => propertyInfo.GetValue(target, index));
    }

    /// <summary>
    /// Получить значение поля через Reflection с сохранением оригинального стека исключения.
    /// </summary>
    /// <param name="fieldInfo">Информация о поле.</param>
    /// <param name="target">Объект, для которого получаем значение поля или null, если поле статическое.</param>
    /// <returns>Значение поля.</returns>
    public static object GetReflectionFieldValue(this FieldInfo fieldInfo, object target)
    {
      return ExecuteWithPreservedStackTrace(() => fieldInfo.GetValue(target));
    }

    /// <summary>
    /// Задать значение свойства через Reflection с сохранением оригинального стека исключения.
    /// </summary>
    /// <param name="propertyInfo">Информация о свойстве.</param>
    /// <param name="target">Объект, для свойства которого задается значение или null, если свойство статическое.</param>
    /// <param name="value">Значение свойства.</param>
    public static void SetReflectionPropertyValue(this PropertyInfo propertyInfo, object target, object value)
    {
      ExecuteWithPreservedStackTrace(() => propertyInfo.SetValue(target, value, null));
    }

    /// <summary>
    /// Задать значение свойства через Reflection с сохранением оригинального стека исключения.
    /// </summary>
    /// <param name="propertyInfo">Информация о свойстве.</param>
    /// <param name="target">Объект, для свойства которого задается значение или null, если свойство статическое.</param>
    /// <param name="value">Значение свойства.</param>
    /// <param name="index">Значение индекса для свойства-индексатора.</param>
    public static void SetReflectionPropertyValue(this PropertyInfo propertyInfo, object target, object value, object[] index)
    {
      ExecuteWithPreservedStackTrace(() => propertyInfo.SetValue(target, value, index));
    }

    /// <summary>
    /// Задать значение поля через Reflection с сохранением оригинального стека исключения.
    /// </summary>
    /// <param name="fieldInfo">Информация о поле.</param>
    /// <param name="target">Объект, для поля которого задается значение или null, если поле статическое.</param>
    /// <param name="value">Значение поля.</param>
    public static void SetReflectionFieldValue(this FieldInfo fieldInfo, object target, object value)
    {
      ExecuteWithPreservedStackTrace(() => fieldInfo.SetValue(target, value));
    }

    /// <summary>
    /// Вызвать метод через Reflection с сохранением оригинального стека исключения.
    /// </summary>
    /// <param name="methodInfo">Информация о методе.</param>
    /// <param name="target">Объект, метод которого вызывается или null, если метод статический.</param>
    /// <param name="args">Параметры метода.</param>
    /// <returns>Результат выполнения метода.</returns>
    public static object ReflectionInvoke(this MethodInfo methodInfo, object target, object[] args)
    {
      return ExecuteWithPreservedStackTrace(() => methodInfo.Invoke(target, args));
    }

    /// <summary>
    /// Вызвать конструктор через reflection с получением оригинального стека исключения.
    /// </summary>
    /// <param name="constructorInfo">Информация о конструкторе.</param>
    /// <param name="args">Параметры конструктора.</param>
    /// <returns>Созданный конструктором объект.</returns>
    public static object ReflectionInvoke(this ConstructorInfo constructorInfo, object[] args)
    {
      return ExecuteWithPreservedStackTrace(() => constructorInfo.Invoke(args));
    }

    /// <summary>
    /// Выполнить функцию с сохранением оригинального стека исключения.
    /// </summary>
    /// <param name="func">Функция.</param>
    /// <returns>Значение функции.</returns>
    public static object ExecuteWithPreservedStackTrace(Func<object> func)
    {
      try
      {
        return func.Invoke();
      }
      catch (TargetInvocationException ex)
      {
        PreserveStackTrace(ex.InnerException);
        throw ex.InnerException;
      }
    }

    /// <summary>
    /// Выполнить метод с сохранением оригинального стека исключения.
    /// </summary>
    /// <param name="action">Метод.</param>
    public static void ExecuteWithPreservedStackTrace(Action action)
    {
      try
      {
        action.Invoke();
      }
      catch (TargetInvocationException ex)
      {
        PreserveStackTrace(ex.InnerException);
        throw ex.InnerException;
      }
    }

    /// <summary>
    /// Сохранить оригинальный стек исключения.
    /// </summary>
    /// <param name="exception">Исключение.</param>
    private static void PreserveStackTrace(Exception exception)
    {
      ExceptionInternalPreserveStackTrace.Invoke(exception, null);
    }

    #endregion
  }
}
