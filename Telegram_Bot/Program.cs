﻿using AngleSharp.Dom;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram_Bot.Downloaders;
using YoutubeExplode;
using YoutubeExplode.Videos;

namespace Telegram_Bot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Environment.SetEnvironmentVariable("SLAVA_UKRAINI", "1");
            var client = new TelegramBotClient("7824764223:AAEBQywOoE8-sCyscVMQexcs-3TOd38ozxU");
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            client.StartReceiving(Update, Error,
                new Telegram.Bot.Polling.ReceiverOptions
                {
                    AllowedUpdates = { } // Получать все типы обновлений
                },
            cancellationToken);
            Console.ReadLine();
            cancellationTokenSource.Cancel();
        }


        private static async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var message = update.Message;
            if (message != null)
            {
                string userDirectory = $"C:\\Users\\zadre\\Desktop\\Telegram_Bot_Data\\{message.Chat.Id}-{message.Chat.Username ?? "NoUsername"}-{message.Chat.FirstName}";
                string loggerPath = $"{userDirectory}/messages.txt";

                string loggerMessage;
                loggerMessage = $"{DateTime.Now}: {message.Text}";

                if (message.Text == ("/start"))
                {
                    var replyKeyboard = new InlineKeyboardMarkup(new[]
                         {
                    InlineKeyboardButton.WithCallbackData("Скачать видео с YouTube", "download_video")
                });
                    await client.SendMessage(
                    chatId: message.Chat.Id,
                    text: "Привет😜 Я тебе помогу скачать твои любимые песни с YouTube❤️\r\nПросто отправь мне ссылку на эту песню(пр. \"https://www.youtube.com/Ссылка_на_песню\" или \"https://youtu.be/Ссылка_на_песню\")👌",
                    replyMarkup: replyKeyboard,
                    cancellationToken: token
                    );

                    if (!Directory.Exists(userDirectory))
                    {
                        Directory.CreateDirectory(userDirectory);
                    }
                    if (!File.Exists(loggerPath))
                    {
                        File.Create(loggerPath).Dispose();
                    }
                    return;
                }
                else if (message.Text.StartsWith("https://www.youtube.com") || message.Text.StartsWith("https://youtu.be/"))
                {

                    string youtubeUrl = message.Text;

                    YoutubeClient youtubeClient = new YoutubeClient();
                    var video = await youtubeClient.Videos.GetAsync(youtubeUrl);
                    loggerMessage += $", Title : {video.Title}";

                    if (video.Duration <= TimeSpan.FromMinutes(10))
                    {
                        var loadingMessage = await client.SendMessage(message.Chat.Id, "Идёт отправка песни...");
                        YoutubeDownloader.DownloadAndConvertToMp3(youtubeUrl);
                        await using Stream stream = File.OpenRead($"C:\\Users\\zadre\\Desktop\\Telegram_Bot_Data\\{video.Title}.m4a");
                        await client.SendAudio(message.Chat.Id, stream, title: $"{video.Title}💘");
                        await client.DeleteMessage(message.Chat.Id, loadingMessage.MessageId);
                        File.Delete($"C:\\Users\\zadre\\Desktop\\Telegram_Bot_Data\\{video.Title}.m4a");
                        File.AppendAllText(loggerPath, loggerMessage + "\n");
                        Console.WriteLine($"{message.Chat.FirstName} | {loggerMessage}");
                    }
                    else
                    {
                        await client.SendMessage(message.Chat.Id, "Отправьте видео с длительностью менее 10-и минут");
                    }
                    return;

                }
            }
            else if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery != null)
            {
                var callbackQuery = update.CallbackQuery;

                if (callbackQuery.Data == "download_video")
                {
                    await client.SendMessage(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "Отправьте ссылку на видео.",
                        cancellationToken: token
                    );

                }
            }
        }
        private static async Task Error(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
        {
            await File.AppendAllTextAsync("C:\\Users\\zadre\\Desktop\\Telegram_Bot_Data\\ErrorLog.txt", $"Error : {exception.Message} ....... {source} : {DateTime.Now}\n");
        }
    }
}
