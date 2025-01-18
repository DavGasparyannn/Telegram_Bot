using AngleSharp.Dom;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
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
            string userDirectory = $"C:\\Users\\zadre\\Desktop\\Telegram_Bot_Data\\{message.Chat.Id}";
            string loggerPath = $"{userDirectory}/messages.txt";
            string loggerMessage;
            loggerMessage = $"{DateTime.Now}: {message.Text}";
            if (message.Text.ToLower().Contains("/start"))
            {

                await client.SendMessage(message.Chat.Id, "Hi! please input youtube link for download !!!");
                Directory.CreateDirectory(userDirectory);
                if (!File.Exists(loggerPath))
                {
                    File.Create(loggerPath).Dispose(); 
                }
                return;
            }

            if (message.Text.StartsWith("https://www.youtube.com") || message.Text.StartsWith("https://youtu.be/"))
            {
                var loadingMessage = await client.SendMessage(message.Chat.Id, "Идёт отправка песни...");
                string youtubeUrl = message.Text;

                YoutubeClient youtubeClient = new YoutubeClient();
                var video = await youtubeClient.Videos.GetAsync(youtubeUrl);
                loggerMessage += $", Title : {video.Title}";


                YoutubeDownloader.DownloadAndConvertToMp3(youtubeUrl);

                await using Stream stream = File.OpenRead($"C:\\Users\\zadre\\Desktop\\Telegram_Bot_Data\\{video.Title}.m4a");
                await client.SendAudio(message.Chat.Id, stream, title : $"{video.Title}");

                await client.DeleteMessage(message.Chat.Id, loadingMessage.MessageId);
            }
                File.AppendAllText(loggerPath, loggerMessage + "\n");

        }
        private static async Task Error(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
        {
            Console.WriteLine($"Telegram Bot: Error {exception.Message} ....... {source}" );
        }
    }
}
