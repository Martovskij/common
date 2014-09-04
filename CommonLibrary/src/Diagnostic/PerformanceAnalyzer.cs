using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace CommonLibrary.Diagnostics
{
  /// <summary>
  /// Анализатор производительности.
  /// </summary>
  public static class PerformanceAnalyzer
  {
    /// <summary>
    /// Словарь все собранных данных по таймерам. Группируется по имени таймера.
    /// </summary>
    private static ConcurrentDictionary<string, PerformanceCategoryData> dataByTimer = new ConcurrentDictionary<string, PerformanceCategoryData>();

    /// <summary>
    /// Словарь всех запущенных счетчиков.
    /// </summary>
    private static ConcurrentDictionary<string, PerformanceTimer> runningTimers = new ConcurrentDictionary<string, PerformanceTimer>();

    /// <summary>
    /// Запустить счетчик типа PerformanceTimer с автоматическим присваиванием имени.
    /// </summary>
    /// <returns>Экземпляр запущенного счетчика.</returns>
    public static PerformanceTimer Start()
    {
      return Start(Guid.NewGuid().ToString());
    }

    /// <summary>
    /// Запустить счетчик типа PerformanceTimer c явным указанием имени.
    /// </summary>
    /// <param name="name">Имя счетчика.</param>
    /// <returns>Экземпляр запущенного счетчика.</returns>
    public static PerformanceTimer Start(string name)
    {
      // TODO: проверка на существование счетчика?
      PerformanceTimer timer = new PerformanceTimer(name);
      if (!runningTimers.TryAdd(name, timer))
        throw new InvalidOperationException("Can't start a new timer");
      timer.Start();
      return timer;
    }

    /// <summary>
    /// Запустить счетчик, если он не был запущен ранее.
    /// </summary>
    /// <param name="name">Имя счетчика.</param>
    /// <returns>Экземпляр запущенного счетчика.</returns>
    public static PerformanceTimer EnsureStart(string name)
    {
      PerformanceTimer timer;
      if (!runningTimers.TryGetValue(name, out timer))
        return Start(name);
      else
        return timer;
    }

    /// <summary>
    /// Сбросить значение счетчика и учесть в расчетах среднего значения.
    /// </summary>
    /// <param name="timerName">Имя счетчика.</param>
    /// <param name="categoryName">Имя категории профилирования, в которой будет учитываться результат работы счетчика.</param>
    /// <returns>Время выполнения счетчика (ms). Если счетчик не найден или не запущен, то null.</returns>
    public static long? Stop(string timerName, string categoryName)
    {
      PerformanceTimer timer;
      if (!runningTimers.TryGetValue(timerName, out timer))
        return null;
      if (!timer.IsRunning)
        return null;
      if (!runningTimers.TryRemove(timerName, out timer))
        return null;

      timer.Stop();      
      PerformanceCategoryData data = dataByTimer.GetOrAdd(categoryName, category => new PerformanceCategoryData());
      data.Increment(timer.ElapsedTime);
      return timer.ElapsedTime;
    }

    /// <summary>
    /// Принудительно остановить счетчик, даже если он был запущен. В статистике время срабатывания не будет учтено.
    /// </summary>
    /// <param name="timerName">Имя счетчика.</param>        
    public static void EnsureStop(string timerName)
    {
      PerformanceTimer timer;
      if (runningTimers.TryRemove(timerName, out timer))
        timer.Stop();
    }

    /// <summary>
    /// Приостановить все таймеры.
    /// </summary>
    public static void PauseAllTimers()
    {
      foreach (var timer in runningTimers.Values)
        timer.Pause();
    }

    /// <summary>
    /// Возобновить все таймеры.
    /// </summary>
    public static void ResumeAllTimers()
    {
      foreach (var timer in runningTimers.Values)
        timer.Resume();
    }

    /// <summary>
    /// Получить результаты расчета среднего значения по всем счетчикам.
    /// </summary>
    /// <returns>Словарь среднего времени выполнения операций по категориям.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",
      Justification = "В методе идут подсчеты средних значений, неправильно делать его свойством.")]
    public static IDictionary<string, long> GetStatistic()
    {
      var result = new Dictionary<string, long>();
      foreach (var category in dataByTimer.Keys)
        result.Add(category, CalculateStatistic(category));
      return result;
    }

    /// <summary>
    /// Посчитать среднее значение показаний счетчика с указаным именем.
    /// </summary>
    /// <param name="category">Категория профилирования.</param>
    /// <returns>Среднее время работы (ms).</returns>
    private static long CalculateStatistic(string category)
    {
      PerformanceCategoryData data;
      if (!dataByTimer.TryGetValue(category, out data))
        throw new InvalidOperationException(string.Format("Performance timer with name \"{0}\" not found!", category));
      return (long)Math.Round(data.TotalElapsedTime / (double)data.NumberOfCalls);
    }

    /// <summary>
    /// Сводные данные по категории операций.
    /// </summary>
    private class PerformanceCategoryData
    {
      private int numberOfCalls;

      private long totalElapsedTime;

      /// <summary>
      /// Общее время выполнения операций.
      /// </summary>
      public long TotalElapsedTime { get { return this.totalElapsedTime; } }

      /// <summary>
      /// Число операций.
      /// </summary>
      public int NumberOfCalls { get { return this.numberOfCalls; } }

      public void Increment(long elapsedTime)
      {
        Interlocked.Increment(ref this.numberOfCalls);
        Interlocked.Add(ref this.totalElapsedTime, elapsedTime);
      }
    }
  }
}
