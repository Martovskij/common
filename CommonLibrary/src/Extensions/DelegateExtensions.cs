using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace CommonLibrary
{
  /// <summary>
  /// Расширения для работы с делегатами.
  /// </summary>
  public static class DelegateExtensions
  {
    #region Поля и свойства

    /// <summary>
    /// Кэш полей с событиями в типах.
    /// </summary>
    private static readonly Dictionary<Type, List<FieldInfo>> eventFieldsCache = new Dictionary<Type, List<FieldInfo>>();

    /// <summary>
    /// Все привязки для поиска полей с событиями в типах.
    /// </summary>
    private static readonly BindingFlags AllBindings = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    #endregion

    #region Методы

    /// <summary>
    /// Вернуть делегат с удаленными обработчиками, относящимися к указанному объекту.
    /// </summary>
    /// <param name="originalDelegate">Оригинальный делегат.</param>
    /// <param name="target">Объект, обработчики которого нужно удалить из делегата.</param>
    /// <returns>Делегат с удаленными обработчиками.</returns>
    public static Delegate WithoutTarget(this Delegate originalDelegate, object target)
    {
      if (originalDelegate != null)
      {
        var result = new List<Delegate>();
        var invocationList = originalDelegate.GetInvocationList();
        if (invocationList != null)
          result.AddRange(invocationList.Where(invocation => invocation.Target != target));

        return Delegate.Combine(result.ToArray());
      }
      else
        return null;
    }

    /// <summary>
    /// Удалить у объекта все обработчики заданного события.
    /// </summary>
    /// <typeparam name="T">Тип объекта.</typeparam>
    /// <param name="obj">Экземпляр объекта.</param>
    /// <param name="eventName">Имя события, обработчики которого нужно удалить.</param>
    /// <returns>Делегат, содержащий удалённые обработчики.</returns>
    public static Delegate RemoveEventHandler<T>(this T obj, string eventName)
    {
      if (obj == null)
        return null;

      var objectType = obj.GetType();
      var eventFields = GetEventFields(objectType)
        .Where(e => string.Compare(eventName, e.Name, StringComparison.OrdinalIgnoreCase) == 0);

      foreach (var eventFieldInfo in eventFields)
      {
        if (eventFieldInfo.IsStatic)
        {
          var staticEventHandlers = GetStaticEventHandlers(obj);

          var fieldValue = eventFieldInfo.GetValue(obj);
          var eventHandler = staticEventHandlers[fieldValue];
          if (eventHandler == null)
            continue;

          var handlerDelegates = eventHandler.GetInvocationList();
          if (handlerDelegates == null)
            continue;

          var eventInfo = objectType.GetEvent(eventFieldInfo.Name, AllBindings);
          foreach (var handlerDelegate in handlerDelegates)
            eventInfo.RemoveEventHandler(obj, handlerDelegate);
        }
        else
        {
          var eventInfo = objectType.GetEvent(eventFieldInfo.Name, AllBindings);
          if (eventInfo == null)
            continue;

          var eventHandler = eventFieldInfo.GetValue(obj) as Delegate;
          if (eventHandler == null)
            continue;

          foreach (Delegate handlerDelegate in eventHandler.GetInvocationList())
            eventInfo.RemoveEventHandler(obj, handlerDelegate);

          return eventHandler;
        }
      }

      return null;
    }

    /// <summary>
    /// Добавить к объекту обработчик события в виде делегата.
    /// </summary>
    /// <typeparam name="T">Тип объекта.</typeparam>
    /// <param name="obj">Экземпляр объекта.</param>
    /// <param name="eventName">Имя события.</param>
    /// <param name="eventHandler">Делегат с обработчиком события.</param>
    public static void AddEventHandler<T>(this T obj, string eventName, Delegate eventHandler)
    {
      if (obj == null || eventHandler == null)
        return;

      var objectType = obj.GetType();
      var eventFields = GetEventFields(objectType)
        .Where(e => string.Compare(eventName, e.Name, StringComparison.OrdinalIgnoreCase) == 0);

      foreach (var eventFieldInfo in eventFields)
      {
        var eventInfo = objectType.GetEvent(eventFieldInfo.Name, AllBindings);
        eventInfo.AddEventHandler(obj, eventHandler);
      }
    }

    /// <summary>
    /// Получить у типа поля, содержащие события.
    /// </summary>
    /// <param name="type">Тип.</param>
    /// <returns>Список полей с событиями.</returns>
    private static List<FieldInfo> GetEventFields(Type type)
    {
      List<FieldInfo> eventField;
      if (eventFieldsCache.TryGetValue(type, out eventField))
        return eventField;

      var eventFields = type.GetEvents(AllBindings)
        .Select(ei => ei.DeclaringType.GetField(ei.Name, AllBindings))
        .SkipNull()
        .ToList();
      eventFieldsCache.Add(type, eventFields);
      return eventFields;
    }

    /// <summary>
    /// Получить через экземпляра объекта обработчики статических событий.
    /// </summary>
    /// <typeparam name="T">Тип объекта.</typeparam>
    /// <param name="obj">Экземпляр объекта.</param>
    /// <returns>Обработчики статических событий.</returns>
    private static EventHandlerList GetStaticEventHandlers<T>(T obj)
    {
      var getEventsMethod = obj.GetType().GetMethod("get_Events", AllBindings);
      return (EventHandlerList)getEventsMethod.Invoke(obj, new object[] { });
    }

    #endregion
  }
}