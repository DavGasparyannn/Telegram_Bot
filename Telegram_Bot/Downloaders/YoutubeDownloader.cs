using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Telegram_Bot.Downloaders
{
    public static class YoutubeDownloader
    {
        public static void DownloadAndConvertToMp3(string youtubeUrl)
        {
            try
            {
                // Скачивание видео через yt-dlp
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = @"yt-dlp", // Путь к yt-dlp
                    Arguments = $"-f bestaudio --no-playlist --get-filename --output \"C:\\Users\\zadre\\Desktop\\Telegram_Bot_Data\\%(title)s.%(ext)s\" {youtubeUrl}",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                Process.Start(processStartInfo);
               /* string output = process.StandardOutput.ReadToEnd();*/
                Process.WaitForExit();


                /*// Путь к скачанному видео
                string downloadedFile = "downloaded_video.webm"; // или другой формат, в зависимости от выбора

                // Преобразование видео в MP3 через ffmpeg
                Process.Start(new ProcessStartInfo
                {
                    FileName = @"C:\path\to\ffmpeg.exe", // Путь к ffmpeg
                    Arguments = $"-i \"{downloadedFile}\" \"{outputMp3Path}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true
                }).WaitForExit();

                Console.WriteLine("Конвертация завершена. MP3 файл: " + outputMp3Path);*/
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
