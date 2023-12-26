using PPPredictor.Data;
using PPPredictor.Data.DisplayInfos;
using System;

namespace PPPredictor.Interfaces
{
    internal interface IPPPWebSocket
    {
        event EventHandler<PPPWebSocketData> OnScoreSet;
        void StopWebSocket();
    }
}
