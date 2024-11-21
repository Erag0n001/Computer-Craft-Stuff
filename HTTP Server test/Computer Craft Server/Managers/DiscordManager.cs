using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTTP_Server
{
    public class DiscordManager
    {
        private static readonly string tokenPath = Path.Combine(Program.dataPath, "bot_token.token");

        private readonly string token;

        private readonly DiscordSocketClient client;

        private readonly ulong channelID = 1309252443661406358;


        public DiscordManager() 
        {
            token = Serializer.SerializeFromFile<Token>(tokenPath).token;
            DiscordSocketConfig config = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            };
            client = new DiscordSocketClient(config);
            client.MessageReceived += MessageHandler;
        }

        public async Task MessageHandler(SocketMessage message) 
        {
            if (message.Author.IsBot) return;
            Console.WriteLine(message.Channel.Id);
            if (message.Channel.Id != channelID) return;
            Program.webSocket.messageQueue.Enqueue(new ChatMessage() {owner = $"@{message.Author.GlobalName}", message = message.Content});
        }

        public async Task StartBotAsync() 
        {
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
        }

        public async void SendMessage(ChatMessage message)
        {
            IMessageChannel channel = client.GetChannel(channelID) as IMessageChannel;
            if(channel != null)
                await channel.SendMessageAsync($"<{message.owner}> {message.message}");
        }
    }
}
