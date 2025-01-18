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

                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = @"yt-dlp",
                    Arguments = $"-f bestaudio[ext=m4a] --no-playlist --output \"C:\\Users\\zadre\\Desktop\\Telegram_Bot_Data\\%(title)s.%(ext)s\" {youtubeUrl}",
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
