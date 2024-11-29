using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace HTTP_Server
{
    public class Listener
    {
        HttpListener listener = new HttpListener();
        public Listener()
        {
            if(Program.networkConfig.ip == "0.0.0.0")
                listener.Prefixes.Add($"http://+:{Program.networkConfig.port}/"); //Run as admin
            else
                listener.Prefixes.Add($"http://{Program.networkConfig.ip}:{Program.networkConfig.port}/"); //Run as admin
            listener.Start();
            try
            {
                Task.Run(() => { Listen(); });
            }
            catch (Exception ex) { Debug.Write(ex); }
        }

        public void Listen()
        {
            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                if (context.Request.IsWebSocketRequest)
                {
                    new WebSocketListener(context);
                }
                else 
                {
                    HTTPManager.HandleRequest(context);
                }
            }
        }
    }
}