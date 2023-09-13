using System;
using System.IO;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace Theme9_TelegramBot
{
    class Program : UrlGenerator
    {
        static TelegramBotClient bot;
        static string path = @"D:\temp\bot\";

        static void Main(string[] args)
        {
            string tokentg = File.ReadAllText(@"D:\temp\bot\token1.txt");

            bot = new TelegramBotClient(tokentg);
            var me = bot.GetMeAsync().Result;
            Console.WriteLine(me.FirstName);

            bot.OnMessage += MessageListener;
            bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            bot.StartReceiving();
            Console.ReadKey();
        }

        /// <summary>
        /// обработка кнопок
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            string buttonText = e.CallbackQuery.Data;
            string name = $"{e.CallbackQuery.From.FirstName} {e.CallbackQuery.From.LastName}";
            Console.WriteLine($"{name} нажал кнопку {buttonText}");
            await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, $"Вы нажали кнопку {buttonText}");
        }

        /// <summary>
        /// слушатель сообщений
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static async void MessageListener(object sender, MessageEventArgs e)
        {
            var message = e.Message;                                             //полученное сообщение
            string name = $"{message.From.FirstName} {message.From.LastName}";   //имя собеседника

            string text = $"{DateTime.Now.ToLongTimeString()}:<< {name} {message.Chat.Id}  *{message.Text}*";    //для лога

            Console.WriteLine($"{text} TypeMessage: {message.Type}");

            var messageText = @"ВАС ПРИВЕТСВУЕТ MEGABOT
Введи команду и я пришлю тебе пятничный мем!
Cписок команд:
/random - рандомный мем"
//myText - ввести текст
+ "\n";

            try
            {
                switch (message.Type)
                {
                    //Photo
                    case MessageType.Photo:       ////Если пришло фото
                        {
                            string fileNamePhoto = message.Photo[message.Photo.Length - 1].FileUniqueId + ".jpeg";
                            string fileIdPhoto = message.Photo[message.Photo.Length - 1].FileId;
                            string pathPhoto = path + fileNamePhoto;
                            //SaveToDisc(fileIdPhoto, pathPhoto);
                            messageText = "Файл " + fileNamePhoto +
                                        " Тип " + message.Type +
                            // ", Размер " + e.Message.Document.FileSize +   ??
                            ", Загружен в " + pathPhoto;
                            await bot.SendPhotoAsync(message.Chat.Id, fileIdPhoto);
                            break;
                        };

                    case MessageType.Text:  //Если текст, то :
                        {
                            switch (message.Text)
                            {
                                case "/myText":
                                    {
                                        await bot.SendTextMessageAsync(message.From.Id, messageText); //Информационное сообщение в чат
                                        break;
                                    }
                                default:
                                    {
                                        UrlGenerator gen = new UrlGenerator();
                                        string memPath = $"D:\\temp\\bot\\mem-{Guid.NewGuid()}.jpeg";
                                        DownLoad(gen.GenerateURL(), memPath);
                                        using (var fileStream = new FileStream(memPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                                        {
                                            await bot.SendPhotoAsync(
                                                chatId: message.Chat.Id,
                                                photo: new InputOnlineFile(fileStream)
                                            );
                                        }
                                        await bot.SendTextMessageAsync(message.Chat.Id, messageText);   //Информационное сообщение в чат
                                        break;
                                    }
                            }
                            break;
                        };

                    default:
                        {
                            messageText = "Такие файлы я еще не могу принимать!";   //Если файл непонятный:
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                messageText = ex.Message;
                //throw;
            }

            Console.WriteLine($"{DateTime.Now.ToLongTimeString()}:>> {message.Chat.FirstName} {message.Chat.Id} *{messageText}*");//Информационное сообщение в консоль
        }

        private static async void SaveToDisc(string fileIdPhoto, string pathPhoto)
        {
            var file = await bot.GetFileAsync(fileIdPhoto);
            FileStream fs = new FileStream(path, FileMode.Create);
            await bot.DownloadFileAsync(file.FilePath, fs);
            fs.Close();

            fs.Dispose();
        }

        static async void DownLoad(string url, string path)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(url, path);
                }
            }
            catch (Exception ex)
            {
                if (ex != null) Console.WriteLine(ex.Message);
            }
        }
    }
}
