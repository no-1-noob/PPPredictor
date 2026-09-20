using PPPredictor.Core.DataType;
using PPPredictor.Shared.Data;
using PPPredictor.Shared.Interfaces;
using PPPredictor.Shared.Manager;
using PPPredictor.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zenject;
using static PPPredictor.Core.DataType.Enums;

namespace PPPredictor.Manager
{
    internal class WebSocketMgr : WebSocketMgrBase, IInitializable, IDisposable
    {
        public WebSocketMgr() : base(new WebSocketOverlayServer())
        {
            this.RestartOverlayServer();
        }
        
        #region init dispose
        public override void Dispose()
        {
            OverlayServer.CloseSocket();
            CloseScoreWebSockets();
        }
        
        public override async void CreateScoreWebSockets()
        {
            if (Plugin.ProfileInfo.IsScoreSaberEnabled)
            {
                PPPWebSocket<PPPWsScoreSaberCommand> socket = new PPPWebSocket<PPPWsScoreSaberCommand>("wss://scoresaber.com/ws", Enums.Leaderboard.ScoreSaber.ToString());
                socket.OnScoreSet += PPPWebsocket_OnScoreSet;
                _lsWebSockets.Add(socket);
            }
            if (Plugin.ProfileInfo.IsBeatLeaderEnabled)
            {
                PPPWebSocket<PPPWsBeatLeaderData> socket = new PPPWebSocket<PPPWsBeatLeaderData>("wss://sockets.api.beatleader.com/scores", Enums.Leaderboard.BeatLeader.ToString());
                socket.OnScoreSet += PPPWebsocket_OnScoreSet;
                _lsWebSockets.Add(socket);
            }
            if (Plugin.ProfileInfo.IsAccSaberReloadedEnabled)
            {
                PPPWebSocket<PPPWsAccSaberReloadedData> socket = new PPPWebSocket<PPPWsAccSaberReloadedData>("wss://accsaberreloaded.com/ws/scores", Enums.Leaderboard.AccSaberReloaded.ToString());
                socket.OnScoreSet += PPPWebsocket_OnScoreSet;
                _lsWebSockets.Add(socket);
            }
        }

        internal override void CloseScoreWebSockets()
        {
            foreach (var socket in _lsWebSockets)
            {
                socket.StopWebSocket();
                socket.OnScoreSet -= PPPWebsocket_OnScoreSet;
            }
            _lsWebSockets.Clear();
        }
        
        internal override sealed void RestartOverlayServer()
        {
            OverlayServer?.CloseSocket();
            OverlayServer = new WebSocketOverlayServer();
            OverlayServer.StartSocket();
        }

        public override void Initialize()
        {
            //throw new NotImplementedException();
        }
        #endregion
    }
}
