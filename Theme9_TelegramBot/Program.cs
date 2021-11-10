using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using ApiAiSDK;
using ApiAiSDK.Util;
using ApiAiSDK.Model;
using Newtonsoft.Json;
using Google.Cloud.Dialogflow.V2;
using Environment = System.Environment;

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
    //    //И тогда это можно назвать так, например, чтобы обнаружить Intents
    //    DialogFlowManager dialogflow = new DialogFlowManager("{INSERT_USER_ID}",
    //   _hostingEnvironment.WebRootPath,
    //   _hostingEnvironment.ContentRootPath,
    //   "{INSERT_AGENT_ID");
    //    var dialogflowQueryResult = await dialogflow.CheckIntent("{INSERT_USER_INPUT}");

        static TelegramBotClient bot;
        static string path = @"D:\\bot\";
        static ApiAi apiAi;
        static DirectoryInfo directoryInfo = new DirectoryInfo(path);
        static bool flag = false;   //flag = true, если ожидается ответ пользователя

        static void Main(string[] args)
        {
            //string tokentg = System.IO.File.ReadAllText(@"D:\programms\Яндекс диск\Синхронизация\YandexDisk\token1.txt");
            string tokentg = System.IO.File.ReadAllText(@"C:\Users\User\YandexDisk\token1.txt");
            //string tokenAi = System.IO.File.ReadAllText(@"small-talk-lckd-8c1d6b8922a0.json");
            //string dialogFlowKeyFile = @"small-talk-lckd-8c1d6b8922a0.json";
            
            //var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(System.IO.File.ReadAllText(dialogFlowKeyFile));
            //var projectID = dic["project_id"];
            //var sessionID = dic["private_key_id"];
           
            //var dialogFlowBuilder = new SessionsClientBuilder
            //{
            //    CredentialsPath = dialogFlowKeyFile
            //};
            //var dialogFlowClient = dialogFlowBuilder.Build();
           
           

            //AIConfiguration config = new AIConfiguration(tokenAi, SupportedLanguage.Russian);
            //apiAi = new ApiAi(config);

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
            //if (buttonText == "GetFileList")
            //{
            //    GetDir(path).ToString();
            ////    await bot.SendTextMessageAsync(e.CallbackQuery.Id, GetDir(path).ToString());   //Информационное сообщение в чат
            ////    await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, GetDir(path).ToString(), true);
            //}
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

            Console.WriteLine($"{text} TypeMessage: {message.Type.ToString()}");

            var messageText = @"ВАС ПРИВЕТСВУЕТ MEGABOT
Вышли мне файл и я вышлю его тебе обратно!
Cписок команд:
/start - запуск бота
/inline - вывод меню
/keyboard - вывод клавиатуры
/ShowMeFiles - просмотр файлов в наличии
/GetFile - получить файл" + "\n"; 

            try
            {
                switch (message.Type)
                {
                    case MessageType.Audio:   //Если пришло аудио
                        {
                            string pathAudio = path + message.Audio.FileName;
                            DownLoad(message.Audio.FileId, pathAudio);
                            messageText = "Файл " + message.Audio.FileName +
                                        " Тип " + message.Type +
                                     ", Размер " + message.Audio.FileSize +
                            " байт, Загружен в " + pathAudio;
                            
                            break;
                        };
                    case MessageType.Document:   //Если пришло документ
                        {
                            string pathDocument = path + message.Document.FileName;
                            DownLoad(message.Document.FileId, pathDocument);
                            messageText = "Файл " + message.Document.FileName +
                                        " Тип " + message.Type +
                                     ", Размер " + message.Document.FileSize +
                            " байт, Загружен в " + pathDocument;
                            break;
                        };
                    case MessageType.Video:    //Если пришло видео
                        {
                            if (e.Message.Video.FileSize < 20000000)
                            {
                                string fileNameVideo = message.Video.FileUniqueId + ".mp4";
                                string pathVideo = path + fileNameVideo;
                                DownLoad(message.Video.FileId, pathVideo);

                                messageText = "Файл " + fileNameVideo +
                                            " Тип " + message.Type +
                                         ", Размер " + message.Video.FileSize +
                                " байт, Загружен в " + pathVideo;
                            }
                            else
                            {
                                messageText = "Неосилю, слишком большой файл!";
                            }

                            break;
                        };
                    case MessageType.Sticker:            //Если пришло стикер
                        {
                            string pathSticker = path + message.Sticker.Emoji;
                            DownLoad(message.Sticker.FileId, pathSticker);
                            messageText = "Файл " + message.Sticker.Emoji +
                                        "Тип " + message.Type +
                                     ", Размер " + message.Sticker.FileSize +
                            " байт, Загружен в " + pathSticker;
                            break;
                        };

                    //Photo
                    case MessageType.Photo:       ////Если пришло фото
                        {
                            string fileNamePhoto = message.Photo[message.Photo.Length - 1].FileUniqueId + ".jpeg";
                            string fileIdPhoto = message.Photo[message.Photo.Length - 1].FileId;
                            string pathPhoto = path + fileNamePhoto;
                            DownLoad(fileIdPhoto, pathPhoto);
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
                                case "/start":
                                    await bot.SendTextMessageAsync(message.From.Id, messageText); //Информационное сообщение в чат
                                    break;

                                case "/inline":    //инлайн-клавиатура(тест)
                                    var inlinekeyboard = new InlineKeyboardMarkup(new[]
                                    {
                                        new[]
                                        {
                                            InlineKeyboardButton.WithUrl("MySite","http://nice-honey.com/"),
                                            InlineKeyboardButton.WithUrl("Telegram","https://t.me/sergonegro"),
                                        },
                                        new[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("Просмотр файлов","GetFileList"),
                                            InlineKeyboardButton.WithCallbackData("Выдача файлов")
                                        }
                                    }); ;
                                    messageText = "Выберите пункт меню:";

                                    await bot.SendTextMessageAsync(message.Chat.Id, messageText,
                                                                    replyMarkup: inlinekeyboard);   //Информационное сообщение в чат
                                    break;

                                case "/keyboard":      //кнопки(тест)
                                    var replyKeyboard = new ReplyKeyboardMarkup(new[]
                                    {
                                        new[]
                                        {
                                            new KeyboardButton ("Привет!"),
                                            new KeyboardButton("Пока!")
                                        },
                                        new[]
                                        {
                                            new KeyboardButton("контакт"){RequestContact = true },
                                            new KeyboardButton("Геолокация") {RequestLocation = true}
                                        }
                                    });
                                    messageText = "Сообщение";
                                    await bot.SendTextMessageAsync(message.Chat.Id, messageText,
                                                                    replyMarkup: replyKeyboard);   //Информационное сообщение в чат
                                    break;


                                case "/ShowMeFiles":    //показывает все файлы в директории path

                                    foreach (var item in GetDir(path))
                                    {
                                        messageText += $"{item}\n";
                                    }
                                    await bot.SendTextMessageAsync(message.Chat.Id, messageText);   
                                    break;

                                case "/GetFile":    //запрашивает файл
                                    if (flag == false)
                                    {
                                        flag = true;
                                        messageText = "Введите номер файла, который желаете получить:\n";
                                        await bot.SendTextMessageAsync(message.Chat.Id, messageText);
                                    }
                                    else goto default;
                                    break;

                                //DialogflowManager dialogflow = new DialogflowManager("{INSERT_USER_ID}",
                                //var response = 
                                //var response = apiAi.TextRequest(message.Text);
                                //string answer = response.Result.Fulfillment.Speech;
                                //if (answer == "")
                                //    answer = "Сорян, не понял тебя.";


                                default:
                                    if (flag)
                                    {
                                        int fileNumber = Int32.Parse(message.Text);
                                        
                                        messageText = "Получение файла";
                                        string currentFile = path + GetCurrentFile(fileNumber);
                                        string extension = Path.GetExtension(currentFile); // определяем расширение
                                        await bot.SendTextMessageAsync(message.Chat.Id, currentFile);
                                        switch (extension)
                                        {
                                            //https://github.com/sergshu/LearnTogether/blob/master/TelegramBotIsSimple/TelegraBotHelper.cs

                                            case ".mp3":
                                                using (var stream = System.IO.File.OpenRead(currentFile))
                                                {
                                                    //await bot.SendTextMessageAsync(message.Chat.Id, currentFile);
                                                    await bot.SendAudioAsync(message.Chat.Id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream));
                                                }

                                                //InputMediaAudio audioFile = new InputMediaAudio(currentFile);
                                                ////InputMedia audioFile = new InputMedia(currentFile);
                                                //await bot.SendAudioAsync(message.Chat.Id, currentFile);
                                                break;
                                            case "":
                                                break;
                                            //case "":
                                            //    break;
                                            //case "":
                                            //    break;
                                            //case "":
                                            //    break;
                                            default:
                                                //bot.SendDocumentAsync(message.Chat.Id, currentFile);
                                                break;

                                        }
                                        flag = false;
                                    }
                                    await bot.SendTextMessageAsync(message.Chat.Id, messageText);   //По умолчанию отсылает меню

                                    break;
                            }

                            break;
                        }

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

        static List<string> GetDir(string path, string trim = "")
        {
          
            List<string> files = new List<string>();

            int i = 1;
            foreach (var item in directoryInfo.GetFiles())          // Перебираем все файлы текущего каталога
            {
                string file = $"{trim}{item.Name}";
                files.Add(file);
                i++;
                //Console.WriteLine(file);            // Выводим информацию о них
            }

            return files;
        }

        static string GetCurrentFile(int number)
        {
            List<string> files = GetDir(path);

            return files[number - 1];
        }

    }
}
