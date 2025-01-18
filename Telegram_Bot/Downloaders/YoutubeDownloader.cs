using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YoutubeExplode;

namespace Telegram_Bot.Downloaders
{
    public static class YoutubeDownloader
    {
        
        public static async void DownloadAndConvertToMp3(string youtubeUrl)
        {
            try
            {
                YoutubeClient youtubeClient = new YoutubeClient();
                var video = await youtubeClient.Videos.GetAsync(youtubeUrl);
                
                // Скачивание видео через yt-dlp
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = @"yt-dlp", // Путь к yt-dlp
                    Arguments = $"-f bestaudio --no-playlist --output \"C:\\Users\\zadre\\Desktop\\Telegram_Bot_Data\\%(title)s.%(ext)s\" {youtubeUrl}",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                Process.Start(processStartInfo).WaitForExit();

                // Путь к скачанному видео
                string downloadedFile = $"C:\\Users\\zadre\\Desktop\\Telegram_Bot_Data\\{video.Title}.webm"; // или другой формат, в зависимости от выбора
                string outputMp3Path = $"C:\\Users\\zadre\\Desktop\\Telegram_Bot_Data\\{video.Title}.mp3";
                // Преобразование видео в MP3 через ffmpeg

                Process.Start(new ProcessStartInfo
                {
                    FileName = @"ffmpeg", // Путь к ffmpeg
                    Arguments = $"-i \"{downloadedFile}\" -vn -ar 44100 -ac 2 -b:a 192k \"{outputMp3Path}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }).WaitForExit();

                Console.WriteLine("Конвертация завершена. MP3 файл: " + outputMp3Path);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }
        public static async Task DownloadFileAsync(string url, string filePath)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Получаем данные с сервера
                    using (HttpResponseMessage response = await client.GetAsync(url))
                    {
                        response.EnsureSuccessStatusCode(); // Проверяем, что запрос успешен

                        // Читаем содержимое ответа в виде потока
                        using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                               fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            // Копируем содержимое в файл
                            await contentStream.CopyToAsync(fileStream);
                            Console.WriteLine("Файл успешно загружен и сохранён.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при скачивании файла: {ex.Message}");
                }
            }
        }
    }
}
