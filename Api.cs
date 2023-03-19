using Alba.CsConsoleFormat.Fluent;
using OpenAI.Enums;
using OpenAI.GPT3;
using OpenAI.GPT3.Managers;
using OpenAI.GPT3.ObjectModels;
using OpenAI.GPT3.ObjectModels.RequestModels;
using OpenAI.GPT3.ObjectModels.ResponseModels;

namespace OpenAI
{
    public class Api
    {
        #region Data
        public string ApiKey = ""; // api key that you get from openai.com

        /// <summary>
        /// The name that will be used for the bot, default: NetBOT
        /// </summary>
        public string Name { get; set; } = "NetBOT";

        public List<Model> SupportedModels = new List<Model>()
            {
                new Model(Models.ChatGpt3_5Turbo, "1", "(recommended) - $0.002 / 1K tokens"),
                new Model(Models.ChatGpt3_5Turbo0301, "2"),
                new Model(Models.Gpt4, "3", "(not available to all)")

            };

        /// <summary>
        /// The gender that will be used for the bot, default: BotInfo.Gender.Undefined
        /// </summary>
        public BotInfo.Gender Gender { get; set; } = BotInfo.Gender.Undefined;
        private OpenAIService? endpoint;
        public string PreferredModel { get; set; } = Models.ChatGpt3_5Turbo;

        /// <summary>
        ///  The conversation history along with initial instructions for bot
        /// </summary>
        public List<ChatMessage>? messages;

        /// <summary>
        /// The time when the instance started, used for the log files
        /// </summary>
        public DateTime InstanceStart;

        /// <summary>
        /// True/false, whether or not the bot has been initialized yet.
        /// </summary>
        public bool Initialized { get; set; }
        #endregion

        #region Initialization
        /// <summary>
        /// Initialize the bot, and send initial instructions. Initialized will equal true after this event
        /// </summary>
        /// <param name="key">OpenAI Api Key</param>
        public void Initialize(string key)
        {
            // set api key value
            ApiKey = key;

            // create endpoint
            endpoint = new(new OpenAiOptions()
            {
                ApiKey = ApiKey
            });

            // initial instructions and definition for message history
            messages = new()
            {
                ChatMessage.FromSystem("Your sole purpose is to be anything the user describes, but start as a friend first, not an assistant, and attempt to be as human-like as possible. This can include chatting, assistance, roleplay, storybuilding, or anything else the user might want. You will also do your best to remember context. "),
                ChatMessage.FromSystem("Your default name is NetBOT. The user can change this by typing: !name <name>"),
                ChatMessage.FromSystem("By default you do not have a gender, but the user can set it by typing: !gender <male/female>"),
                ChatMessage.FromSystem("Do not inference the gender of the user, instead ask. If the user does not want to answer, refer to them as 'they'."),
            };

            // sets the console title
            BackToName();

            // logs initial instance start time
            InstanceStart = DateTime.UtcNow;

            // tells the rest of the program the API has been initialized
            Initialized = true;
        }
        #endregion

        /// <summary>
        /// Retrieves the next response from OpenAI endpoint
        /// </summary>
        /// <param name="messages">Current message history</param>
        /// <returns>The chat completion response, or the next message from the chat bot</returns>
        private async Task<ChatCompletionCreateResponse> RetrieveResponse(List<ChatMessage> messages)
        {
            // throws exception if endpoint is null, should never actually occur but implemented to remove warnings
            if (endpoint == null)
            {
                throw new Exception("The API endpoint is null or could not be found! Is everything set up correctly? Check your API Key.");
            }

            // returns the next response based on message history
            // this is also where you can set a model or max tokens for the response
            return await endpoint.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
            {
                Messages = messages,
                Model = PreferredModel,
                MaxTokens = 512,
            });
        }

        /// <summary>
        /// Logs a chat message to the log file
        /// </summary>
        /// <param name="msg">The message to be logged</param>
        private async void LogChat(string[] msg)
        {
            // writes to current instance file, with message
            await File.AppendAllLinesAsync(InstanceStart.ToString("yyyy-MM-dd HH-mm-ss") + ".txt", msg);
        }

        /// <summary>
        /// Returns the Console.Title back to the default
        /// </summary>
        private void BackToName() => Console.Title = $"You are speaking to {Name}";

        /// <summary>
        /// Retrieve a list of models and print them out to the console.
        /// </summary>
        public void RetrieveModels()
        {
            if (endpoint == null)
                return;

            foreach (Model m in SupportedModels)
            {
                Console.WriteLine(m.InternalId + " - " + m.ModelId + (!string.IsNullOrEmpty(m.AdditionalNotes) ? $" - {m.AdditionalNotes}" : ""));
            }
        }

        /// <summary>
        /// The main method for generating a response
        /// </summary>
        /// <param name="message">The input that is fed to the bot by user</param>
        /// <exception cref="Exception">If the API is not initialized, this error will be thrown.</exception>
        public void Speak(string message)
        {
            // if not yet initialized, throw exception
            if (!Initialized || messages == null || endpoint == null)
                throw new Exception("Api is not initialized. Please call Initialize() first.");


            // if user enters !name command
            if (message.Contains("!name"))
            {
                // split string into array
                string[] p = message.Split(' ');
                // set Name variable to second string, which should be the name parameter
                Name = p[1];
                // inform the bot that it's name has changed
                messages[1] = ChatMessage.FromSystem($"Your name is now {Name}. The user can change this by typing: !name <name>");
                // reset title to accomodate name change
                BackToName();
            }
            // if the user enters !gender command
            else if (message.Contains("!gender"))
            {
                // split string into array
                string[] p = message.Split(' ');
                // based on input, assign gender
                switch (p[1])
                {
                    case "male":
                        Gender = BotInfo.Gender.Male;
                        break;
                    case "female":
                        Gender = BotInfo.Gender.Female;
                        break;
                    default:
                        Gender = BotInfo.Gender.Undefined;
                        break;
                }
                // inform bot on gender change
                messages[2] = ChatMessage.FromSystem($"Your gender is now {BotInfo.GenderToString(Gender)}. The user can set it by typing: !gender <male/female> ");

            }

            // add latest user response to message history
            messages.Add(ChatMessage.FromUser(message));
            // get next response based on new message & prev history
            var task = RetrieveResponse(messages);
            // while the response is being waited on, set new window title.
            while (!task.IsCompleted)
            {
                Console.Title = $"{Name} is thinking...";
            }
            // after, revert back to default title
            BackToName();

            // retrieve the result of response
            var result = task.Result;

            // if it was successful, 
            if (result.Successful)
            {
                // retrieve first choice for response
                var response = result.Choices.First().Message;
                // output to console
                Colors.WriteLine(response.Content.ToString().Green());
                // add bot message to history
                messages.Add(response);
                // log to file
                LogChat(new string[] { "You: " + message, $"{Name}: " + response.Content.ToString() });
            }
            else // if not successful
            {
                // and the error is not null
                if (result.Error != null)
                    Colors.WriteLine(result.Error.ToString().Red()); // output error
                else
                    throw new Exception("Unknown error occured."); // if there is no error, output unknown error exception
            }
        }
    }
}
