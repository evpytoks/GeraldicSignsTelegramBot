using Microsoft.Extensions.Logging;


/// <summary>
/// Класс для передачи логгера.
/// </summary>
public class FileLoggerProvider : ILoggerProvider
{
    string _path;


    /// <summary>
    /// Конструктор с путём по умолчанию.
    /// </summary>
    public FileLoggerProvider()
    {
        _path = "logging";
    }


    /// <summary>
    /// Конструктор с передаваемым путём.
    /// </summary>
    /// <param name="path">Передаваемый путь.</param>
    public FileLoggerProvider(string path)
    {
        _path = path;
    }

    /// <summary>
    /// Передача логгера.
    /// </summary>
    /// <returns>Передаваемый логгер.</returns>
    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(_path);
    }

    public void Dispose() { }
}