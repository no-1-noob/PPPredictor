using PPPredictor.Data;

namespace PPPredictor.Interfaces
{
    internal interface IPPPRawWebsocketData
    {
        PPPWebSocketData ConvertToPPPWebSocketData(string leaderboardName);
    }
}
