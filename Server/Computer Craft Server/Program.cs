using System.Collections;
using System.Reflection;

namespace HTTP_Server {

    static class Program
    {
        public static string dataPath = Path.Combine(Directory.GetCurrentDirectory(), "Data");
        public static string chatLogPath =  Path.Combine(dataPath, "chat.log");
        public static string userDataPath = Path.Combine(dataPath, "Users");

        public static string configPath = Path.Combine(Assembly.GetExecutingAssembly().Location, "Configs");
        public static string networkConfigPath = Path.Combine(dataPath, "network.config");

        public static List<User> allUsers = new List<User>();
        public static WebSocketListener webSocket;
        public static NetworkConfig networkConfig;
        static void Main()
        {
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            if (!Directory.Exists(userDataPath)) Directory.CreateDirectory(userDataPath);
            if (!Directory.Exists(userDataPath)) Directory.CreateDirectory(configPath);
            if (!File.Exists(Program.chatLogPath))
            {
                Logs log = new Logs() {messages = new string[]{"Chat log started"} };
                Serializer.SerializeToFile(chatLogPath, log);
            }
            if (!File.Exists(networkConfigPath)) 
            {
                NetworkConfig config = new NetworkConfig();
                Serializer.SerializeToFile(networkConfigPath, config);
            }
            networkConfig = Serializer.SerializeFromFile<NetworkConfig>(networkConfigPath);
            Console.WriteLine($"Server listening on {networkConfig.ip}:{networkConfig.port}");
            Task.Run(() => { new Listener(); });
            DiscordManager.StartBotAsync();
            while (true)
            {
                if(webSocket != null)
                webSocket.messageQueue.Enqueue(new ChatMessage() {owner = "Erag0n001", message = Console.ReadLine() });
            }
        }
    }
}