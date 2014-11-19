using System;
using Ninject;


namespace CommonLibrary.DependencyInversion
{
  /// <summary>
  /// Менеджер управления зависимостями.
  /// </summary>
  public static class DependencyManager
  {
    /// <summary>
    /// Ядро.
    /// </summary>
    private static readonly IKernel kernel = new StandardKernel();

    /// <summary>
    /// Зарегистрировать интерфейс.
    /// </summary>
    /// <typeparam name="TInterface">Интерфейс.</typeparam>
    /// <typeparam name="TImplementer">Реализация.</typeparam>
    public static void Register<TInterface, TImplementer>()
    {
      kernel.Bind<TInterface, TImplementer>();
    }

    /// <summary>
    /// Получить экземпляр типа.
    /// </summary>
    /// <typeparam name="T">Требуемый тип.</typeparam>
    /// <returns>Экземпляр требуемого типа.</returns>
    public static T Resolve<T>()
    {
      return kernel.Get<T>();
    }

    /// <summary>
    /// Попытаться зарезолвить экземпляр типа.
    /// </summary>
    /// <typeparam name="T">Требуемый тип.</typeparam>
    /// <returns>Экземпляр требуемого типа.</returns>
    public static T TryResolve<T>() where T : class 
    {
      try
      {
        return kernel.Get<T>();
      }
      catch (Exception)
      {
        return null;
      }
    }
  }
}
