# .NET-bot
A .NET 7 console application that allows you to interface with ChatGPT-3.5-turbo

## How does it work?
Using the Betalgo.OpenAI.GPT3 nuget package, we are able to interface with OpenAI and use it's various models. In the future, I may allow the use of other models (besides CGPT-3.5-turbo) but for now this is all. The bot is currently designed to listen to any of your wishes and essentially play out any scenario you desire.

- You **NEED** an API Key for this to work. You can get one here: https://platform.openai.com/account/api-keys

## Where do I download?
Download any of the corresponding releases based on your OS. [Click here to visit the release page.](https://github.com/RenderBr/.NET-bot/releases/)

### Current Features
 - Name changing - use !name <name> to change the bot's name
 - Gender changing - use !gender <female/male> to change the bot's gender
 - Remembers context on given instance, it will forget everything once you close the app
 - Conversation logging - in the folder, a log file will be generated for each instance
 - Ability to switch models

### What's planned?
 - Context concurrency after you close the app - loadable profiles that allow the bot to remember previous conversations
 - Ability to run a web server and recieve/send RESTful requests and responses, to utilize later on for other projects/web apps
 - QoL - make things more polished
 - More customization options related to how the bot perceives itself and you
 - More model implementations
