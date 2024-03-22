using Microsoft.Extensions.Logging;

/// <summary>
/// Класс для добавления логгирования в файл.
/// </summary>
public static class FileLoggerExtensions
{
    /// <summary>
    /// Добавление логгирования в файл.
    /// </summary>
    /// <param name="factory">Объект для добавления.</param>
    /// <param name="filePath">Путь к файлу.</param>
    /// <returns>Полученный объект.</returns>
    public static ILoggerFactory AddFile(this ILoggerFactory factory, string filePath)
    {
        factory.AddProvider(new FileLoggerProvider(filePath));
        return factory;
    }
}