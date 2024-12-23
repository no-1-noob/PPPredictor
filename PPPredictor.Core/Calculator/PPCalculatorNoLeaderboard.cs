using PPPredictor.Core.DataType;
using PPPredictor.Core.DataType.BeatSaberEncapsulation;
using PPPredictor.Core.DataType.Score;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PPPredictor.Core.Calculator
{
    class PPCalculatorNoLeaderboard : PPCalculator
    {
        //Dummy class, for when no Leaderboards are selected in the options. mhh... why even use this mod then

        public override Task<PPPBeatMapInfo> GetBeatMapInfoAsync(PPPBeatMapInfo beatMapInfo, PPPMapPool mapPool)
        {
            return Task.FromResult(beatMapInfo);
        }

        protected override Task<PPPPlayer> GetPlayerInfo(long userId, PPPMapPool mapPool)
        {
            return Task.FromResult(new PPPPlayer());
        }

        protected override Task<List<PPPPlayer>> GetPlayers(double fetchIndexPage, PPPMapPool mapPool)
        {
            return Task.FromResult(new List<PPPPlayer>());
        }

        protected override Task<PPPScoreCollection> GetRecentScores(string userId, int pageSize, int page, PPPMapPool mapPool)
        {
            return Task.FromResult(new PPPScoreCollection());
        }

        protected override Task<PPPScoreCollection> GetAllScores(string userId, string mapPoolId)
        {
            return Task.FromResult(new PPPScoreCollection());
        }

        public override string CreateSeachString(string hash, BeatmapKey beatmapKey)
        {
            return $"{hash}_{beatmapKey.difficulty}";
        }

        public override Task UpdateMapPoolDetails(PPPMapPool mapPool)
        {
            return Task.CompletedTask;
        }

        public override PPPBeatMapInfo ApplyModifiersToBeatmapInfo(PPPBeatMapInfo beatMapInfo, PPPMapPool mapPool, GameplayModifiers gameplayModifiers, bool levelFailed = false, bool levelPaused = false)
        {
            return beatMapInfo;
        }
#warning IsScoreSetOnCurrentMapPool needed?
        //public override bool IsScoreSetOnCurrentMapPool(PPPWebSocketData score) { return true; }
    }
}
