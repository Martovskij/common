using System.Diagnostics;

namespace CommonLibrary.Diagnostics
{
  /// <summary>
  /// Счетчик для подсчета времени выполнения операции.
  /// </summary>
  public sealed class PerformanceTimer
  {
    /// <summary>
    /// Имя таймера.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Замеритель времени.
    /// </summary>
    private Stopwatch stopwatch;

    /// <summary>
    /// Замеренное время (ms).
    /// </summary>
    private long elapsed;

    /// <summary>
    /// Получить замеренное время (ms).
    /// </summary>
    public long ElapsedTime
    {
      get
      {
        return this.stopwatch != null && !this.stopwatch.IsRunning ? this.elapsed : 0;
      }
    }

    /// <summary>
    /// Получить признак того, что таймер запущен.
    /// </summary>
    public bool IsRunning
    {
      get
      {
        return this.stopwatch != null && this.stopwatch.IsRunning;
      }
    }

    /// <summary>
    /// Запустить.
    /// </summary>
    public void Start()
    {
      if (this.stopwatch == null || !this.stopwatch.IsRunning)
        this.stopwatch = Stopwatch.StartNew();
    }

    /// <summary>
    /// Остановить.
    /// </summary>
    public void Stop()
    {
      if (this.stopwatch != null && this.stopwatch.IsRunning)
      {
        this.stopwatch.Stop();
        this.elapsed = this.stopwatch.ElapsedMilliseconds;
      }
    }

    /// <summary>
    /// Приостановить.
    /// </summary>
    public void Pause()
    {
      if (this.stopwatch != null && this.stopwatch.IsRunning)
        this.stopwatch.Stop();
    }

    /// <summary>
    /// Возобновить.
    /// </summary>
    public void Resume()
    {
      if (this.stopwatch != null && !this.stopwatch.IsRunning)
        this.stopwatch.Start();
    }

    /// <summary>
    /// Создает экземпляр счетчика.
    /// </summary>
    /// <param name="name">Имя счетчика.</param>
    public PerformanceTimer(string name)
    {
      this.Name = name;      
    }    
  }
}
