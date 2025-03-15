using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Telegram_Bot.Services;
using YoutubeExplode;
using YoutubeExplode.Videos;

namespace Telegram_Bot.Downloaders
{
    public static class YoutubeDownloader
    {
        
        public static void DownloadAndConvertToMp3(string youtubeUrl, Video video)
        {
            
            string safeTitle = Helper.MakeFileNameSafe(video.Title);
            string outputTemplate = $"C:\\Users\\zadre\\Desktop\\Telegram_Bot_Data\\{safeTitle}.%(ext)s";
            try
            {

                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = @"yt-dlp",
                    Arguments = $"-f bestaudio[ext=m4a] --no-playlist --output \"{outputTemplate}\" {youtubeUrl}",
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                Process.Start(processStartInfo).WaitForExit();          
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }
        private static async Task<string> DownloadSongFromYoutubeBySpotify(string songName)
        {
            var outputFileName = $"{songName}.mp3".Replace(" ", "_");
            var outputPath = Path.Combine(Directory.GetCurrentDirectory(), outputFileName);

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "yt-dlp", // Убедись, что yt-dlp доступен в PATH
                    Arguments = $"-x --audio-format mp3 -o \"{outputPath}\" \"ytsearch:{songName}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();

            return File.Exists(outputPath) ? outputPath : null;
        }

    }
}
