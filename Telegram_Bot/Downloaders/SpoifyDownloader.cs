using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Telegram_Bot.Downloaders
{
    public class SpoifyDownloader
    {
        public void DownloadTrack(string spotifyUrl, string downloadPath)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "spotdl",
                    Arguments = $"\"{spotifyUrl}\" --output \"{downloadPath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при скачивании трека: {ex.Message}");
            }
        }
    }
}
