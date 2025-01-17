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
            if (message.Text.ToLower().Contains("/start"))
            {

                await client.SendMessage(message.Chat.Id, "Hi! please input youtube link for download !!!");
                Directory.CreateDirectory(userDirectory);
                return;
            }
            if (message.Text.StartsWith("https://www.youtube.com"))
            {
                var youtube = new YoutubeClient();


                string youtubeUrl = message.Text;

                var video = await youtube.Videos.GetAsync(youtubeUrl);

                var UrlForDownload = await YoutubeDownloader.GetAudioDownloadLinkAsync(youtubeUrl);
                Console.WriteLine();
                await YoutubeDownloader.DownloadFileAsync(youtubeUrl, $"{userDirectory}\\{video.Title}.mp3");

                await client.SendAudio(message.Chat.Id, UrlForDownload);
                return;
            }

        }
        private static async Task Error(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
