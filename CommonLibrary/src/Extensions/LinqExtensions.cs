using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CommonLibrary
{
  /// <summary>
  /// Расширения для Linq.
  /// </summary>
  public static class LinqExtensions
  {
    /// <summary>
    /// Добавить в коллекцию перечень элементов.
    /// </summary>
    /// <typeparam name="T">Тип элементов коллекции.</typeparam>
    /// <param name="collection">Коллекция.</param>
    /// <param name="source">Перечень элементов, которые необходимо добавить.</param>
    public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> source)
    {
      if (source == null)
        throw new ArgumentNullException("source");

      foreach (T item in source)
        collection.Add(item);
    }

    /// <summary>
    /// Проверить два перечисления на эквивалентность содержимого с учетом того, что коллекции могут быть Null.
    /// </summary>
    /// <typeparam name="T">Тип элементов коллекции.</typeparam>
    /// <param name="first">Первая коллекция.</param>
    /// <param name="second">Вторя коллекция.</param>
    /// <returns>True, если содержимое коллекций эквивалетно или они обе null. Иначе - false.</returns>
    public static bool SafeSequenceEqual<T>(this IEnumerable<T> first, IEnumerable<T> second)
    {
      if (object.ReferenceEquals(first, second))
        return true;
      if (first == null && second != null)
        return false;
      if (first != null && second == null)
        return false;
      return first.SequenceEqual(second);
    }

    /// <summary>
    /// Проверить два перечисления на эквивалентность содержимого с учетом того, что коллекции могут быть Null.
    /// </summary>
    /// <typeparam name="T">Тип элементов коллекции.</typeparam>
    /// <param name="first">Первая коллекция.</param>
    /// <param name="second">Вторя коллекция.</param>
    /// <param name="comparer">Сравнивальщик элементов.</param>
    /// <returns>True, если содержимое коллекций эквивалетно или они обе null. Иначе - false.</returns>
    public static bool SafeSequenceEqual<T>(this IEnumerable<T> first, IEnumerable<T> second, IEqualityComparer<T> comparer)
    {
      if (object.ReferenceEquals(first, second))
        return true;
      if (first == null || second == null)
        return false;
      return first.SequenceEqual(second, comparer);
    }

    /// <summary>
    /// Проверить два перечисления на эквивалентность содержимого с учетом того, что коллекции могут быть Null.
    /// </summary>
    /// <typeparam name="T">Тип элементов коллекции.</typeparam>
    /// <param name="first">Первая коллекция.</param>
    /// <param name="second">Вторя коллекция.</param>
    /// <returns>True, если содержимое коллекций эквивалетно или они обе null. Иначе - false.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Positionally",
      Justification = "Позиционно - корректный термин.")]
    public static bool SequencePositionallyEquals<T>(this IList<T> first, IList<T> second)
    {
      if (object.ReferenceEquals(first, second))
        return true;
      if (first == null && second != null)
        return false;
      if (first != null && second == null)
        return false;
      if (first.Count != second.Count)
        return false;
      for (int i = 0; i < first.Count; i++)
        if (!object.ReferenceEquals(first[i], second[i]))
          return false;
      return true;
    }

    /// <summary>
    /// Проверить два перечисления на эквивалентность с точным соблюдением одинакового порядка элемента в перечислениях.   
    /// </summary>
    /// <typeparam name="T">Тип элемента коллекции.</typeparam>
    /// <param name="first">Первая коллекция.</param>
    /// <param name="second">Вторая коллекция.</param>
    /// <param name="comparer">Функция для сравнения элементов, например, object.Equals или object.ReferenceEquals.</param>
    /// <returns>True, если содержимое коллекций эквивалетно или они обе null. Иначе - false.</returns>
    public static bool SequenceEqualsWithExactOrder<T>(this IEnumerable<T> first, IEnumerable<T> second,
      Func<T, T, bool> comparer)
    {
      if (object.ReferenceEquals(first, second))
        return true;
      if (first == null && second != null)
        return false;
      if (first != null && second == null)
        return false;
      var sourceEnumerator = first.GetEnumerator();
      var targetEnumerator = second.GetEnumerator();
      while (sourceEnumerator.MoveNext())
      {
        if (!targetEnumerator.MoveNext())
          return false;
        if (!comparer(sourceEnumerator.Current, targetEnumerator.Current))
          return false;
      }
      return true;
    }

    /// <summary>
    /// Проверить два массива на эквивалентность содержимого с учетом того, что коллекции могут быть Null.
    /// </summary>
    /// <typeparam name="T">Тип элементов массива.</typeparam>
    /// <param name="first">Первая массив.</param>
    /// <param name="second">Вторая массив.</param>
    /// <returns>True, если содержимое массивов эквивалетно или они оба null. Иначе - false.</returns>
    public static bool SafeSequenceEqual<T>(this T[] first, T[] second)
    {
      if (object.ReferenceEquals(first, second))
        return true;
      if (first == null && second != null)
        return false;
      if (first != null && second == null)
        return false;
      return first.SequenceEqual(second);
    }

    /// <summary>
    /// Выполнить действие для каждого элемента перечня.
    /// </summary>
    /// <typeparam name="T">Тип элементов перечня.</typeparam>
    /// <param name="collection">Перечень элементов.</param>
    /// <param name="action">Действие.</param>
    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
    {
      if (action == null)
        throw new ArgumentNullException("action");

      foreach (T item in collection)
        action(item);
    }

    /// <summary>
    /// Получить перечислитель объектов из источника, используя порционное чтение.
    /// </summary>
    /// <typeparam name="T">Тип объектов в источнике.</typeparam>
    /// <typeparam name="TValue">Тип коллекции ключей, по которым будет выполняться выборка объектов.</typeparam>
    /// <param name="source">Источник данных.</param>
    /// <param name="selector">Селектор - лямбда выражение, которое определяет ключ, по которому будет выполняться поиск объектов в источнике.</param>
    /// <param name="blockSize">Размер блока данных. Если получение данных выполняется с SQL сервера, то значение не должно превышать 2100.</param>
    /// <param name="values">Список всех ключей объектов, которые надо поблочно получить из источника данных.</param>
    /// <returns>Перечислитель объектов.</returns>
    /// <remarks>Если в списке ключей есть дубли, то в итоговом результате могут также присутствовать дубли.</remarks>
    public static IEnumerable<T> ByParts<T, TValue>(
            this IQueryable<T> source,
            Expression<Func<T, TValue>> selector,
            int blockSize,
            IEnumerable<TValue> values)
    {
      const string MethodName = "Contains";
      MethodInfo methodTemplate = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
        .FirstOrDefault(m => m.Name.Equals(MethodName, StringComparison.OrdinalIgnoreCase)
          && m.IsGenericMethodDefinition
          && m.GetParameters().Length == 2);
      if (methodTemplate == null)
        throw new InvalidOperationException("unknown");

      MethodInfo method = methodTemplate.MakeGenericMethod(typeof(TValue));
      foreach (var block in values.SplitPages(blockSize))
      {
        var row = Expression.Parameter(typeof(T), "row");
        var member = Expression.Invoke(selector, row);
        var keys = Expression.Constant(block.ToList(), typeof(IEnumerable<TValue>));
        var predicate = Expression.Call(method, keys, member);
        var lambda = Expression.Lambda<Func<T, bool>>(
              predicate, row);
        foreach (T record in source.Where(lambda))
          yield return record;
      }
    }

    /// <summary>
    /// Разбить последовательность на страницы.
    /// </summary>
    /// <typeparam name="T">Тип элементов последовательности.</typeparam>
    /// <param name="source">Последовательность.</param>
    /// <param name="pageSize">Размер страницы.</param>
    /// <returns>Набор страниц.</returns>
    public static IEnumerable<IEnumerable<T>> SplitPages<T>(this IEnumerable<T> source, int pageSize)
    {
      while (source.Any())
      {
        yield return source.Take(pageSize);
        source = source.Skip(pageSize);
      }
    }

    /// <summary>
    /// Получить свойства заданного типа, из типов других свойств.
    /// </summary>
    /// <typeparam name="TProperty">Тип получаемых свойств.</typeparam>
    /// <param name="properties">Свойства, из которых взять типы.</param>
    /// <returns>Пары - свойство переданное, свойство полученное из переданного типа.</returns>
    public static IEnumerable<KeyValuePair<PropertyInfo, PropertyInfo>> GetPropertyTypeProperties<TProperty>(this IEnumerable<PropertyInfo> properties)
      where TProperty : class
    {
      return properties.SelectMany(p => p.PropertyType
        .GetTypeProperties<TProperty>()
        .Select(childProperty => new KeyValuePair<PropertyInfo, PropertyInfo>(p, childProperty)));
    }

    /// <summary>
    /// Проверить наличия атрибута у члена класса.
    /// </summary>
    /// <typeparam name="T">Тип атрибута.</typeparam>
    /// <param name="memberInfo">Член класса.</param>
    /// <returns>true - атрибут задан.</returns>
    public static bool HasAttribute<T>(this MemberInfo memberInfo)
      where T : System.Attribute
    {
      return memberInfo.GetCustomAttributes(typeof(T), true).Any();
    }

    /// <summary>
    /// Отбросить null значения из коллекции.
    /// </summary>
    /// <typeparam name="T">Тип элементов коллекции.</typeparam>
    /// <param name="collection">Коллекция.</param>
    /// <returns>Коллекция без null значений.</returns>
    public static IEnumerable<T> SkipNull<T>(this IEnumerable<T> collection)
      where T : class
    {
      return collection.Where(value => value != null);
    }
  }
}
