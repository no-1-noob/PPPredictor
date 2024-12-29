using Newtonsoft.Json;
using PPPredictor.Core.DataType;
using PPPredictor.Data;
using PPPredictor.Interfaces;
using PPPredictor.Utilities;
using System;
using System.Threading.Tasks;
using WebSocketSharp;
using static PPPredictor.Core.DataType.Enums;

namespace PPPredictor.OpenAPIs
{
    internal class PPPWebSocket<T> : IPPPWebSocket where T : IPPPRawWebsocketData
    {
        private WebSocketSharp.WebSocket webSocket;
        public event EventHandler<PPPScoreSetData> OnScoreSet;
        private string userId = string.Empty;
        private string _leaderboardName = string.Empty;
        private string _url = string.Empty;

        public PPPWebSocket(string url, string leaderboardName)
        {
            _ = StartWebSocket(url, leaderboardName);
        }

        private async Task StartWebSocket(string url, string leaderboardName)
        {
            this._leaderboardName = leaderboardName;
            this._url = url;
            try
            {
                userId = (await Plugin.GetUserInfoBS()).platformUserId;
                webSocket = new WebSocketSharp.WebSocket(url);
                webSocket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                webSocket.OnMessage += WebSocket_OnMessage;
                webSocket.OnError += WebSocket_OnError;
                webSocket.Connect();
            }
            catch (Exception ex)
            {
                Plugin.ErrorPrint($"Error creating Websocket for {leaderboardName}: {ex.Message}");
            }
        }

        private void WebSocket_OnMessage(object sender, MessageEventArgs e)
        {
            try
            {
                var socketData = JsonConvert.DeserializeObject<T>(e.Data);
                var v = socketData.ConvertToPPPWebSocketData(_leaderboardName);
                if (v.userId == userId)
                {
                    OnScoreSet?.Invoke(this, v);
                }
            }
            catch (Exception ex)
            {
                if (e != null && e.Data != null && e.Data == "Connected to the ScoreSaber WSS") return; //Ignore SS connect message
                Plugin.ErrorPrint($"Error in Websocket for {_leaderboardName} OnMessage: {ex.Message}");
            }
        }

        private async void WebSocket_OnError(object sender, ErrorEventArgs e)
        {
            Plugin.ErrorPrint($"Error in Websocket for {_leaderboardName} Retry connecting...");
            await Task.Delay(5000);
            _ = StartWebSocket(_url, _leaderboardName);
        }

        public void StopWebSocket()
        {
            webSocket.OnMessage -= WebSocket_OnMessage;
            webSocket.OnError -= WebSocket_OnError;
            if (_leaderboardName != Leaderboard.BeatLeader.ToString())
            {
                webSocket?.Close(); //Stop beatleader error when tying to disconnect...
            }
            webSocket = null;
        }
    }
}
