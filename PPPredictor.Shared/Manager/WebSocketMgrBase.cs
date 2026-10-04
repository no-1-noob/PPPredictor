using PPPredictor.Core.DataType;
using PPPredictor.Shared.Data;
using PPPredictor.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace PPPredictor.Shared.Manager
{
    public abstract class WebSocketMgrBase
    {
        internal List<IPPPWebSocket> _lsWebSockets = new List<IPPPWebSocket>();
        private Dictionary<string, Task> dctWaitingRefresh = new Dictionary<string, Task>();
        public event EventHandler<PPPScoreSetData> OnScoreSet;

        internal IWebSocketOverlayServer OverlayServer;

        public WebSocketMgrBase(IWebSocketOverlayServer overlayServer)
        {
            this.OverlayServer = overlayServer;
        }

        internal abstract void CloseScoreWebSockets();
        public abstract void Initialize();
        public abstract void Dispose();
        public abstract void CreateScoreWebSockets();
        internal abstract void RestartOverlayServer();

        internal void PPPWebsocket_OnScoreSet(object sender, PPPScoreSetData data)
        {
            if (Enum.TryParse(data.leaderboardName, out Enums.Leaderboard leaderboard))
            {
                PluginBase.DebugNetworkPrint($"[PPPredictor][ScoreRefresh][WebSocketManagerReceived] refreshId={data.refreshId} messageId={data.messageId} leaderboard={data.leaderboardName} hash={data.hash} context={data.context} subscriberCount={OnScoreSet?.GetInvocationList().Length ?? 0}", leaderboard);
            }
            OnScoreSet?.Invoke(this, data);
            if (Enum.TryParse(data.leaderboardName, out leaderboard))
            {
                PluginBase.DebugNetworkPrint($"[PPPredictor][ScoreRefresh][WebSocketManagerForwarded] refreshId={data.refreshId} messageId={data.messageId} leaderboard={data.leaderboardName}", leaderboard);
            }
            if (PluginBase.ProfileInfo.IsHitBloqEnabled)
            {
                AddDelayedRefresh(Enums.Leaderboard.HitBloq ,data);
            }
        }

        private void AddDelayedRefresh(Enums.Leaderboard leaderboard, PPPScoreSetData data)
        {
            string key = $"{leaderboard}_{data.hash}";
            if (!dctWaitingRefresh.ContainsKey(key))
            {
                dctWaitingRefresh.Add(key, Task.Run(async () => await WaitForRefresh(leaderboard, data)));
            }
        }

        private async Task WaitForRefresh(Enums.Leaderboard leaderboard, PPPScoreSetData data)
        {
            await Task.Delay(5000);
            OnScoreSet?.Invoke(this, data);
            dctWaitingRefresh.Remove($"{leaderboard}_{data.hash}");
        }

        
    }
}
