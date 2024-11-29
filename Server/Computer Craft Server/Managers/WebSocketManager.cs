using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace HTTP_Server
{
    public class WebSocketListener
    {
        public HttpListenerWebSocketContext webSocketContext;
        public WebSocket socket;
        public bool alive = true;
        public bool closed = false;
        public Queue<ChatMessage> messageQueue = new Queue<ChatMessage>();
        public WebSocketListener(HttpListenerContext context) 
        {
            Program.webSocket = this;
            alive = true;
            DiscordManager.SendMessage(new ChatMessage() { message = "## Server just restarted!" });
            try {
                Task.Run(async () => { HandleSocket(context); });
            } catch ( Exception ex) {Console.WriteLine(ex); }
        }
        public async Task HandleSocket(HttpListenerContext context)
        {
            webSocketContext = await context.AcceptWebSocketAsync(subProtocol: null);
            socket = webSocketContext.WebSocket;
            Task.Run(async () =>
            {
                while (closed == false)
                {
                    await ReceiveMessages();
                    await SendMessages();
                    await Task.Delay(100);
                }
            });
            Task.Run(async () =>
            {
                while (closed == false)
                {
                    await KeepAlive();
                    await Task.Delay(20000);
                }
            });
        }

        public async Task CloseSocket() 
        {
            Console.WriteLine($"Closing WebSocket. Status: {socket.CloseStatus}, Reason: {socket.CloseStatusDescription}");
            DiscordManager.SendMessage(new ChatMessage() { message = "## Server is not responding, could be an automatic restart or a crash" });
            closed = true;
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }

        public async Task SendMessages() 
        {
            if (closed) return;
            if (messageQueue.Count > 0)
            {
                ChatMessage message = messageQueue.Dequeue();
                byte[] bytes = Encoding.UTF8.GetBytes(Serializer.SerializeToString(message));
                await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                Console.WriteLine($"<{message.owner}> {message.message}");
            }
        }

        public async Task ReceiveMessages() 
        {
            if(closed) return;
            byte[] buffer = new byte[1024];
            WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await CloseSocket();
                return;
            }
            else
            {
                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                if (message != "Ping") Console.WriteLine("Received message: " + message);
            }
            alive = true;
        }

        public async Task KeepAlive() 
        {
            if(alive == false) 
            {
                Console.WriteLine("Connection timed out with client");
                await CloseSocket();
                return;
            }
            alive = false;
            // Send a ping to keep the connection alive
            ChatMessage pingMessage = new ChatMessage() {owner = "", message = "Ping" };
            byte[] pingBytes = Encoding.UTF8.GetBytes(Serializer.SerializeToString(pingMessage));
            await socket.SendAsync(new ArraySegment<byte>(pingBytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
