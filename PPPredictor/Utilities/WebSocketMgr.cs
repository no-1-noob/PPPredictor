using PPPredictor.Data;
using PPPredictor.Interfaces;
using PPPredictor.OpenAPIs;
using PPPredictor.OverlayServer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zenject;

namespace PPPredictor.Utilities
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
            OverlayServer = new WebSocketOverlayServer();
            OverlayServer.StartSocket();
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

        private void PPPWebsocket_OnScoreSet(object sender, PPPWebSocketData data)
        {
            _ppPredictorMgr.ScoreSet(data.leaderboardName, data);
            if (Plugin.ProfileInfo.IsHitBloqEnabled)
            {
                if (!dctWaitingRefresh.ContainsKey(data.hash))
                {
                    dctWaitingRefresh.Add(data.hash, Task.Run(async () => await WaitHitloqRefresh(data)));
                }
            }
        }

        private async Task WaitHitloqRefresh(PPPWebSocketData data)
        {
            await Task.Delay(5000);
            _ppPredictorMgr.ScoreSet(Leaderboard.HitBloq.ToString(), data);
            dctWaitingRefresh.Remove(data.hash);
        }

        #region init dispose
        public void Dispose()
        {
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
