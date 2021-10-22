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
    public class DialogflowManager
    {

        private string _userID;
        private string _webRootPath;
        private string _contentRootPath;
        private string _projectId;
        private SessionsClient _sessionsClient;
        private SessionName _sessionName;

        public DialogflowManager(string userID, string webRootPath, string contentRootPath, string projectId)
        {
            _userID = userID;
            _webRootPath = webRootPath;
            _contentRootPath = contentRootPath;
            _projectId = projectId;
            SetEnvironmentVariable();
        }

        private void SetEnvironmentVariable()
        {
            try
            {
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", _contentRootPath + "\\Keys\\{THE_DOWNLOADED_JSON_FILE_HERE}.json");
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (SecurityException)
            {
                throw;
            }
        }

        private async Task CreateSession()
        {
            // Create client
            _sessionsClient = await SessionsClient.CreateAsync();
            // Initialize request argument(s)
            _sessionName = new SessionName(_projectId, _userID);
        }

        public async Task<QueryResult> CheckIntent(string userInput, string LanguageCode = "en")
        {
            await CreateSession();
            QueryInput queryInput = new QueryInput();
            var queryText = new TextInput();
            queryText.Text = userInput;
            queryText.LanguageCode = LanguageCode;
            queryInput.Text = queryText;

            // Make the request
            DetectIntentResponse response = await _sessionsClient.DetectIntentAsync(_sessionName, queryInput);
            return response.QueryResult;

        }

    }



   //И тогда это можно назвать так, например, чтобы обнаружить Intents

     DialogflowManager dialogflow = new DialogflowManager("{INSERT_USER_ID}",

    _hostingEnvironment.WebRootPath,

    _hostingEnvironment.ContentRootPath,

    "{INSERT_AGENT_ID");


    var dialogflowQueryResult = await dialogflow.CheckIntent("{INSERT_USER_INPUT}");
    class Program
    {
        static TelegramBotClient bot;
        static string path = @"D:\\bot\";
        static ApiAi apiAi;

        static void Main(string[] args)
        {
            //string token = File.ReadAllText(@"D:\programms\Яндекс диск\Синхронизация\YandexDisk\token1.txt");
            string tokentg = System.IO.File.ReadAllText(@"C:\Users\User\YandexDisk\token1.txt");
            //string tokenAi = System.IO.File.ReadAllText(@"small-talk-lckd-8c1d6b8922a0.json");
            string dialogFlowKeyFile = @"small-talk-lckd-8c1d6b8922a0.json";
            
            var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(System.IO.File.ReadAllText(dialogFlowKeyFile));
            var projectID = dic["project_id"];
            var sessionID = dic["private_key_id"];
           
            var dialogFlowBuilder = new SessionsClientBuilder
            {
                CredentialsPath = dialogFlowKeyFile
            };
            var dialogFlowClient = dialogFlowBuilder.Build();
           
           

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

        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            string buttonText = e.CallbackQuery.Data;
            string name = $"{e.CallbackQuery.From.FirstName} {e.CallbackQuery.From.LastName}";
            Console.WriteLine($"{name} нажал кнопку {buttonText}");

            await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id, $"Вы нажали кнопку {buttonText}");
        }

        private static async void MessageListener(object sender, MessageEventArgs e)
        {
            var message = e.Message;
            string name = $"{message.From.FirstName} {message.From.LastName}";

            string text = $"{DateTime.Now.ToLongTimeString()}:<< {name} {message.Chat.Id}  *{message.Text}*";

            Console.WriteLine($"{text} TypeMessage: {message.Type.ToString()}");

            var messageText = "Вышли мне что нибудь";    //для ответа
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
                    default:
                        {
                            messageText = "Такие файлы я еще не могу принимать!";
                            break;
                        }

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
                            break;
                        };

                    case MessageType.Text:  //Если текст, то :
                        {
                            switch (message.Text)
                            {
                                case "/start":
                                    messageText = @"ВАС ПРИВЕТСВУЕТ MEGABOT
Cписок команд:
/start - запуск бота
/inline - вывод меню
/keyboard - вывод клавиатуры";
                                   await bot.SendTextMessageAsync(message.From.Id, messageText); //Информационное сообщение в чат

                                    break;

                                case "/inline":
                                    var inlinekeyboard = new InlineKeyboardMarkup(new[]
                                    {
                                        new[]
                                        {
                                            InlineKeyboardButton.WithUrl("MySite","http://nice-honey.com/"),
                                            InlineKeyboardButton.WithUrl("Telegram","https://t.me/sergonegro"),
                                        },
                                        new[]
                                        {
                                            InlineKeyboardButton.WithCallbackData("Пункт 1"),
                                            InlineKeyboardButton.WithCallbackData("Пункт 2")
                                        }
                                    });
                                    messageText = "Выберите пункт меню:";
                                    await bot.SendTextMessageAsync(message.Chat.Id, messageText,
                                                                    replyMarkup:inlinekeyboard);   //Информационное сообщение в чат
                                    break;

                                case "/keyboard":
                                    var replyKeyboard = new ReplyKeyboardMarkup(new[]
                                    {
                                        new[]
                                        {
                                            new KeyboardButton("Привет!"),
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

                                default:

                                    DialogflowManager dialogflow = new DialogflowManager("{INSERT_USER_ID}",
                                    //var response = 
                                    //var response = apiAi.TextRequest(message.Text);
                                    //string answer = response.Result.Fulfillment.Speech;
                                    //if (answer == "")
                                    //    answer = "Сорян, не понял тебя.";
                                    //await bot.SendTextMessageAsync(message.Chat.Id, answer);   //Информационное сообщение в чат
                                    break;
                            }

                            break;
                        }

                }
            }
            catch (Exception ex)
            {
                messageText = ex.Message;
                throw;
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

    }
}
