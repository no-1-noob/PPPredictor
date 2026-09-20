using PPPredictor.Core.DataType;

namespace PPPredictor.Shared.Interfaces
{
    internal interface IPPPRawWebsocketData
    {
        PPPScoreSetData ConvertToPPPWebSocketData(string leaderboardName);
    }
}
