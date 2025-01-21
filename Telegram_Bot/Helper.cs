using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Telegram.Bot.Types;

namespace Telegram_Bot
{
    public class Helper
    {
        // Флаг, чтобы отслеживать, что пользователь сейчас переименовывает песню
        public ChatId chatId { get; set; }
        public  bool isRenamingSong { get; set; } = false;
        // Хранит путь к оригинальному файлу до переименования
        public  string originalFilePath { get; set; }

        // Можно использовать для хранения пути к файлу, который будет отправлен
        public  string filePath { get; set; }
        public void ChangeEscapedFileName(string filePath)
        {
            string newFilePath = filePath.Replace("\\", "-");
            File.Move(filePath,newFilePath);
            this.filePath = newFilePath;
        }
    }
}
