using System.Collections.Generic;
using System.Threading.Tasks;
using static PPPredictor.Core.DataType.LeaderBoard.HitBloqDataTypes;

namespace PPPredictor.Core.Interface
{
    internal interface IHitBloqAPI
    {
        Task<HitBloqUserId> GetHitBloqUserIdByUserId(string id);

        Task<List<HitBloqMapPool>> GetHitBloqMapPools();

        Task<HitBloqMapPoolDetails> GetHitBloqMapPoolDetails(string poolIdent, int page);

        Task<HitBloqUser> GetHitBloqUserByPool(long userId, string poolIdent);

        Task<List<HitBloqScores>> GetRecentScores(string userId, string poolId, int page);

        Task<List<HitBloqScores>> GetAllScores(string userId, string poolId);

        Task<HitBloqLadder> GetPlayerListForMapPool(double page, string mapPoolId);

        Task<HitBloqRankFromCr> GetPlayerRankByCr(string mapPoolId, double cr);

        Task<HitBloqLeaderboardInfo> GetLeaderBoardInfo(string searchString);
    }
}
