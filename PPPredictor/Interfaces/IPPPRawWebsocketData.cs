using PPPredictor.Core.DataType;

namespace PPPredictor.Interfaces
{
    internal interface IPPPRawWebsocketData
    {
        PPPScoreSetData ConvertToPPPWebSocketData(string leaderboardName);
    }
}
