using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using PPPredictor.Data;
using scoresaberapi;
using SongCore.Utilities;
using SongDetailsCache;
using SongDetailsCache.Structs;

namespace PPPredictor.Utilities
{
    public class PPCalculatorScoreSaber : PPCalculator
    {
        internal static readonly float accumulationConstant = 0.965f;
        private readonly HttpClient httpClient = new HttpClient();
        private readonly scoresaberapi.scoresaberapi scoreSaberClient;
        private SongDetails SongDetails { get; }
        public PPCalculatorScoreSaber() : base()
        {
            scoreSaberClient = new scoresaberapi.scoresaberapi(httpClient);
            SongDetails = SongDetails.Init().Result;
        }

        protected override async Task<PPPPlayer> GetPlayerInfo(long userId)
        {
            try
            {
                var playerInfo = scoreSaberClient.BasicAsync(userId);
                var scoreSaberPlayer = await playerInfo;
                PPPPlayer player = new PPPPlayer(scoreSaberPlayer);
                return player;
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPCalculatorScoreSaber GetPlayerInfo Error: {ex.Message}");
                return new PPPPlayer(true);
            }
        }

        protected override async Task<PPPScoreCollection> GetRecentScores(string userId, int pageSize, int page)
        {
            try
            {
                PlayerScoreCollection scoreSaberCollection = await scoreSaberClient.Scores3Async(userId, pageSize, Sort.Recent, page, true);
                return new PPPScoreCollection(scoreSaberCollection);
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPCalculatorScoreSaber GetRecentScores Error: {ex.Message}");
                return new PPPScoreCollection();
            }
        }

        protected override async Task<List<PPPPlayer>> GetPlayers(double fetchIndexPage)
        {
            try
            {
                List<PPPPlayer> lsPlayer = new List<PPPPlayer>();
                PlayerCollection scoreSaberPlayerCollection = await scoreSaberClient.PlayersAsync(null, fetchIndexPage, null, true);
                foreach (var scoreSaberPlayer in scoreSaberPlayerCollection.Players)
                {
                    lsPlayer.Add(new PPPPlayer(scoreSaberPlayer));
                }
                return lsPlayer;
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPCalculatorScoreSaber GetPlayers Error: {ex.Message}");
                return new List<PPPPlayer>();
            }
        }

        public override Task<double> GetStarsForBeatmapAsync(LevelSelectionNavigationController lvlSelectionNavigationCtrl, IDifficultyBeatmap beatmap)
        {
            try
            {
                if (lvlSelectionNavigationCtrl.selectedBeatmapLevel is CustomBeatmapLevel selectedCustomBeatmapLevel)
                {
                    if (SongDetails.songs.FindByHash(Hashing.GetCustomLevelHash(selectedCustomBeatmapLevel), out Song song))
                    {
                        if (song.GetDifficulty(out SongDifficulty songDiff, (MapDifficulty)beatmap.difficulty))
                        {
                            return Task.FromResult((double)songDiff.stars);
                        }
                    }
                    return Task.FromResult(0d);
                }
                return Task.FromResult(0d);
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPCalculatorScoreSaber GetStarsForBeatmapAsync Error: {ex.Message}");
                return Task.FromResult(-1d);
            }
        }

        public override double ApplyModifierMultiplierToStars(double baseStars, GameplayModifiers gameplayModifiers, bool levelFailed)
        {
            return baseStars;
        }
    }
}
