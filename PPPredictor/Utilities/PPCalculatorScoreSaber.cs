using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PPPredictor.Data;
using PPPredictor.OpenAPIs;
using SongCore.Utilities;
using SongDetailsCache;
using SongDetailsCache.Structs;
using static PPPredictor.OpenAPIs.ScoresaberAPI;

namespace PPPredictor.Utilities
{
    class PPCalculatorScoreSaber : PPCalculator
    {
        internal static readonly float accumulationConstant = 0.965f;
        private readonly ScoresaberAPI scoresaberAPI;

        private SongDetails SongDetails { get; }
        public PPCalculatorScoreSaber() : base()
        {
            playerPerPages = 50;
            scoresaberAPI = new ScoresaberAPI();
            SongDetails = SongDetails.Init().Result;
        }

        protected override async Task<PPPPlayer> GetPlayerInfo(long userId)
        {
            try
            {
                ScoreSaberPlayer scoreSaberPlayer = await scoresaberAPI.GetPlayer(userId);
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
                ScoreSaberPlayerScoreList scoreSaberPlayerScoreList = await scoresaberAPI.GetPlayerScores(userId, pageSize, page);
                return new PPPScoreCollection(scoreSaberPlayerScoreList);
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
                ScoreSaberPlayerList scoreSaberPlayerCollection = await scoresaberAPI.GetPlayers(fetchIndexPage);
                foreach (var scoreSaberPlayer in scoreSaberPlayerCollection.players)
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

        public override Task<PPPBeatMapInfo> GetBeatMapInfoAsync(PPPBeatMapInfo beatMapInfo)
        {
            try
            {
                if (beatMapInfo.SelectedCustomBeatmapLevel != null)
                {
                    if (SongDetails.songs.FindByHash(Hashing.GetCustomLevelHash(beatMapInfo.SelectedCustomBeatmapLevel), out Song song))
                    {
                        if (song.GetDifficulty(out SongDifficulty songDiff, (MapDifficulty)beatMapInfo.Beatmap.difficulty))
                        {
                            return Task.FromResult(new PPPBeatMapInfo(beatMapInfo, songDiff.stars, 0));
                        }
                    }
                    return Task.FromResult(new PPPBeatMapInfo (beatMapInfo, 0));
                }
                return Task.FromResult(new PPPBeatMapInfo(beatMapInfo, 0));
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPCalculatorScoreSaber GetStarsForBeatmapAsync Error: {ex.Message}");
                return Task.FromResult(new PPPBeatMapInfo(beatMapInfo , - 1, -1));
            }
        }

        public override double ApplyModifierMultiplierToStars(PPPBeatMapInfo beatMapInfo, GameplayModifiers gameplayModifiers, bool levelFailed)
        {
            return beatMapInfo.BaseStars;
        }

        public override string CreateSeachString(string hash, IDifficultyBeatmap beatmap)
        {
            return $"{hash}_{beatmap.difficultyRank}";
        }
        override public Task UpdateMapPoolDetails(PPPMapPool mapPool)
        {
            return Task.CompletedTask;
        }
    }
}
