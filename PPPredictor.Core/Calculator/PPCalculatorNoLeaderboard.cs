using PPPredictor.Core.DataType;
using PPPredictor.Core.DataType.BeatSaberEncapsulation;
using PPPredictor.Core.DataType.Curve;
using PPPredictor.Core.DataType.MapPool;
using PPPredictor.Core.DataType.Score;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PPPredictor.Core.DataType.Enums;

namespace PPPredictor.Core.Calculator
{
    class PPCalculatorNoLeaderboard : PPCalculator
    {
        public PPCalculatorNoLeaderboard(Dictionary<string, PPPMapPool> dctMapPool, Settings settings) : base(dctMapPool, settings, Leaderboard.NoLeaderboard)
        {
        }

        //Dummy class, for when no Leaderboards are selected in the options. mhh... why even use this mod then

        internal override Task<PPPBeatMapInfo> GetBeatMapInfoAsync(PPPBeatMapInfo beatMapInfo, PPPMapPool mapPool)
        {
            return Task.FromResult(beatMapInfo);
        }

        internal override Task<PPPPlayer> GetPlayerInfo(long userId, PPPMapPool mapPool)
        {
            return Task.FromResult(new PPPPlayer());
        }

        internal override Task<List<PPPPlayer>> GetPlayers(double fetchIndexPage, PPPMapPool mapPool)
        {
            return Task.FromResult(new List<PPPPlayer>());
        }

        internal override Task<PPPScoreCollection> GetRecentScores(string userId, int pageSize, int page, PPPMapPool mapPool)
        {
            return Task.FromResult(new PPPScoreCollection());
        }

        internal override Task<PPPScoreCollection> GetAllScores(string userId, PPPMapPool mapPool)
        {
            return Task.FromResult(new PPPScoreCollection());
        }

        public override string CreateSeachString(string hash, BeatmapKey beatmapKey)
        {
            return $"{hash}_{beatmapKey.difficulty}";
        }

        internal override Task InternalUpdateMapPoolDetails(PPPMapPool mapPool)
        {
            return Task.CompletedTask;
        }

        internal override PPPBeatMapInfo ApplyModifiersToBeatmapInfo(PPPBeatMapInfo beatMapInfo, PPPMapPool mapPool, GameplayModifiers gameplayModifiers, bool levelFailed = false, bool levelPaused = false)
        {
            return beatMapInfo;
        }

        public override Task UpdateAvailableMapPools()
        {
            var mapPool = new PPPMapPool(MapPoolType.Default, $"", 0, 0, new CustomPPPCurve(new List<(double, double)>(), CurveType.Linear, 0));
            if (!_dctMapPool.ContainsKey(mapPool.Id)) _dctMapPool.Add(mapPool.Id, mapPool);
            return Task.CompletedTask;
        }

        internal override List<PPPMapPoolShort> GetMapPools()
        {
            return _dctMapPool.Values.OrderBy(x => x.SortIndex).Select(x => (PPPMapPoolShort)x).ToList();
        }

        internal override bool IsScoreSetOnCurrentMapPool(PPPMapPool mapPool, PPPScoreSetData score) { return true; }
    }
}
