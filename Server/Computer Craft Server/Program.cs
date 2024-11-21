using System.Collections;
using System.Reflection;

namespace HTTP_Server {

    static class Program
    {
        public static string dataPath = Path.Combine(Assembly.GetExecutingAssembly().Location, "..", "Data");
        public static string chatLogPath =  Path.Combine(dataPath, "chat.log");
        public static string userDataPath = Path.Combine(dataPath, "Users");
        public static DiscordManager discord;
        public static List<User> allUsers = new List<User>();
        public static WebSocketListener webSocket;
        static void Main()
        {
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            if (!Directory.Exists(userDataPath)) Directory.CreateDirectory(userDataPath);
            if (!File.Exists(Program.chatLogPath))
            {
                Logs log = new Logs() {messages = new string[]{"Chat log started"} };
                Serializer.SerializeToFile(chatLogPath, log);
            }
            Task.Run(() => { new Listener(); });
            discord = new DiscordManager();
            discord.StartBotAsync();
            while (true)
            {
                if(webSocket != null)
                webSocket.messageQueue.Enqueue(new ChatMessage() {owner = "Erag0n001", message = Console.ReadLine() });
            }
        }
    }
}