using beatleaderapi;
using PPPredictor.Data;
using SongCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PPPredictor.Utilities
{
    public class PPCalculatorBeatLeader : PPCalculator
    {
        private readonly HttpClient httpClient = new HttpClient();
        private beatleaderapi.beatleaderapi beatLeaderClient;

        private double ppCalcWeight = 42;

        public PPCalculatorBeatLeader() : base() 
        {
            beatLeaderClient = new beatleaderapi.beatleaderapi("https://api.beatleader.xyz/", httpClient);
        }

        protected override async Task<PPPPlayer> getPlayerInfo(long userId)
        {
            var playerInfo = beatLeaderClient.PlayerAsync(userId.ToString(), false);
            var beatLeaderPlayer = await playerInfo;
            return new PPPPlayer(beatLeaderPlayer);
        }

        protected override async Task<List<PPPPlayer>> getPlayers(double fetchIndexPage)
        {
            List<PPPPlayer> lsPlayer = new List<PPPPlayer>();
            PlayerResponseWithStatsResponseWithMetadata scoreSaberPlayerCollection = await beatLeaderClient.PlayersAsync("pp", (int) fetchIndexPage, 50, null, "desc", null, null, null, null, null, null, null, null, null, null);
                 //(null, fetchIndexPage, null, true);
            foreach (var scoreSaberPlayer in scoreSaberPlayerCollection.Data)
            {
                lsPlayer.Add(new PPPPlayer(scoreSaberPlayer));
            }
            return lsPlayer;
        }

        protected override async Task<PPPScoreCollection> getRecentScores(string userId, int pageSize, int page)
        {
            ScoreResponseWithMyScoreResponseWithMetadata scoreSaberCollection = await beatLeaderClient.ScoresAsync(userId, "date", "desc", page, pageSize, null, null, null, null, null);
            return new PPPScoreCollection(scoreSaberCollection);
        }

        public override double CalculatePPatPercentage(double star, double percentage)
        {
            if (star <= 0) return 0;
            var l = 1.0 - (0.03 * ((star - 0.5) - 3) / 11);
            var n = percentage / 100.0;
            var a = 0.96 * l;
            var f = 1.2 - (0.6 * ((star - 0.5) / 14));
            return (star + 0.5) * ppCalcWeight * Math.Pow((Math.Log(l / (l - n)) / (Math.Log(l / (l - a)))), f);
        }

        public override async Task<double> GetStarsForBeatmapAsync(LevelSelectionNavigationController lvlSelectionNavigationCtrl, IDifficultyBeatmap beatmap)
        {
            if (lvlSelectionNavigationCtrl.selectedBeatmapLevel is CustomBeatmapLevel selectedCustomBeatmapLevel)
            {
                Song song = await beatLeaderClient.Hash2Async(Hashing.GetCustomLevelHash(selectedCustomBeatmapLevel));
                if(song != null)
                {
                    DifficultyDescription diff = song.Difficulties.FirstOrDefault(x => x.Value == beatmap.difficultyRank);
                    if(diff != null)
                    {
                        if(diff.Stars != null && diff.Ranked)
                        {
                            return (double) diff.Stars;
                        }
                    }
                }
            }
            return 0;
        }
    }
}
