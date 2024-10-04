using PPPredictor.Data;
using System;

namespace PPPredictor.Interfaces
{
    internal interface IPPPWebSocket
    {
        event EventHandler<PPPWebSocketData> OnScoreSet;
        void StopWebSocket();
    }
}
