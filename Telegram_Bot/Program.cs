using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram_Bot.Downloaders;

namespace Telegram_Bot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var client = new TelegramBotClient("7824764223:AAEBQywOoE8-sCyscVMQexcs-3TOd38ozxU");
            client.StartReceiving(Update, Error);
            Console.ReadKey();
        }


        private static async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var message = update.Message;

            if (message.Text.ToLower().Contains("/start"))
            {
                await client.SendMessage(message.Chat.Id, "Hi! please input youtube link for download !!!");
                return;
            }
            if (message.Text.StartsWith("https://www.youtube.com"))
            {
                string youtubeUrl = message.Text;
                var UrlForDownload = await YoutubeDownloader.GetAudioDownloadLinkAsync(youtubeUrl);
                Console.WriteLine();
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
