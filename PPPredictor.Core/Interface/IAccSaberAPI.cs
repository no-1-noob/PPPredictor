using System.Collections.Generic;
using System.Threading.Tasks;
using static PPPredictor.Core.DataType.LeaderBoard.AccSaberDataTypes;

namespace PPPredictor.Core.Interface
{
    internal interface IAccSaberAPI
    {
        Task<List<AccSaberRankedMap>> GetAllRankedMaps();
        Task<List<AccSaberRankedMap>> GetRankedMaps(string mapPool);
        Task<List<AccSaberMapPool>> GetAccSaberMapPools();
        Task<AccSaberPlayer> GetAccSaberUserByPool(long userId, string poolIdent);
        Task<List<AccSaberPlayer>> GetPlayerListForMapPool(double page, string mapPoolId);
        Task<List<AccSaberScores>> GetAllScores(string userId);
        Task<List<AccSaberScores>> GetAllScoresByPool(string userId, string poolId);
    }
}
