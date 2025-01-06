using PPPredictor.Core.DataType;
using PPPredictor.Data;
using PPPredictor.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zenject;
using static PPPredictor.Core.DataType.Enums;

namespace PPPredictor.WebSocket
{
    internal class WebSocketMgr : IInitializable, IDisposable
    {
        private readonly IPPPredictorMgr _ppPredictorMgr;
        private List<IPPPWebSocket> _lsWebSockets = new List<IPPPWebSocket>();
        private Dictionary<string, Task> dctWaitingRefresh = new Dictionary<string, Task>();

        internal WebSocketOverlayServer OverlayServer;

        public WebSocketMgr(IPPPredictorMgr ppPredictorMgr)
        {
            this._ppPredictorMgr = ppPredictorMgr;
            RestartOverlayServer();
        }

        public void CreateScoreWebSockets()
        {
            if (Plugin.ProfileInfo.IsScoreSaberEnabled)
            {
                PPPWebSocket<PPPWsScoreSaberCommand> socket = new PPPWebSocket<PPPWsScoreSaberCommand>("wss://scoresaber.com/ws", Leaderboard.ScoreSaber.ToString());
                socket.OnScoreSet += PPPWebsocket_OnScoreSet;
                _lsWebSockets.Add(socket);
            }
            if (Plugin.ProfileInfo.IsBeatLeaderEnabled)
            {
                PPPWebSocket<PPPWsBeatLeaderData> socket = new PPPWebSocket<PPPWsBeatLeaderData>("wss://sockets.api.beatleader.xyz/scores", Leaderboard.BeatLeader.ToString());
                socket.OnScoreSet += PPPWebsocket_OnScoreSet;
                _lsWebSockets.Add(socket);
            }
        }

        private void PPPWebsocket_OnScoreSet(object sender, PPPScoreSetData data)
        {
            _ppPredictorMgr.ScoreSet(data.leaderboardName, data);
            if (Plugin.ProfileInfo.IsHitBloqEnabled)
            {
                AddDelayedRefresh(Leaderboard.HitBloq ,data);
            }
        }

        private void AddDelayedRefresh(Leaderboard leaderboard, PPPScoreSetData data)
        {
            string key = $"{leaderboard}_{data.hash}";
            if (!dctWaitingRefresh.ContainsKey(key))
            {
                dctWaitingRefresh.Add(key, Task.Run(async () => await WaitForRefresh(leaderboard, data)));
            }
        }

        private async Task WaitForRefresh(Leaderboard leaderboard, PPPScoreSetData data)
        {
            await Task.Delay(5000);
            _ppPredictorMgr.ScoreSet(leaderboard.ToString(), data);
            dctWaitingRefresh.Remove(data.hash);
        }

        internal void RestartOverlayServer()
        {
            OverlayServer?.CloseSocket();
            OverlayServer = new WebSocketOverlayServer();
            OverlayServer.StartSocket();
        }

        #region init dispose
        public void Dispose()
        {
            OverlayServer.CloseSocket();
            foreach (var socket in _lsWebSockets)
            {
                socket.StopWebSocket();
                socket.OnScoreSet -= PPPWebsocket_OnScoreSet;
            }
        }

        public void Initialize()
        {
            //throw new NotImplementedException();
        }
        #endregion
    }
}
