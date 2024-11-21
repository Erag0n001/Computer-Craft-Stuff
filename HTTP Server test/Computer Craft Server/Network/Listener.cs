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
            listener.Prefixes.Add("http://192.168.0.151:25565/"); //Run as admin
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