using Newtonsoft.Json;
using PPPredictor.Core.DataType;
using PPPredictor.Shared;
using PPPredictor.Shared.Interfaces;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using WebSocketSharp;
using static PPPredictor.Core.DataType.Enums;
using WebSocketState = WebSocketSharp.WebSocketState;

namespace PPPredictor.WebSocket
{
    internal class PPPWebSocket<T> : PPPredictor.Shared.Interfaces.IPPPWebSocket where T : IPPPRawWebsocketData
    {
        private WebSocketSharp.WebSocket webSocket;
        public event EventHandler<PPPScoreSetData> OnScoreSet;
        private const int KeepAliveIntervalMs = 45000;
        private const int ReconnectDelayMs = 5000;
        private readonly object _webSocketLock = new object();
        private System.Threading.Timer _keepAliveTimer;
        private string userId = string.Empty;
        private string _leaderboardName = string.Empty;
        private string _url = string.Empty;
        private bool _isStopping = false;
        private bool _isReconnectScheduled = false;

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
                userId = await Plugin.Instance.GetPlatformUserId();
                CleanupWebSocket(true);

                WebSocketSharp.WebSocket socket;
                lock (_webSocketLock)
                {
                    if (_isStopping) return;
                    _isReconnectScheduled = false;
                    socket = new WebSocketSharp.WebSocket(url);
                    socket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                    socket.OnMessage += WebSocket_OnMessage;
                    socket.OnError += WebSocket_OnError;
                    socket.OnClose += WebSocket_OnClose;
                    webSocket = socket;
                }

                socket.ConnectAsync();
                StartKeepAlive();
            }
            catch (Exception ex)
            {
                Plugin.ErrorPrint($"Error creating Websocket for {leaderboardName}: {ex.Message}");
                ScheduleReconnect();
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

        private void WebSocket_OnError(object sender, ErrorEventArgs e)
        {
            Plugin.ErrorPrint($"Error in Websocket for {_leaderboardName}: {e.Message}");
            ScheduleReconnect();
        }

        private void WebSocket_OnClose(object sender, CloseEventArgs e)
        {
            if (_isStopping) return;
            Plugin.ErrorPrint($"Websocket for {_leaderboardName} closed: {e.Code} {e.Reason}");
            ScheduleReconnect();
        }

        private void StartKeepAlive()
        {
            _keepAliveTimer?.Dispose();
            _keepAliveTimer = new System.Threading.Timer(_ => SendKeepAlive(), null, KeepAliveIntervalMs, KeepAliveIntervalMs);
        }

        private void SendKeepAlive()
        {
            WebSocketSharp.WebSocket socket;
            lock (_webSocketLock)
            {
                socket = webSocket;
            }

            if (socket == null || socket.ReadyState != WebSocketState.Open) return;

            try
            {
                if (!socket.Ping())
                {
                    Plugin.ErrorPrint($"Websocket keepalive ping failed for {_leaderboardName}");
                    ScheduleReconnect();
                }
            }
            catch (Exception ex)
            {
                Plugin.ErrorPrint($"Websocket keepalive ping error for {_leaderboardName}: {ex.Message}");
                ScheduleReconnect();
            }
        }

        private async void ScheduleReconnect()
        {
            lock (_webSocketLock)
            {
                if (_isStopping || _isReconnectScheduled) return;
                _isReconnectScheduled = true;
            }

            await Task.Delay(ReconnectDelayMs);
            if (_isStopping) return;
            _ = StartWebSocket(_url, _leaderboardName);
        }

        private void CleanupWebSocket(bool closeSocket)
        {
            _keepAliveTimer?.Dispose();
            _keepAliveTimer = null;

            WebSocketSharp.WebSocket socket;
            lock (_webSocketLock)
            {
                socket = webSocket;
                webSocket = null;
            }

            if (socket == null) return;

            socket.OnMessage -= WebSocket_OnMessage;
            socket.OnError -= WebSocket_OnError;
            socket.OnClose -= WebSocket_OnClose;

            if (closeSocket && _leaderboardName != Leaderboard.BeatLeader.ToString())
            {
                socket.Close();
            }
        }

        public void StopWebSocket()
        {
            _isStopping = true;
            CleanupWebSocket(true);
        }
    }
}
