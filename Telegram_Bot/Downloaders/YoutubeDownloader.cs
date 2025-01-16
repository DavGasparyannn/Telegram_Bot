using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
                    Arguments = $"-f bestaudio --get-url {youtubeUrl}", // Получаем ссылку на лучший аудиофайл
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
                string errorOutput = process.StandardError.ReadToEnd(); // Чтение ошибок

                process.WaitForExit();

                // Логирование ошибок
                if (!string.IsNullOrEmpty(errorOutput))
                {
                    Console.WriteLine($"Ошибка при выполнении yt-dlp: {errorOutput}");
                }

                if (string.IsNullOrEmpty(audioUrl))
                {
                    Console.WriteLine("Не удалось получить ссылку на аудиофайл.");
                }

                return audioUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении аудио URL: {ex.Message}");
                return null;
            }

        }
    }
}
