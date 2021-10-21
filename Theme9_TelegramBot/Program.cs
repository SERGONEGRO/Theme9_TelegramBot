using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

// Создать бота, позволяющего принимать разные типы файлов, 
// *Научить бота отправлять выбранный файл в ответ
// 
// https://data.mos.ru/
// https://apidata.mos.ru/
// 
// https://vk.com/dev
// https://vk.com/dev/manuals

// https://dev.twitch.tv/
// https://discordapp.com/developers/docs/intro
// https://discordapp.com/developers/applications/
// https://discordapp.com/verification


namespace Theme9_TelegramBot
{
    class Program
    {
        static TelegramBotClient bot;
        static string path = @"E:\\bot\";

        static void Main(string[] args)
        {
            string token = File.ReadAllText(@"D:\programms\Яндекс диск\Синхронизация\YandexDisk\token1.txt");

            bot = new TelegramBotClient(token);
            bot.SendTextMessageAsync($"Приветствую тебя, {e.Message.Chat.FirstName}",
            bot.OnMessage += MessageListener;
            bot.StartReceiving();
            Console.ReadKey();
        }
        private static void MessageListener(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            string text = $"{DateTime.Now.ToLongTimeString()}: {e.Message.Chat.FirstName} {e.Message.Chat.Id} {e.Message.Text}";

            Console.WriteLine($"{text} TypeMessage: {e.Message.Type.ToString()}");

            var messageText = "Вышли мне что нибудь";    //для ответа
            try
            {
                switch (e.Message.Type)
                {
                    case Telegram.Bot.Types.Enums.MessageType.Audio:
                        {
                            string pathAudio = path + e.Message.Audio.FileName;
                            DownLoad(e.Message.Audio.FileId, pathAudio);
                            messageText = "Файл " + e.Message.Audio.FileName +
                                        "\nТип " + e.Message.Type +
                                     ", Размер " + e.Message.Audio.FileSize +
                            " байт \nЗагружен в " + pathAudio;
                            break;
                        };
                    case Telegram.Bot.Types.Enums.MessageType.Document:
                        {
                            string pathDocument = path + e.Message.Document.FileName;
                            DownLoad(e.Message.Document.FileId, pathDocument);
                            messageText = "Файл " + e.Message.Document.FileName +
                                        "\nТип " + e.Message.Type +
                                     ", Размер " + e.Message.Document.FileSize +
                            " байт \nЗагружен в " + pathDocument;
                            break;
                        };
                    case Telegram.Bot.Types.Enums.MessageType.Video:
                        {
                            string pathVideo = path + e.Message.Video.FileName;
                            DownLoad(e.Message.Video.FileId, pathVideo);
                            messageText = "Файл " + e.Message.Video.FileName +
                                        "\nТип " + e.Message.Type +
                                     ", Размер " + e.Message.Video.FileSize +
                            " байт \nЗагружен в " + pathVideo;
                            break;
                        };
                    case Telegram.Bot.Types.Enums.MessageType.Sticker:
                        {
                            string pathSticker = path + e.Message.Sticker.Emoji;
                            DownLoad(e.Message.Sticker.FileId, pathSticker);
                            messageText = "Файл " + e.Message.Sticker.Emoji +
                                        "\nТип " + e.Message.Type +
                                     ", Размер " + e.Message.Sticker.FileSize +
                            " байт \nЗагружен в " + pathSticker;
                            break;
                        };
                    default:
                        {
                            messageText = "Такие файлы я еще не могу принимать!";
                            break;
                        }

                        //Где ID и NAME,...?
                    //case Telegram.Bot.Types.Enums.MessageType.Photo:
                    //    {
                    //        string pathPhoto = path + e.Message.Photo;
                    //        DownLoad(e.Message.Photo., pathPhoto);
                    //        messageText = "Файл" + e.Message.Photo.Length +
                    //                    "\nТип " + e.Message.Type +
                    //                 ", Размер " + e.Message.Photo.Length +
                    //        "байт \nЗагружен в " + pathPhoto;
                    //        break;
                    //    };

                }
            }
            catch (Exception ex)
            {
                messageText = ex.Message;
                throw;
            }
            
         
            //if (e.Message.Text == null) return;

            
            bot.SendTextMessageAsync(e.Message.Chat.Id, messageText);
        }

        static async void DownLoad(string fileId, string path)
        {
            try
            {
                var file = await bot.GetFileAsync(fileId);
                FileStream fs = new FileStream(path, FileMode.Create);
                await bot.DownloadFileAsync(file.FilePath, fs);
                fs.Close();

                fs.Dispose();
            }
            catch (Exception ex)
            {
                if (ex != null) Console.WriteLine(ex.Message);
            }
            
        }

    }
}
