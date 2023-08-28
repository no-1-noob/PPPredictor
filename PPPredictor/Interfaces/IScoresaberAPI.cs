using System.Threading.Tasks;
using static PPPredictor.Data.LeaderBoardDataTypes.ScoreSaberDataTypes;

namespace PPPredictor.Interfaces
{
    internal interface IScoresaberAPI
    {
        Task<ScoreSaberPlayerList> GetPlayers(double? page);

        Task<ScoreSaberPlayer> GetPlayer(long playerId);

        Task<ScoreSaberPlayerScoreList> GetPlayerScores(string playerId, int? limit, int? page);
    }
}
