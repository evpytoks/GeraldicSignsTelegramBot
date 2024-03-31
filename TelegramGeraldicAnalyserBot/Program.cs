// tg: @fistry_bot.
using System;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Microsoft.Extensions.Logging;


class Program
{


    static async Task Main()
    {
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
        var logger = factory.CreateLogger<Program>();


        // Создание бота.
        Bot bot = new Bot(new TelegramBotClient("<Your token here>"),
            new ReceiverOptions
            {
                AllowedUpdates = new[]
            {
                UpdateType.Message
            },

            }, new CancellationTokenSource());


        // Запуск.
        bot.TurnOn();

        var me = await bot.BotClient.GetMeAsync();
        logger.LogInformation($"{me.FirstName} запущен.");

        // Завершение бота
        Console.WriteLine("Нажмите enter, чтобы завершить бота.");
        Console.ReadLine();
        logger.LogInformation($"{me.FirstName} корректно завершен.");
        bot.Cts.Cancel();
    }
}
