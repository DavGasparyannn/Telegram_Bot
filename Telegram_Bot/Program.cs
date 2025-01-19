using AngleSharp.Dom;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
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
            client.StartReceiving(Update, Error);
            Console.ReadKey();
        }


        private static async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var message = update.Message;
            /*var replyMarkup = new ReplyKeyboardMarkup(true)
    .AddButtons("Help me", "Call me ☎️");*/
            if (message != null)
            {
                string userDirectory = $"C:\\Users\\zadre\\Desktop\\Telegram_Bot_Data\\{message.Chat.Id}-{message.Chat.Username ?? "NoUsername"}-{message.Chat.FirstName}";
                string loggerPath = $"{userDirectory}/messages.txt";

                string loggerMessage;
                loggerMessage = $"{DateTime.Now}: {message.Text}";
                if (message.Text.ToLower().Contains("/start"))
                {

                    await client.SendMessage(message.Chat.Id, "Привет😜 Я тебе помогу скачать твои любимые песни с YouTube❤️\r\nПросто отправь мне ссылку на эту песню (пр. \"https://www.youtube.com/Ссылка_на_песню\" или \"https://youtu.be/Ссылка_на_песню\")👌");
                    Directory.CreateDirectory(userDirectory);
                    if (!File.Exists(loggerPath))
                    {
                        File.Create(loggerPath).Dispose();
                    }
                    return;
                }

                if (message.Text.StartsWith("https://www.youtube.com") || message.Text.StartsWith("https://youtu.be/"))
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
        }
        private static async Task Error(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
        {
           await File.AppendAllTextAsync("C:\\Users\\zadre\\Desktop\\Telegram_Bot_Data\\ErrorLog.txt", $"Error : {exception.Message} ....... {source} : {DateTime.Now}\n");
        }
    }
}
