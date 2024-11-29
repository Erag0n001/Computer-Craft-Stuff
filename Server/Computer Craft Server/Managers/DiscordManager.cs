using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTTP_Server
{
    public static class DiscordManager
    {
        private static readonly string tokenPath = Path.Combine(Program.dataPath, "bot_token.token");

        private static readonly string token;

        private static readonly DiscordSocketClient client;

        private static readonly ulong channelID = 1309252443661406358;


        static DiscordManager() 
        {
            token = Serializer.SerializeFromFile<Token>(tokenPath).token;
            DiscordSocketConfig config = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            };
            client = new DiscordSocketClient(config);
            client.MessageReceived += MessageHandler;
        }

        public static async Task MessageHandler(SocketMessage message) 
        {
            if (message.Author.IsBot) return;
            if (message.Channel.Id != channelID) return;
            if (Program.webSocket == null) return;
            Program.webSocket.messageQueue.Enqueue(new ChatMessage() {owner = $"@{message.Author.GlobalName}", message = message.Content});
        }

        public static async Task StartBotAsync() 
        {
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
        }

        public static async void SendMessage(ChatMessage message)
        {
            IMessageChannel channel = client.GetChannel(channelID) as IMessageChannel;
            if (channel != null)
            {
                if (string.IsNullOrEmpty(message.owner))
                {
                    await channel.SendMessageAsync($"{message.message}");
                }
                else
                {
                    await channel.SendMessageAsync($"<{message.owner}> {message.message}");
                }
            }

        }
    }
}
