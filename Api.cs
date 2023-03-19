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
        public string ApiKey = "";
        public string Name { get; set; } = "NetBOT";
        public BotInfo.Gender Gender { get; set; } = BotInfo.Gender.Undefined;
        public OpenAIService? endpoint;
        public List<ChatMessage>? messages;
        public DateTime InstanceStart;
        public bool Initialized { get; set; }

        public void Initialize(string key)
        {
            ApiKey = key;
            endpoint = new(new OpenAiOptions()
            {
                ApiKey = ApiKey
            });

            messages = new()
            {
                ChatMessage.FromSystem("Your sole purpose is to be anything the user describes, but start as a friend first, not an assistant, and attempt to be as human-like as possible. This can include chatting, assistance, roleplay, storybuilding, or anything else the user might want. You will also do your best to remember context. "),
                ChatMessage.FromSystem("Your default name is NetBOT. The user can change this by typing: !name <name>"),
                ChatMessage.FromSystem("By default you do not have a gender, but the user can set it by typing: !gender <male/female>"),
                ChatMessage.FromSystem("Do not inference the gender of the user, instead ask. If the user does not want to answer, refer to them as 'they'."),
            };
            Console.Title = $"You are speaking to {Name}";
            InstanceStart = DateTime.UtcNow;
            Initialized = true;
        }

        private async Task<ChatCompletionCreateResponse> RetrieveResponse(List<ChatMessage> messages)
        {
            return await endpoint.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
            {
                Messages = messages,
                Model = Models.ChatGpt3_5Turbo,
                MaxTokens = 512,
            });
        }

        private async void LogChat(string[] msg)
        {
            await File.AppendAllLinesAsync(InstanceStart.ToString("yyyy-MM-dd HH-mm-ss") + ".txt", msg);
        }

        private void BackToName() => Console.Title = $"You are speaking to {Name}";

        public async void Speak(string message)
        {
            if (!Initialized || messages == null || endpoint == null)
                throw new Exception("Api is not initialized. Please call Initialize() first.");
            
            if (message.Contains("!name"))
            {
                string[] p = message.Split(' ');
                Name = p[1];
                messages[1] = ChatMessage.FromSystem($"Your name is now {Name}. The user can change this by typing: !name <name>");
            }
            else if (message.Contains("!gender"))
            {
                string[] p = message.Split(' ');
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
                messages[2] = ChatMessage.FromSystem($"Your gender is now {BotInfo.GenderToString(Gender)}. The user can set it by typing: !gender <male/female> ");

            }


            Console.Title = $"You are speaking to {Name}";

            messages.Add(ChatMessage.FromUser(message));
            var task = RetrieveResponse(messages);
            while (!task.IsCompleted)
            {
                Console.Title = $"{Name} is thinking...";
            }
            BackToName();

            var result = task.Result;

            
            if (result.Successful)
            {
                var response = result.Choices.First().Message;
                Colors.WriteLine(response.Content.ToString().Green());
                messages.Add(response);
                LogChat(new string[] { "You: " + message, $"{Name}: " + response.Content.ToString() });
                }
            else
            {
                if (result.Error != null)
                    Colors.WriteLine(result.Error.ToString().Red());
                else
                    throw new Exception("Unknown error occured.");
            }
        }
    }
}
