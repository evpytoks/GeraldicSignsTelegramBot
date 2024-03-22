using System;
using System.Globalization;
using System.Text;
using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;

/// <summary>
/// Работа с CSV файлом, представляющим коллекцию геральдических знаков.
/// </summary>
public class CSVProcessing
{


    /// <summary>
    /// Конвертация данных в файле CSV формата в коллекцию геральдических знаков.
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
            throw new ArgumentNullException("Поток не может быть null.");
        }


        try
        {
            using (StreamReader sr = new StreamReader(fileStream))
            {
                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                    Encoding = Encoding.UTF8
                };
                using (var csv = new CsvReader(sr, csvConfig))
                { 
                    return csv.GetRecords<GeraldicSign>().ToList();
                }
            }
        }
        catch (Exception)
        {
            throw new IOException("Не удалось открыть или распарсить файл.");
        }
    }


    /// <summary>
    /// Метод для записи в файл коллекции геральдических знаков в CSV формате.
    /// </summary>
    /// <param name="signs">Коллекция геральдических знаков</param>
    /// <returns>Поток данных файла.</returns>
    /// <exception cref="ArgumentNullException">Аргументы null.</exception>
    /// <exception cref="IOException">Не удалось записать данные в файл.</exception>
    public FileStream Write(List<GeraldicSign> signs)
    {
        // Проверка на корректность входных данных.
        if (signs == null)
        {
            throw new ArgumentNullException("signs не может быть null.");
        }

        try
        {
            // Подготовка колекции.
            signs.Insert(0, new GeraldicSign("Название символа (знака)",
                "Вид символа (знака)", "Изображение", "Описание",
                "Семантика", "Наименование обладателя свидетельства",
                "Дата внесения официального символа (знака) в реестр",
                "Регистрационный номер", "global_id"));

            // Запись в файл.
            using (StreamWriter sw = new StreamWriter("output"))
            {
                var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ";",
                    Encoding = Encoding.UTF8,
                    ShouldQuote = _ => true,
                    NewLine = ";\r\n"
                };
                using (var csv = new CsvWriter(sw, csvConfig))
                {
                    csv.WriteRecords(signs);
                }
            }


            // Очищаем коллекцию.
            signs.Remove(signs[0]);

            return new FileStream("output", FileMode.Open);
        }
        catch (Exception)
        {
            throw new IOException("Не удалось записать данные в файл.");
        }
    }
}

