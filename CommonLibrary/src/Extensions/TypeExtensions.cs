using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CommonLibrary
{
  /// <summary>
  /// Расширения для работы с Type.
  /// </summary>
  public static class TypeExtensions
  {
    #region Поля

    /// <summary>
    /// Кэш свойств типа/интерфейса.
    /// </summary>
    /// <remarks>
    /// Получить свойства интерфейса включая унаследованные интерфейсы не просто, поэтому кешируем.
    /// </remarks>
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> TypeOrInterfacePropertiesCache = new ConcurrentDictionary<Type, PropertyInfo[]>();

    /// <summary>
    /// Кэш типизированных свойств типа/интерфейса (ключ - пара "тип/интерфейс; тип свойства").
    /// </summary>
    private static readonly ConcurrentDictionary<Tuple<Type, Type>, PropertyInfo[]> TypedPropertiesCache = new ConcurrentDictionary<Tuple<Type, Type>, PropertyInfo[]>();

    #endregion

    #region Методы

    /// <summary>
    /// Получить атрибут, который объявлен на переданном классе или на предках этого класса 
    /// (выбирается атрибут с самого близкого предка).
    /// </summary>
    /// <typeparam name="T">Тип атрибуты.</typeparam>
    /// <param name="type">Класс, на котором надо искать атрибут.</param>
    /// <param name="filter">Дополнительный фильтр на атрибут.</param>
    /// <returns>Найденный атрибут. Null, если нигде в иерархии нет походящих атрибутов.</returns>
    public static T GetMostDerivedAttribute<T>(this Type type, Func<T, bool> filter) where T : Attribute
    {
      var result = type.GetCustomAttributes(typeof(T), false).OfType<T>().SingleOrDefault(filter);
      if (result == null)
        return type.BaseType == typeof(object) ? null : type.BaseType.GetMostDerivedAttribute(filter);
      return result;
    }

    /// <summary>
    /// Возвращает полное имя типа сущности, включая имя сборки, но без указания версии, культуры и токена ключа.
    /// </summary>
    /// <param name="type">Тип.</param>
    /// <returns>Имя типа сущности, включая имя сборки, но без указания версии, культуры и токена ключа.</returns>
    public static string GetFullNameWithAssemblyName(this Type type)
    {
      return string.Format("{0}, {1}", type.FullName, type.Assembly.GetName().Name);
    }

    /// <summary>
    /// Получить информацию о свойстве типа (в том числе, если тип является интерфейсом).
    /// </summary>
    /// <param name="type">Тип.</param>
    /// <param name="propertyName">Имя свойства.</param>
    /// <returns>Объект с информацией о свойстве.</returns>
    public static PropertyInfo GetTypeOrInterfaceProperty(this Type type, string propertyName)
    {
      return type.GetTypeOrInterfaceProperties().FirstOrDefault(property => property.Name == propertyName);
    }

    /// <summary>
    /// Получить информацию о свойствах типа (в том числе, если тип является интерфейсом). 
    /// </summary>
    /// <param name="type">Тип.</param>
    /// <returns>Массив объектов с информацией о свойствах.</returns>
    public static PropertyInfo[] GetTypeOrInterfaceProperties(this Type type)
    {
      return TypeOrInterfacePropertiesCache.GetOrAdd(
        type,
        t => t.GetTypeOrInterfaceProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy));
    }

    /// <summary>
    /// Получить информацию о свойствах типа (в том числе, если тип является интерфейсом). 
    /// </summary>
    /// <param name="type">Тип.</param>
    /// <param name="bindingAttributes">Атрибуты области видимости свойств.</param>
    /// <returns>Массив объектов с информацией о свойствах.</returns>
    public static PropertyInfo[] GetTypeOrInterfaceProperties(this Type type, BindingFlags bindingAttributes)
    {
      if (type.IsInterface)
      {
        var propertyInfos = new List<PropertyInfo>();
        var considered = new List<Type>();
        var queue = new Queue<Type>();
        considered.Add(type);
        queue.Enqueue(type);
        while (queue.Count > 0)
        {
          var subType = queue.Dequeue();
          foreach (var subInterface in subType.GetInterfaces())
          {
            if (considered.Contains(subInterface))
              continue;
            considered.Add(subInterface);
            queue.Enqueue(subInterface);
          }
          var typeProperties = subType.GetProperties(bindingAttributes);
          var newPropertyInfos = typeProperties.Where(x => !propertyInfos.Contains(x));
          propertyInfos.AddRange(newPropertyInfos);
        }
        return propertyInfos.ToArray();
      }
      return type.GetProperties(bindingAttributes);
    }

    /// <summary>
    /// Получить свойства заданного типа.
    /// </summary>
    /// <typeparam name="TProperty">Тип получаемых свойств.</typeparam>
    /// <param name="type">Тип, у которого достаются свойства.</param>
    /// <returns>Свойства.</returns>
    public static IEnumerable<PropertyInfo> GetTypeProperties<TProperty>(this Type type)
      where TProperty : class
    {
      return TypedPropertiesCache.GetOrAdd(
        Tuple.Create(type, typeof(TProperty)),
        key => key.Item1.GetTypeOrInterfaceProperties().Where(p => key.Item2.IsAssignableFrom(p.PropertyType)).ToArray());
    }

    #endregion
  }
}
