using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ninject.Planning.Bindings;
using Ninject;
using CommonLibrary.DependencyInversion;

namespace CommonLibrary.DependencyInversion
{
  /// <summary>
  /// 
  /// </summary>
  public static class DependencyManager
  {
    /// <summary>
    /// 
    /// </summary>
    private static readonly IKernel kernel = new StandardKernel();

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    /// <typeparam name="TImplementer"></typeparam>
    public static void Register<TInterface, TImplementer>()
    {
      kernel.Bind<TInterface, TImplementer>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Resolve<T>()
    {
      return kernel.Get<T>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
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
