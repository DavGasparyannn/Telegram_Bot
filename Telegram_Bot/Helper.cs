using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Telegram.Bot.Types;
using YoutubeExplode.Videos;

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
        public void ChangeEscapedFileName(string titleForChange)
        {
            string newTitle = MakeFileNameSafe(titleForChange);
            filePath = $"C:\\Users\\zadre\\Desktop\\Telegram_Bot_Data\\{newTitle}.m4a";
        }
        public static string MakeFileNameSafe(string name)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars(); // Запрещённые символы
            foreach (var c in invalidChars)
            {
                name = name.Replace(c, '-'); // Заменяем запрещённые символы на `-`
            }
            return name;
        }
    }
}
