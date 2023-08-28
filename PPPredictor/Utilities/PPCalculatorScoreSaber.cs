using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PPPredictor.Data;
using PPPredictor.Interfaces;
using SongCore.Utilities;
using SongDetailsCache;
using SongDetailsCache.Structs;
using static PPPredictor.Data.LeaderBoardDataTypes.ScoreSaberDataTypes;

namespace PPPredictor.Utilities
{
    class PPCalculatorScoreSaber<SSAPI> : PPCalculator where SSAPI : IScoresaberAPI, new()
    {
        internal static readonly float accumulationConstant = 0.965f;
        private readonly SSAPI scoresaberAPI;

        private SongDetails SongDetails { get; }
        public PPCalculatorScoreSaber() : base()
        {
            playerPerPages = 50;
            scoresaberAPI = new SSAPI();
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

        protected override Task<PPPScoreCollection> GetAllScores(string userId)
        {
            return Task.FromResult(new PPPScoreCollection());
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
                            return Task.FromResult(new PPPBeatMapInfo(beatMapInfo, new PPPStarRating(songDiff.stars)));
                        }
                    }
                    return Task.FromResult(new PPPBeatMapInfo (beatMapInfo, new PPPStarRating(0)));
                }
                return Task.FromResult(new PPPBeatMapInfo(beatMapInfo, new PPPStarRating(0)));
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPCalculatorScoreSaber GetStarsForBeatmapAsync Error: {ex.Message}");
                return Task.FromResult(new PPPBeatMapInfo(beatMapInfo , new PPPStarRating(-1)));
            }
        }

        public override PPPBeatMapInfo ApplyModifiersToBeatmapInfo(PPPBeatMapInfo beatMapInfo, GameplayModifiers gameplayModifiers, bool levelFailed)
        {
            return beatMapInfo;
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
