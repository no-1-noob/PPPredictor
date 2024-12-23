using System.Threading.Tasks;
using static PPPredictor.Core.DataType.LeaderBoard.ScoreSaberDataTypes;

namespace PPPredictor.Core.Interface
{
    internal interface IScoresaberAPI
    {
        Task<ScoreSaberPlayerList> GetPlayers(double? page);

        Task<ScoreSaberPlayer> GetPlayer(long playerId);

        Task<ScoreSaberPlayerScoreList> GetPlayerScores(string playerId, int? limit, int? page);
    }
}
