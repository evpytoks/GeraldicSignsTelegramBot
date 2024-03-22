using System;
using CsvHelper;
using System.Globalization;
using System.Text.Json;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;

/// <summary>
/// Работа с JSON файлом, представляющим коллекцию геральдических знаков.
/// </summary>
public class JSONProcessing
{
    /// <summary>
    /// Конвертация данных в файле JSON формата в коллекцию геральдических знаков.
    /// </summary>
    /// <param name="fileStream">Поток файла.</param>
    /// <returns>Конвертированная коллекция геральдических знаков.</returns>
    /// <exception cref="IOException">Не удалось открыть или распарсить файл.</exception>
    /// <exception cref="ArgumentNullException">Входные аргументы null.</exception>
    public List<GeraldicSign> Read(Stream fileStream)
    {
        // Проверка входных данных на корректность.
        if (fileStream == null)
        {
            throw new ArgumentNullException("fileStream не может быть null.");
        }


        try
        {
            string fileStr;
            using (StreamReader sr = new StreamReader(fileStream))
            {
                fileStr = sr.ReadToEnd();
            }
            return JsonSerializer.Deserialize<List<GeraldicSign>>(fileStr);

        }
        catch (Exception)
        {
            throw new IOException("Не удалось открыть или распарсить файл.");
        }
    }


    /// <summary>
    /// Метод для записи в файл коллекции геральдических знаков в JSON формате.
    /// </summary>
    /// <param name="signs">Коллекция геральдических знаков.</param>
    /// <returns>Поток данных файла.</returns>
    /// <exception cref="ArgumentNullException">Аргументы null.</exception>
    /// <exception cref="IOException">Не удалось записать файл.</exception>
    public Stream Write(List<GeraldicSign> signs)
    {
        // Проверка на корректность входных данных.
        if (signs == null)
        {
            throw new ArgumentNullException("signs не может быть null.");
        }


        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic, UnicodeRanges.LetterlikeSymbols)
            };
            string fileStr = JsonSerializer.Serialize<List<GeraldicSign>>(signs, options);

            // Запись в файл.
            using (StreamWriter sw = new StreamWriter("output")) 
            {
                sw.Write(fileStr);
            }

            return new FileStream("output", FileMode.Open);
        }
        catch (Exception)
        {
            throw new IOException("Не удалось записать данные в файл.");
        }
    }
}


