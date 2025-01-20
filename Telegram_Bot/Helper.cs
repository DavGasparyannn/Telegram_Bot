using System;
using System.Collections.Generic;
using System.Text;

namespace Telegram_Bot
{
    public class Helper
    {
        // Флаг, чтобы отслеживать, что пользователь сейчас переименовывает песню
        public  bool isRenamingSong { get; set; } = false;

        // Хранит путь к оригинальному файлу до переименования
        public  string originalFilePath { get; set; }

        // Можно использовать для хранения пути к файлу, который будет отправлен
        public  string filePath { get; set; }
    }
}
