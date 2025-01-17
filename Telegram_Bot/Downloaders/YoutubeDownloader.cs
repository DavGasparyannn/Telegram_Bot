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
        public static async Task<string> GetAudioDownloadLinkAsync(string youtubeUrl)
        {
            try
            {
                // Используем лучший доступный аудиофайл (формат может быть любым)
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = "yt-dlp", // Убедитесь, что yt-dlp установлен и доступен в системе
                    Arguments = $"-f 134 --get-url {youtubeUrl}", // Получаем ссылку на лучший аудиофайл
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                // Запускаем процесс yt-dlp
                var process = new Process
                {
                    StartInfo = processStartInfo
                };

                process.Start();

                // Чтение стандартного вывода процесса (ссылка на аудиофайл)
                string audioUrl = process.StandardOutput.ReadLine();

                // process.WaitForExit();


                return audioUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении аудио URL: {ex.Message}");
                return null;
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
