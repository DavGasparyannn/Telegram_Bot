using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
       
    }
}
