using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram_Bot.Enums;

namespace Telegram_Bot.Services
{
    public class Logger
    {
        private string loggerPath;
        private string message;
        private Message messageFull;
        public string musicTitle;

        public LogMessageType type = LogMessageType.Message;
        public Logger(string userDirectory, Message message)
        {
            this.messageFull = message;
            this.loggerPath = $"{userDirectory}/messages.txt";
            this.message = $"{DateTime.Now}: ";
            if (!Directory.Exists(userDirectory))
            {
                Directory.CreateDirectory(userDirectory);
            }
            if (!File.Exists(loggerPath))
            {
                File.Create(loggerPath).Dispose();
            }
        }
        public void Log() {
            MakeMessage();
            File.AppendAllText(loggerPath, message + "\n");
            Console.WriteLine($"{messageFull.Chat.FirstName} | {message}");
        }
        private void MakeMessage() {
            message += $"{type.ToString()}: ";
            switch (type)
            {
                case LogMessageType.Message:
                    message += $"{messageFull.Text}";
                    break;
                case LogMessageType.Search:
                    message += $"Song - {musicTitle}";
                    break;
                case LogMessageType.Youtube:
                    message += $"Title - {musicTitle}";
                    break;
                default:
                    break;
            }
        }
    }
}
