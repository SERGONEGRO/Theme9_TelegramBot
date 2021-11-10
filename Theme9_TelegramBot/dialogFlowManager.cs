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
using System.Security;

namespace Theme9_TelegramBot
{
    class DialogFlowManager
    {
        private string _userID;
        private string _webRootPath;
        private string _contentRootPath;
        private string _projectId;
        private SessionsClient _sessionsClient;
        private SessionName _sessionName;

        public DialogFlowManager(string userID, string webRootPath, string contentRootPath, string projectId)
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
}
