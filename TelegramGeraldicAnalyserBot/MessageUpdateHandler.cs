using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


/// <summary>
/// Класс для реагирования на обновления вида "новое сообщение".
/// </summary>
public class MessageUpdateHandler
{
    private ITelegramBotClient _botClient;
    private Dictionary<long, string> _stages;
    private Dictionary<long, GeraldicSigns> _signs;
    private CancellationToken _cancellationToken;
    private User? _user;
    private Chat? _chat;
    private Message? _message;
    private ILogger _logger;


    /// <summary>
    /// Конструктор бота по умолчанию с неизвестным сообщением и его местонахождением.
    /// </summary>
    public MessageUpdateHandler()
	{
        _botClient = new TelegramBotClient("6745804700:AAGgLPQmWQT6uzw8tdf86fyXBhs3D-H93hE");
        _stages = new Dictionary<long, string>(0);
        _signs = new Dictionary<long, GeraldicSigns>(0);
        _chat = null;
        _user = null;
        _message = null;


        // Создание логгера.
        using ILoggerFactory factory = LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.TimestampFormat = "HH:mm:ss";
            }
            );
        });
        factory.AddFile("../../../../var/logging");
        _logger = factory.CreateLogger<MessageUpdateHandler>();
    }


    /// <summary>
    /// Конструктор со данными о боте.
    /// </summary>
    /// <param name="botClient">Клиент бота.</param>
    /// <param name="stages">Состояния бота для каждого пользователя..</param>
    /// <param name="signs">Текущие данные для кадлого пользователя.</param>
    /// <exception cref="ArgumentNullException">Аргумент или его содержание null.</exception>
    public MessageUpdateHandler(ITelegramBotClient botClient, Dictionary<long, string> stages,
        Dictionary<long, GeraldicSigns> signs)
    {
        // Проверка на корректноть аргументов.
        if (botClient == null || stages == null)
        {
            throw new ArgumentNullException("Аргументы не могут быть null");
        }
        foreach (string stageValue in stages.Values)
        {
            if (stageValue == null)
            {
                throw new ArgumentNullException("Содержание _stages не может быть null.");
            }
        }
        foreach (GeraldicSigns sign in signs.Values)
        {
            if (sign == null)
            {
                throw new ArgumentNullException("Содержание _signs не может быть null.");
            }
        }


        _botClient = botClient;
        _user = null;
        _chat = null;
        _message = null;

        _stages = new Dictionary<long, string>(0);
        // Поэлементное копирование.
        foreach (var elem in stages)
        {
            _stages.Add(elem.Key, elem.Value);
        }

        _signs = new Dictionary<long, GeraldicSigns>(0);
        // Поэлементное копирование.
        foreach (var elem in signs)
        {
            _signs.Add(elem.Key, elem.Value);
        }


        // Создание логгера.
        using ILoggerFactory factory = LoggerFactory.Create(builder =>
        {
            builder.AddSimpleConsole(options =>
            {
                options.TimestampFormat = "HH:mm:ss";
            }
            );
        });
        factory.AddFile("../../../../var/logging");
        _logger = factory.CreateLogger<MessageUpdateHandler>();
    }


    /// <summary>
    /// Создание меню из заданных кнопок, где две кнопки в одной строке.
    /// </summary>
    /// <param name="buttons">Заданные кнопки.</param>
    /// <returns>Полученное меню.</returns>
    private ReplyKeyboardMarkup GetReplyKeyboard(string[] buttons)
    {
        List<KeyboardButton[]> designedButtons = new List<KeyboardButton[]>(0);
        // Для каждой пары кнопок.
        for (int i = 0; i < (buttons.Length + 1) / 2; ++i)
        {
            // Обрабатываем последние кнопки отдельно.
            if (i == (buttons.Length + 1) / 2 - 1)
            {
                // Если осталась одна, то только она в строке.
                if (buttons.Length % 2 == 0)
                {
                    designedButtons.Add(new KeyboardButton[]
                                           {
                                                new KeyboardButton(buttons[i * 2]),
                                                new KeyboardButton(buttons[i * 2 + 1]),
                                           });
                }
                else
                {
                    designedButtons.Add(new KeyboardButton[]
                                           {
                                                new KeyboardButton(buttons[i * 2]),
                                           });
                }
            }
            else
            {
                designedButtons.Add(new KeyboardButton[]
                                          {
                                                new KeyboardButton(buttons[i * 2]),
                                                new KeyboardButton(buttons[i * 2 + 1]),
                                          });
            }
        }


        // Создание меню.
        var replyKeyboard = new ReplyKeyboardMarkup(designedButtons)
        {
            ResizeKeyboard = true,
        };
        return replyKeyboard;
    }


    /// <summary>
    /// Проверка что базовые поля при работе с чатом не null.
    /// </summary>
    /// <returns>Истинность того, что поля не null.</returns>
    private bool CheckNullBaseArguments()
    {
        if (_user == null || _chat == null || _message == null || _cancellationToken == null)
        {
            return false;
        }
        return true;
    }


    /// <summary>
    /// Вступительные действия при сообщения от нового пользователя.
    /// </summary>
    /// <exception cref="ArgumentNullException">Одно из полей null.</exception>
    private async void StartWork()
    {
        // Проверка на корректность параметров.
        if (!CheckNullBaseArguments())
        {
            throw new ArgumentNullException("Поле не может быть null.");
        }


        // Добавление нового пользователя.
        if (!_stages.ContainsKey(_user.Id))
        {
            _stages.Add(_user.Id, "0");
            _signs.Add(_user.Id, new GeraldicSigns());
        }


        // Вступительное сообщение.
        await _botClient.SendTextMessageAsync(
        _chat.Id,
        "Добрый день! Я бот для работы с вашим файлом, содержащим информацию о геральдических символах. Выберите операцию:",
        replyMarkup: GetReplyKeyboard(new string[]
        { "Загрузить данные", "Сделать выборку", "Отсортировать", "Сохранить"}));
    }


    /// <summary>
    /// Вступительные действия при сообщении от пользователя,
    /// с которым работал до отключения.
    /// </summary>
    /// <exception cref="ArgumentNullException">Одно из полей null.</exception>
    private async void StartWorkAfterShotdown()
    {
        // Проверка на корректность полей.
        if (!CheckNullBaseArguments())
        {
            throw new ArgumentNullException("Поле не может быть null.");
        }


        // Добавление нового пользователя.
        _stages.Add(_user.Id, "0");
        _signs.Add(_user.Id, new GeraldicSigns());


        // Вступительное сообщение.
        await _botClient.SendTextMessageAsync(
        _chat.Id,
        "Добрый день! Я бот для работы с вашим файлом, содержащим информацию о геральдических символах. Выберите действие:",
        replyMarkup: GetReplyKeyboard(new string[]
        { "Загрузить данные", "Сделать выборку", "Отсортировать", "Сохранить"}));
    }


    /// <summary>
    /// Переход в изначальное состояние.
    /// </summary>
    /// <exception cref="ArgumentNullException">Одно из полей null.</exception>
    private async void ResetToNewOperation()
    {
        // Проверка на корректность полей.
        if (!CheckNullBaseArguments())
        {
            throw new ArgumentNullException("Поле не может быть null.");
        }


        _stages[_user.Id] = "0";

        // Предлагаем новую операцию.
        await _botClient.SendTextMessageAsync(
        _chat.Id,
        "Выберите следующую операцию.",
        replyMarkup: GetReplyKeyboard(new string[]
        { "Загрузить данные", "Сделать выборку", "Отсортировать", "Сохранить"}));
    }


    /// <summary>
    /// Обработать ответ пользователя на стадии выбора операции.
    /// </summary>
    /// <exception cref="ArgumentNullException">Одно из полей null.</exception>
    private async void ProduceStageOperationInput()
    {
        // Проверка на корректность полей.
        if (!CheckNullBaseArguments())
        {
            throw new ArgumentNullException("Поле не может быть null.");
        }


        if (_message.Text == "Загрузить данные")
        {
            _stages[_user.Id] += ".0";

            await _botClient.SendTextMessageAsync(
            chatId: _chat.Id,
            text: "Выберите формат загружаемого файла.",
            replyMarkup: GetReplyKeyboard(new string[]
            { "JSON", "CSV"}),
            cancellationToken: _cancellationToken);

        }
        else if (_message.Text == "Сделать выборку")
        {
            _stages[_user.Id] += ".1";

            await _botClient.SendTextMessageAsync(
            chatId: _chat.Id,
            text: "Выберите столбцы для выборки:",
            replyMarkup: GetReplyKeyboard(new string[]
            {"Type", "RegistrationDate", "CertificateHolderName и RegistrationDate" }),
            cancellationToken: _cancellationToken);

        }
        else if (_message.Text == "Отсортировать")
        {
            _stages[_user.Id] += ".2";

            await _botClient.SendTextMessageAsync(
            chatId: _chat.Id,
            text: "Сортировка будет производится по" +
            " столбцу RegistrationNumber. Выберите тип сортировки:",
            replyMarkup: GetReplyKeyboard(new string[]
            { "В порядке возрастания номера", "В порядке убывания номера"}),
            cancellationToken: _cancellationToken);

        }
        else if (_message.Text == "Сохранить")
        {
            _stages[_user.Id] += ".3";

            await _botClient.SendTextMessageAsync(
            chatId: _chat.Id,
            text: "Выберите формат выводимого файла.",
            replyMarkup: GetReplyKeyboard(new string[]
            { "JSON", "CSV"}),
            cancellationToken: _cancellationToken);
        }
        else
        {
            await _botClient.SendTextMessageAsync(
            _chat.Id,
            "Пожалуйста, выберите один из пунктов меню.",
            replyToMessageId: _message.MessageId);
        }

    }


    /// <summary>
    /// Обработать ответ пользователя на стадии выбора входного файла.
    /// </summary>
    /// <exception cref="ArgumentNullException">Одно из полей null.</exception>
    private async void ProduceStageInputFileTypeInput()
    {
        // Проверка на корректность полей.
        if (!CheckNullBaseArguments())
        {
            throw new ArgumentNullException("Поле не может быть null.");
        }


        if (_message.Text == "CSV")
        {
            _stages[_user.Id] += ".0";

            await _botClient.SendTextMessageAsync(
            chatId: _chat.Id,
            text: "Пришлите файл с данными.",
            replyMarkup: new ReplyKeyboardRemove(),
            cancellationToken: _cancellationToken);
        }
        else if (_message.Text == "JSON")
        {
            _stages[_user.Id] += ".1";

            await _botClient.SendTextMessageAsync(
            chatId: _chat.Id,
            text: "Пришлите файл с данными (поля в JSON файле должны быть записаны в сamelCase).",
            replyMarkup: new ReplyKeyboardRemove(),
            cancellationToken: _cancellationToken);
        }
        else
        {
            await _botClient.SendTextMessageAsync(
            chatId: _chat.Id,
            text: "Некорректный запрос.",
            cancellationToken: _cancellationToken);
            return;
        }
    }


    /// <summary>
    /// Обработать ответ пользователя на стадии выбора поля для выборки.
    /// </summary>
    /// <exception cref="ArgumentNullException">Одно из полей null.</exception>
    private async void ProduceStageChooseTypeInput()
    {
        // Проверка на корректность полей.
        if (!CheckNullBaseArguments())
        {
            throw new ArgumentNullException("Поле не может быть null.");
        }


        switch (_message.Text)
        {
            case "Type":
                _stages[_user.Id] += ".0";

                await _botClient.SendTextMessageAsync(
                chatId: _chat.Id,
                text: "Введите значение по которому будет выборка.",
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: _cancellationToken);

                break;
            case "RegistrationDate":
                _stages[_user.Id] += ".1";

                await _botClient.SendTextMessageAsync(
                chatId: _chat.Id,
                text: "Введите значение по которому будет выборка.",
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: _cancellationToken);

                break;
            case "CertificateHolderName и RegistrationDate":
                _stages[_user.Id] += ".2";

                await _botClient.SendTextMessageAsync(
                chatId: _chat.Id,
                text: "Введите значения по которым будет " +
                "выборка разделяя \";\" (вначале для столбца CertificateHolderName," +
                " потом для RegistrationDate).",
                replyMarkup: new ReplyKeyboardRemove(),
                cancellationToken: _cancellationToken);

                break;
            default:
                await _botClient.SendTextMessageAsync(_chat.Id, "Некорректный запрос.");
                return;
        }
    }


    /// <summary>
    /// Обработать ответ пользователя на стадии выбора значения для выборки.
    /// </summary>
    /// <exception cref="ArgumentNullException">Одно из полей null.</exception>
    private async void ProduceStageChooseValueInput()
    {
        // Проверка на корректность полей.
        if (!CheckNullBaseArguments())
        {
            throw new ArgumentNullException("Поле не может быть null.");
        }


        if (_stages[_user.Id][4] == '0')
        {
            _signs[_user.Id].Choose("Type", _message.Text);

            ResetToNewOperation();
        }
        else if (_stages[_user.Id][4] == '1')
        {
            _signs[_user.Id].Choose("RegistrationDate", _message.Text);

            _stages[_user.Id] = "0";

            ResetToNewOperation();
        }
        else if (_stages[_user.Id][4] == '2')
        {
            _signs[_user.Id].Choose(_message.Text.Split("\";\""));

            ResetToNewOperation();
        }
    }





    /// <summary>
    /// Обработать ответ пользователя на стадии выбора типа сортировки.
    /// </summary>
    /// <exception cref="ArgumentNullException">Одно из полей null.</exception>
    private async void ProduceStageSortTypeInput()
    {
        // Проверка на корректность полей.
        if (!CheckNullBaseArguments())
        {
            throw new ArgumentNullException("Поля не может быть null.");
        }


        switch (_message.Text)
        {
            case "В порядке возрастания номера":
                _signs[_user.Id].Sort(1);

                ResetToNewOperation();
                return;
            case "В порядке убывания номера":
                _signs[_user.Id].Sort(2);

                ResetToNewOperation();
                return;
            default:
                await _botClient.SendTextMessageAsync(_chat.Id, "Некорректный запрос.");
                return;
        }
    }


    /// <summary>
    /// Обработать ответ пользователя на стадии выбора типа выходного файла.
    /// </summary>
    /// <exception cref="ArgumentNullException">Одно из полей null.</exception>
    private async void ProduceStageOutputFileTypeInput()
    {
        // Проверка на корректность параметров.
        if (!CheckNullBaseArguments())
        {
            throw new ArgumentNullException("Поля не могут быть null.");
        }


        switch (_message.Text)
        {
            case "JSON":
                try
                {
                    JSONProcessing jsonprocessor = new JSONProcessing();
                    await _botClient.SendDocumentAsync(
                    chatId: _chat.Id,
                    document: InputFile.FromStream(stream: jsonprocessor.Write(_signs[_user.Id].GetList()), fileName: "output.json"),
                    caption: "Ваш файл.");

                    ResetToNewOperation();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"{ex}");
                    await _botClient.SendTextMessageAsync(_chat.Id, "Не удалось выполнить опреацию.");
                }
                return;
            case "CSV":
                try
                {
                    CSVProcessing csvProcessor = new CSVProcessing();
                    await _botClient.SendDocumentAsync(
                    chatId: _chat.Id,
                    document: InputFile.FromStream(stream: csvProcessor.Write(_signs[_user.Id].GetList()), fileName: "output.csv"),
                    caption: "Ваш файл.");
                    ResetToNewOperation();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"{ex}");
                    await _botClient.SendTextMessageAsync(_chat.Id, "Не удалось выполнить операцию.");
                }
                return;
            default:
                await _botClient.SendTextMessageAsync(_chat.Id, "Некорректный запрос.");
                return;
        }
    }


    /// <summary>
    /// Обработать ответ пользователя на стадии ввода CSV файла.
    /// </summary>
    /// <exception cref="ArgumentNullException">Одно из полей null.</exception>
    private async void ProduceStageCSVFileInput()
    {
        // Проверка на корректность полей.
        if (!CheckNullBaseArguments())
        {
            throw new ArgumentNullException("Поля не могут быть null.");
        }


        // Получаем файл.
        string destinationFilePath = "../downloaded_file" + _user.Id.ToString();
        await using Stream fileStream = System.IO.File.Create(destinationFilePath);
        var file = await _botClient.GetInfoAndDownloadFileAsync(
            fileId: _message.Document.FileId,
            destination: fileStream,
            cancellationToken: _cancellationToken);
        fileStream.Close();


        // Записываем данные.
        try
        {
            CSVProcessing csvProcessor = new CSVProcessing();
            _signs[_user.Id] = new GeraldicSigns(csvProcessor.Read(System.IO.File.Open(destinationFilePath, FileMode.Open)));
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"{ex}");
            await _botClient.SendTextMessageAsync(_chat.Id, "Некорректный файл.");
        }


        System.IO.File.Delete(destinationFilePath);

        ResetToNewOperation();
    }



    /// <summary>
    /// Обработать ответ пользователя на стадии ввода JSON файла.
    /// </summary>
    /// <exception cref="ArgumentNullException">Одно из полей null.</exception>
    private async void ProduceStageJSONFileInput()
    {
        // Проверка на корректность полей.
        if (!CheckNullBaseArguments())
        {
            throw new ArgumentNullException("Поля не могут быть null.");
        }


        // Получаем файл.
        string destinationFilePath = "../downloaded_file" + _user.Id.ToString();
        await using Stream fileStream = System.IO.File.Create(destinationFilePath);
        var file = await _botClient.GetInfoAndDownloadFileAsync(
            fileId: _message.Document.FileId,
            destination: fileStream,
            cancellationToken: _cancellationToken);
        fileStream.Close();


        // Записываем данные.
        try
        {
            JSONProcessing jsonprocessor = new JSONProcessing();
            _signs[_user.Id] = new GeraldicSigns(jsonprocessor.Read(System.IO.File.Open(destinationFilePath, FileMode.Open)));
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"{ex}");
            await _botClient.SendTextMessageAsync(_chat.Id, "Некорректный файл.");
        }

        System.IO.File.Delete(destinationFilePath);

        ResetToNewOperation();

    }


    /// <summary>
    /// Организация взаимодействия с пользователем.
    /// </summary>
    public async void ProduceUpdateReaction(CancellationToken cancellationToken, Message message, User user, Chat chat)
    {
        _cancellationToken = cancellationToken;
        _message = message;
        _user = user;
        _chat = chat;
        switch (_message.Type)
        {
            case MessageType.Text:
                {
                    _logger.LogInformation($"{_user.FirstName} ({_user.Id}) написал сообщение: {_message.Text}");
                    if (_message.Text == "/start")
                    {
                        StartWork();
                    }
                    else if (!_stages.ContainsKey(_user.Id))
                    {
                        StartWorkAfterShotdown();
                    }
                    else if (_stages[_user.Id] == "0")
                    {
                        ProduceStageOperationInput();
                    }
                    else
                    {
                        if (_stages[_user.Id][2] == '0')
                        {
                            ProduceStageInputFileTypeInput();
                        }
                        else if (_stages[_user.Id][2] == '1')
                        {
                            if (_stages[_user.Id] == "0.1")
                            {
                                ProduceStageChooseTypeInput();
                            }
                            else
                            {
                                ProduceStageChooseValueInput();
                            }
                        }
                        else if (_stages[_user.Id][2] == '2')
                        {
                            ProduceStageSortTypeInput();
                        }
                        else
                        {
                            ProduceStageOutputFileTypeInput();
                        }
                    }
                    break;
                }
            case MessageType.Document:
                _logger.LogInformation($"{_user.FirstName} ({_user.Id}) подал документ.");
                if (_stages[_user.Id] == "0.0.0")
                {
                    ProduceStageCSVFileInput();
                }
                else if (_stages[_user.Id] == "0.0.1")
                {
                    ProduceStageJSONFileInput();
                }
                else
                {
                    await _botClient.SendTextMessageAsync(_chat.Id, "Некорректный запрос.");
                }
                return;
            default:
                _logger.LogInformation($"{_user.FirstName} ({_user.Id}) прислал сообщение недопустимого типа.");
                await _botClient.SendTextMessageAsync(_chat.Id, "Некорректный запрос.");
                return;
        }
    }
}

