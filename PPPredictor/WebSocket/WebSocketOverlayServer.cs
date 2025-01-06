using WebSocketSharp;
using WebSocketSharp.Server;

namespace PPPredictor.WebSocket
{
    internal class WebSocketOverlayServer
    {
        private WebSocketServer server;

        public void StartSocket()
        {
            server = new WebSocketServer($"ws://localhost:{Plugin.ProfileInfo.StreamOverlayPort}");
            server.AddWebSocketService<PPPreditorWS>("/socket");
            server.Start();
        }

        public void CloseSocket()
        {
            server.Stop();
        }

        public void SendData(string s)
        {
            if (server != null)
            {
                server.WebSocketServices["/socket"].Sessions.Broadcast(s);
            }
        }
    }

    internal class PPPreditorWS : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
        }
    }
}