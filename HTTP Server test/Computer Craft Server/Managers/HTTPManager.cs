using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HTTP_Server
{
    public static class HTTPManager
    {
        public static void HandleRequest(HttpListenerContext context) 
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            string action = "null";
            int id = 0;
            if (request.Headers.Keys.Count > 0)
            {
                //try
                //{
                //    id = Int32.Parse(request.Headers["id"]);
                //} catch (FormatException ex) 
                //{
                //    Console.WriteLine($"Device came with unknown or non standard id\n{ex}");
                //    continue;
                //}
                action = request.Headers["action"];
            }
            //Console.WriteLine($"Received {request.HttpMethod} with id {id} with action {action}");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Methods", "POST, GET, OPTIONS");
            response.Headers.Add("Access-Control-Allow-Headers", "Content-Type");
            string responseString = "";
            if (request.HttpMethod == "POST")
            {
                DoPost();
            }
            if (request.HttpMethod == "GET")
            {
                DoGet();
            }

            void DoGet()
            {
                switch (action)
                {
                    case "Get-Visits-From-User":
                        if (request.Headers.AllKeys.Length < 4)
                        {
                            Console.WriteLine("Request was missing headers, requires 'id', 'action', 'username', 'place'");
                            break;
                        }
                        string toGet = request.Headers[request.Headers.AllKeys[2]];
                        User user = Program.allUsers.FirstOrDefault(N => N.name == toGet);
                        if (user == null)
                        {
                            Console.WriteLine($"Creating new user {toGet}");
                            user = new User()
                            {
                                id = Program.allUsers.Count + 1,
                                name = toGet,
                                visitedPlaces = new Dictionary<string, int>()
                            };
                            Program.allUsers.Add(user);
                        }
                        toGet = request.Headers[request.Headers.AllKeys[3]];

                        if (!user.visitedPlaces.ContainsKey(toGet))
                        {
                            user.visitedPlaces.Add(toGet, 0);
                        }

                        user.visitedPlaces[toGet] += 1;
                        responseString = user.visitedPlaces[toGet].ToString();

                        Serializer.SerializeToFile(Path.Combine(Program.userDataPath, $"{user.name}.user"), user);
                        break;
                }

                if (!string.IsNullOrEmpty(responseString))
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                    response.OutputStream.Close();
                    response.Close();
                }
            }

            void DoPost()
            {
                switch (action)
                {
                    case "Log":
                        using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                        {
                            string requestBody = reader.ReadToEnd();
                            ChatMessage dataToLog = JsonConvert.DeserializeObject<ChatMessage>(requestBody);
                            Logs logs = Serializer.SerializeFromFile<Logs>(Program.chatLogPath);
                            List<string> currentLogs = logs.messages.ToList();
                            currentLogs.Add($"<{dataToLog.owner}> {dataToLog.message}");
                            logs.messages = currentLogs.ToArray();
                            Serializer.SerializeToFile(Program.chatLogPath, logs);
                            Console.WriteLine($"<{dataToLog.owner}> {dataToLog.message}");
                            Program.discord.SendMessage(dataToLog);
                            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                            response.OutputStream.Write(buffer, 0, buffer.Length);
                            response.OutputStream.Close();
                            response.Close();
                        }
                        break;
                }
            }
        }
    }
}
