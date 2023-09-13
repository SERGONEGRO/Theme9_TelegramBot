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
            string tokentg = System.IO.File.ReadAllText(@"D:\temp\token1.txt");
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
Введи команду и я пришлю тебе пятничный мем!
Cписок команд:
/random - рандомный мем
/myText - ввести текст" + "\n";

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
                            DownLoad("https://apimeme.com/meme?meme=Neil-deGrasse-Tyson&top=Заполнил табель&bottom=Bitch", "D:\\temp\\mem.jpeg");
                            await bot.SendTextMessageAsync(message.Chat.Id, messageText);   //Информационное сообщение в чат
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

        static async void DownLoad(string url, string path)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(url, path);
                }

                //var file = await bot.GetFileAsync(fileId);
                //FileStream fs = new FileStream(path, FileMode.Create);
                //await bot.DownloadFileAsync(file.FilePath, fs);
                //fs.Close();

                //fs.Dispose();
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
