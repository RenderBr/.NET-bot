
using Alba.CsConsoleFormat.Fluent;
using Config.Net;
using System.Configuration;
using System.Reflection;

namespace OpenAI
{
    internal class Program
    {
        public static Api? api;
        
        static void Main(string[] args)
        {
            api = new Api();
            IConfiguration settings = new ConfigurationBuilder<IConfiguration>()
           .UseJsonFile("openai.json")
           .Build();

            string ApiKey = "";

            if (string.IsNullOrEmpty(settings.ApiKey))
            {
                Colors.WriteLine("Please enter your OpenAI ApiKey: ".Gray());
                while (string.IsNullOrEmpty(ApiKey))
                {
                    ApiKey = Console.ReadLine();
                    settings.ApiKey = ApiKey;
                }
            }
            else
            {
                ApiKey = settings.ApiKey;
            }

            Console.Clear();
            api.Initialize(ApiKey);
            Colors.WriteLine("Please enter your initial message for NETBOT:".Gray());

            while (true)
            {
                var message = Console.ReadLine();
                if (String.IsNullOrEmpty(message))
                {
                    Colors.WriteLine("Your message cannot be blank!".Yellow());
                    continue;
                }
                api.Speak(message);
            }
            
        }

    }
}