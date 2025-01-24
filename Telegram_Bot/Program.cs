using AngleSharp.Dom;
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
using TagLib;
using File = System.IO.File;
using TagLib.Mpeg4;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using SpotifyAPI.Web;
using System.Collections.Generic;
using System.Linq;
namespace Telegram_Bot
{
    internal class Program
    {
        private static string SpotifyClientId = "ccfb561c2d5d48ca84f31d4b1cd28ae7";
        private static string SpotifyClientSecret = "b18447dc19f346518ce6c398a35e00bb";
        private static SpotifyClient spotifyClient;
        private static Dictionary<long, List<FullTrack>> userTrackSelections = new Dictionary<long, List<FullTrack>>();
        static void Main(string[] args)
        {
            Environment.SetEnvironmentVariable("SLAVA_UKRAINI", "1");
            var client = new TelegramBotClient("7824764223:AAEBQywOoE8-sCyscVMQexcs-3TOd38ozxU");
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            var spotifyConfig = SpotifyClientConfig
            .CreateDefault()
            .WithAuthenticator(new ClientCredentialsAuthenticator(SpotifyClientId, SpotifyClientSecret));
            spotifyClient = new SpotifyClient(spotifyConfig);

            client.StartReceiving(Update, Error,
                new Telegram.Bot.Polling.ReceiverOptions
                {
                    AllowedUpdates = { } // Получать все типы обновлений
                },
            cancellationToken);
            Console.ReadLine();
            cancellationTokenSource.Cancel();
        }

        static Helper helper = new Helper();
        private static async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            
            var message = update.Message;
            if(helper.isRenamingSong && message != null && helper.chatId == message.Chat.Id)
            {
                string newSongName = message.Text;
                await using Stream stream = File.OpenRead($"{helper.filePath}"); 

                await client.SendAudio(message.Chat.Id, stream, title: $"{newSongName}");
                helper.isRenamingSong = false;
                File.Delete($"{helper.filePath}");
                return;
            }
            if (message != null)
            {
                if (message.Text.StartsWith("/song", StringComparison.OrdinalIgnoreCase) ||
            message.Text.StartsWith("найди", StringComparison.OrdinalIgnoreCase))
                {
                    // Извлекаем запрос из сообщения (удаляем ключевое слово "/song" или "найди")
                    var searchQuery = message.Text.Contains(" ") ? message.Text[(message.Text.IndexOf(" ") + 1)..] : "";

                    if (string.IsNullOrWhiteSpace(searchQuery))
                    {
                        await client.SendMessage(
                            chatId: update.Message.Chat.Id,
                            text: "Пожалуйста, напишите название песни после команды /song или слова 'найди'.",
                            cancellationToken: token
                        );
                        return;
                    }

                    // Выполняем поиск песен
                    var tracks = await SearchTracksAsync(searchQuery);

                    // Если ничего не найдено
                    if (tracks.Count == 0)
                    {
                        await client.SendMessage(
                            chatId: update.Message.Chat.Id,
                            text: "Ничего не найдено. Попробуй ввести другое название песни.",
                            cancellationToken: token
                        );
                        return;
                    }

                    // Формируем список треков
                    string response = "Вот что удалось найти:\n";
                    for (int i = 0; i < tracks.Count; i++)
                    {
                        response += $"{i + 1}. {tracks[i].Name} - {tracks[i].Artists.FirstOrDefault()?.Name}\n";
                    }

                    // Отправляем результат пользователю
                    await client.SendMessage(
                        chatId: update.Message.Chat.Id,
                        text: response,
                        cancellationToken: token
                    );
                }

                string userDirectory = $"C:\\Users\\zadre\\Desktop\\Telegram_Bot_Data\\{message.Chat.Id}-{message.Chat.Username ?? "NoUsername"}-{message.Chat.FirstName}";
                string loggerPath = $"{userDirectory}/messages.txt";

                string loggerMessage;
                loggerMessage = $"{DateTime.Now}: {message.Text}";

                if (message.Text == ("/start"))
                {
                    var menuKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[] { InlineKeyboardButton.WithCallbackData("Скачать видео с YouTube", "download_song") },
                    new[] { InlineKeyboardButton.WithCallbackData("Редактировать песню", "edit_song") }
                });
                    await client.SendMessage(
                    chatId: message.Chat.Id,
                    text: "Привет😜 Я тебе помогу скачать твои любимые песни с YouTube❤️\r\nПросто отправь мне ссылку на эту песню(пр. \"https://www.youtube.com/Ссылка_на_песню\" или \"https://youtu.be/Ссылка_на_песню\")👌",
                    replyMarkup: menuKeyboard,
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
                    string title = video.Title;
                   
                    loggerMessage += $", Title : {video.Title}";

                    if (video.Duration <= TimeSpan.FromMinutes(10))
                    {
                        var loadingMessage = await client.SendMessage(message.Chat.Id, "⏳Пожалуйста подождите, идёт отправка песни...");
                        YoutubeDownloader.DownloadAndConvertToMp3(youtubeUrl,video);

                            helper.ChangeEscapedFileName(title);
                        helper.chatId = message.Chat.Id;
                        await using Stream stream = File.OpenRead($@"{helper.filePath}");

                        var songDownloadedMenuKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[] { InlineKeyboardButton.WithCallbackData("Изменить название песни🎵", "change_song_name") },
                    new[] { InlineKeyboardButton.WithCallbackData("Сохранить💾", "save_song") }
                });
                        await client.SendAudio(message.Chat.Id, stream, title: $"{video.Title}💘", replyMarkup: songDownloadedMenuKeyboard);
                        await client.DeleteMessage(message.Chat.Id, loadingMessage.MessageId);
                        

                        
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

                if (callbackQuery.Data == "save_song")
                {
                    await client.SendMessage(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "Спасибо что пользуетесь нашим ботом🎵\r\nЖдём вас заново, можете отправлять ссылку🎵",
                        cancellationToken: token
                    );
                    File.Delete($"{helper.filePath}");
                }
                else if (callbackQuery.Data == "change_song_name")
                {
                    helper.isRenamingSong = true;
                    await client.SendMessage(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "Введи имя для песни:",
                        cancellationToken: token
                    );
                }
                else if (callbackQuery.Data == "download_song")
                {
                    await client.SendMessage(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "Отправьте ссылку на видео.",
                        cancellationToken: token
                    );
                }
                else if (callbackQuery.Data == "edit_song")
                {
                    await client.SendMessage(
                        chatId: callbackQuery.Message.Chat.Id,
                        text: "Отправьте песню для редактирования",
                        cancellationToken: token
                    );
                }
            }
        }
        private static async Task Error(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
        {
            await File.AppendAllTextAsync("C:\\Users\\zadre\\Desktop\\Telegram_Bot_Data\\ErrorLog.txt", $"Error : {exception.Message} ....... {source} : {DateTime.Now}\n");
        }
        private static async Task<List<FullTrack>> SearchTracksAsync(string query)
        {
            var searchRequest = new SearchRequest(SearchRequest.Types.Track, query)
            {
                Limit = 10 // Максимум 10 результатов
            };

            var searchResponse = await spotifyClient.Search.Item(searchRequest);
            return searchResponse.Tracks?.Items?.ToList() ?? new List<FullTrack>();
        }
    }
}
