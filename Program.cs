
using Alba.CsConsoleFormat.Fluent;
using Config.Net;
using OpenAI.GPT3.ObjectModels;

namespace OpenAI
{
    public class Program
    {
        // nullable static API class, used for interacting with OpenAI api
        public static Api? api;
        static void Main(string[] args) // main  program method
        {
            api = new Api(); // initialize api

            IConfiguration settings = new ConfigurationBuilder<IConfiguration>() // initialize config
           .UseJsonFile("openai.json") // in relative path
           .Build();

            string ApiKey = ""; // temp api var

            if (string.IsNullOrEmpty(settings.ApiKey)) // if api key is empty
            {
                Colors.WriteLine("Please enter your OpenAI ApiKey: ".Gray()); // prompt user for api key
                while (string.IsNullOrEmpty(ApiKey)) // loop if it continues to be invalid
                {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    ApiKey = Console.ReadLine(); // read user input
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                    if (string.IsNullOrEmpty(ApiKey))
                        continue; // loop
                    settings.ApiKey = ApiKey; // set api key
                }
            }
            else
            {
                ApiKey = settings.ApiKey; // set api key
            }



            Console.Clear(); // clear console
            api.Initialize(ApiKey); // init api with api key

            api.RetrieveModels();
            Colors.WriteLine("Please select a model from the list above");
            string preferredModel = Console.ReadLine();

            var tempModel = api.SupportedModels.FirstOrDefault<Model>(x => x.ModelId == preferredModel || x.InternalId == preferredModel, null);
            if (tempModel == null)
            {
                Console.Clear();
                Colors.WriteLine("Invalid model selected, defaulting to ChatGpt3_5Turbo".Yellow());
                api.PreferredModel = Models.ChatGpt3_5Turbo;
            }
            else
            {
                Console.Clear();
                api.PreferredModel = tempModel.ModelId;
                Colors.WriteLine($"You are using: {api.PreferredModel}".Green());
            }

            Colors.WriteLine("Begin speaking to .NET-bot by sending it a message:".DarkGray()); // prompt user for first msg

            while (true) // loop for input
            {
                var message = Console.ReadLine(); // retrieve user input
                if (String.IsNullOrEmpty(message)) // if empty, ask for input
                {
                    Colors.WriteLine("Your message cannot be blank!".Yellow());
                    continue; // and loop again
                }
                api.Speak(message); // send message to api
            }

        }

    }
}