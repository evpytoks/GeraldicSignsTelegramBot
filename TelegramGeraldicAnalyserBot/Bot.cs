using System;
using System.Threading;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


/// <summary>
/// Класс для организации работы телеграм бота.
/// </summary>
public class Bot
{
    private ITelegramBotClient _botClient;
    private ReceiverOptions _receiverOptions;
    private Dictionary<long, string> _stages;
    private Dictionary<long, GeraldicSigns> _signs;
    private CancellationTokenSource _cts;
    private MessageUpdateHandler _handler;
    private ILogger _logger;


    /// <summary>
    /// Конструктор бота по умолчанию.
    /// </summary>
    public Bot()
	{
        _botClient = new TelegramBotClient("6745804700:AAGgLPQmWQT6uzw8tdf86fyXBhs3D-H93hE");
        _receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[]
            {
                UpdateType.Message
            },

        };
        _stages = new Dictionary<long, string>(0);
        _signs = new Dictionary<long, GeraldicSigns>(0);
        _cts = new CancellationTokenSource();
        _handler = new MessageUpdateHandler(
                    _botClient, _stages, _signs);


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
    /// Конструктор заданного бота.
    /// </summary>
    /// <param name="client">Клиент бота.</param>
    /// <exception cref="ArgumentNullException">Один из аргументов null.</exception>
    public Bot(ITelegramBotClient client, ReceiverOptions receiverOptions, CancellationTokenSource cts)
    {
        // Проверка аргументов на корректность.
        if (client == null || receiverOptions == null || cts == null)
        {
            throw new ArgumentNullException("Аргументы не могут быть null");
        }


        _botClient = client;
        _receiverOptions = receiverOptions;
        _stages = new Dictionary<long, string>(0);
        _signs = new Dictionary<long, GeraldicSigns>(0);
        _cts = cts;
        _handler = new MessageUpdateHandler(
            _botClient, _stages, _signs);


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
    /// Свойство доступа к клиенту бота.
    /// </summary>
    public ITelegramBotClient BotClient
    {
        get
        {
            return _botClient;
        }
    }


    public CancellationTokenSource Cts
    {
        get
        {
            return _cts;
        }
    }


    public ReceiverOptions RecOptions
    {
        get
        {
            return _receiverOptions;
        }
    }


    /// <summary>
    /// Обработка полученных обновлений.
    /// </summary>
    /// <param name="botClient">Клиент бота.</param>
    /// <param name="update">Полученное обновление</param>
    private async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    var message = update.Message;
                    var user = message.From;
                    var chat = message.Chat;
                    _handler.ProduceUpdateReaction(cancellationToken, message, user, chat);
                    return;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
        }
    }


    /// <summary>
    /// Обработка полученных ошибок.
    /// </summary>
    /// <param name="botClient">Клиент бота.</param>
    /// <param name="error">Полученная ошибка.</param>
    private Task HandleError(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        var ErrorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };
        _logger.LogError(ErrorMessage);
        return Task.CompletedTask;
    }


    public void TurnOn()
    {
        _botClient.StartReceiving(HandleUpdate, HandleError, _receiverOptions, new CancellationToken());
    }
}

