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
        
        public static void DownloadAndConvertToMp3(string youtubeUrl)
        {
            try
            {
                
                
                // Скачивание видео через yt-dlp
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = @"yt-dlp", // Путь к yt-dlp
                    Arguments = $"-f bestaudio[ext=m4a] --no-playlist --output \"C:\\Users\\zadre\\Desktop\\Telegram_Bot_Data\\%(title)s.%(ext)s\" {youtubeUrl}",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                Process.Start(processStartInfo).WaitForExit();
/*
                // Путь к скачанному видео
                string downloadedFile = $"C:\\Users\\zadre\\Desktop\\Telegram_Bot_Data\\{video.Title}.webm"; // или другой формат, в зависимости от выбора
                if (!File.Exists(downloadedFile))
                {
                    Console.WriteLine($"Файл не найден: {downloadedFile}");
                    return;
                }
                string outputMp3Path = $"C:\\Users\\zadre\\Desktop\\Telegram_Bot_Data\\{video.Title}.mp3";
                // Преобразование видео в MP3 через ffmpeg

                Process.Start(new ProcessStartInfo
                {
                    FileName = @"ffmpeg", // Путь к ffmpeg
                    Arguments = $"-i \"C:\\Users\\zadre\\Desktop\\Telegram_Bot_Data\\MACAN, Loc-Dog - Всё равно.webm\" -vn -ar 44100 -ac 2 -b:a 192k \"C:\\Users\\zadre\\Desktop\\Telegram_Bot_Data\\MACAN, Loc-Dog - Всё равно.mp3\"",
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
       
    }
}
