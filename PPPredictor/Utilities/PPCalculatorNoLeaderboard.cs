using PPPredictor.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PPPredictor.Utilities
{
    class PPCalculatorNoLeaderboard : PPCalculator
    {
        //Dummy class, for when no Leaderboards are selected in the options. mhh... why even use this mod then
        public override double ApplyModifierMultiplierToStars(PPPBeatMapInfo beatMapInfo, GameplayModifiers gameplayModifiers, bool levelFailed)
        {
            return 0;
        }

        public override double CalculatePPatPercentage(double star, double percentage, bool levelFailed)
        {
            return 0;
        }

        public override Task<PPPBeatMapInfo> GetBeatMapInfoAsync(LevelSelectionNavigationController lvlSelectionNavigationCtrl, IDifficultyBeatmap beatmap)
        {
            return Task.FromResult(new PPPBeatMapInfo());
        }

        protected override Task<PPPPlayer> GetPlayerInfo(long userId)
        {
            return Task.FromResult(new PPPPlayer());
        }

        protected override Task<List<PPPPlayer>> GetPlayers(double fetchIndexPage)
        {
            return Task.FromResult(new List<PPPPlayer>());
        }

        protected override Task<PPPScoreCollection> GetRecentScores(string userId, int pageSize, int page)
        {
            return Task.FromResult(new PPPScoreCollection());
        }
    }
}
