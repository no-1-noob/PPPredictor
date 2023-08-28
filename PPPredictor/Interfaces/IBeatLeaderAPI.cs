using System.Collections.Generic;
using System.Threading.Tasks;
using static PPPredictor.Data.LeaderBoardDataTypes.BeatLeaderDataTypes;

namespace PPPredictor.Interfaces
{
    internal interface IBeatLeaderAPI
    {
        Task<Dictionary<string, float>> GetModifiers();

        Task<BeatLeaderEventList> GetEvents();

        Task<BeatLeaderSong> GetSongByHash(string hash);

        Task<BeatLeaderPlayer> GetPlayer(long userId);

        Task<BeatLeaderPlayerScoreList> GetPlayerScores(string userId, string sortBy, string order, int page, int count, long? eventId = null);

        Task<BeatLeaderPlayerList> GetPlayersInLeaderboard(string sortBy, int page, int? count, string order);

        Task<BeatLeaderPlayerList> GetPlayersInEventLeaderboard(long eventId, string sortBy, int page, int? count, string order);

        Task<BeatLeaderPlayList> GetPlayList(long playListId);
    }
}
