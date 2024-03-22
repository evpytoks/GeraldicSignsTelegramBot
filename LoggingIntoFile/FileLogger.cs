using Microsoft.Extensions.Logging;


/// <summary>
/// Класс для логироания в файл.
/// </summary>
public class FileLogger : ILogger
{
    string _filePath;
    static object _lock = new object();


    /// <summary>
    /// Конструктор с путём по умолчанию.
    /// </summary>
    public FileLogger()
    {
        _filePath = "logging";
    }


    /// <summary>
    /// Конструктор с передаваемым путём.
    /// </summary>
    /// <param name="path">Передаваемый путь.</param>
    public FileLogger(string path)
    {
        _filePath = path;
    }


    public IDisposable? BeginScope<TState>(TState state)
    {
        return null;
    }


    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }


    /// <summary>
    /// Записывание в файл лога.
    /// </summary>
    /// <param name="logLevel">Тип сообщения.</param>
    /// <param name="eventId">Идентификатор события.</param>
    /// <param name="state">Сохранненное сообщение.</param>
    /// <param name="exception">Информация об исключении.</param>
    /// <param name="formatter">Функция форматирования.</param>
    public void Log<TState>(LogLevel logLevel, EventId eventId,
                TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (formatter == null)
        {
            return;
        }

        lock (_lock)
        {
            File.AppendAllText(_filePath + DateTime.Now.ToString("yyyy-MM-dd")
                + ":" + DateTime.Now.Hour.ToString(), $"{DateTime.Now:mm}:{DateTime.Now:ss} " +
                $"{logLevel} {formatter(state, exception)} {Environment.NewLine}");
        }
    }
}