using System.Threading.Tasks;
using static PPPredictor.Core.DataType.LeaderBoard.BeatLeaderDataTypes;

namespace PPPredictor.Core.Interface
{
    internal interface IBeatLeaderAPI
    {
        Task<BeatLeaderEventList> GetEvents();

        Task<BeatLeaderSong> GetSongByHash(string hash);

        Task<BeatLeaderPlayer> GetPlayer(long userId, long leaderboardContextId);

        Task<BeatLeaderPlayerScoreList> GetPlayerScores(string userId, string sortBy, string order, int page, int count, long leaderboardContextId, long? eventId = null);

        Task<BeatLeaderPlayerList> GetPlayersInLeaderboard(string sortBy, int page, int? count, string order, long leaderboardContextId);

        Task<BeatLeaderPlayerList> GetPlayersInEventLeaderboard(long eventId, string sortBy, int page, int? count, string order);

        Task<BeatLeaderPlayList> GetPlayList(long playListId);
    }
}
