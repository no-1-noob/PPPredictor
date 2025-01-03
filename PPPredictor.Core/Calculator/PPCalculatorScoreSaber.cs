using PPPredictor.Core.API;
using PPPredictor.Core.DataType;
using PPPredictor.Core.DataType.BeatSaberEncapsulation;
using PPPredictor.Core.DataType.Curve;
using PPPredictor.Core.DataType.MapPool;
using PPPredictor.Core.DataType.Score;
using PPPredictor.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PPPredictor.Core.DataType.Enums;
using static PPPredictor.Core.DataType.LeaderBoard.ScoreSaberDataTypes;

namespace PPPredictor.Core.Calculator
{
    class PPCalculatorScoreSaber<SSAPI> : PPCalculator where SSAPI : IScoresaberAPI, new()
    {
        internal static readonly float accumulationConstant = 0.965f;
        private Func<PPPBeatMapInfo, PPPBeatMapInfo> scoreSaberLookUpFunction;
        private readonly SSAPI scoresaberAPI;

        public PPCalculatorScoreSaber(Dictionary<string, PPPMapPool> dctMapPool, Settings settings, Func<PPPBeatMapInfo, PPPBeatMapInfo> scoreSaberLookUpFunction) : base(dctMapPool, settings, Leaderboard.ScoreSaber)
        {
            this.scoreSaberLookUpFunction = scoreSaberLookUpFunction;
            scoresaberAPI = new SSAPI();
        }

        internal override async Task<PPPPlayer> GetPlayerInfo(long userId, PPPMapPool mapPool)
        {
            try
            {
                ScoreSaberPlayer scoreSaberPlayer = await scoresaberAPI.GetPlayer(userId);
                PPPPlayer player = new PPPPlayer(scoreSaberPlayer);
                return player;
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"PPCalculatorScoreSaber GetPlayerInfo Error: {ex.Message}");
                return new PPPPlayer(true);
            }
        }

        internal override async Task<PPPScoreCollection> GetRecentScores(string userId, int pageSize, int page, PPPMapPool mapPool)
        {
            try
            {
                ScoreSaberPlayerScoreList scoreSaberPlayerScoreList = await scoresaberAPI.GetPlayerScores(userId, pageSize, page);
                return new PPPScoreCollection(scoreSaberPlayerScoreList);
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"PPCalculatorScoreSaber GetRecentScores Error: {ex.Message}");
                return new PPPScoreCollection();
            }
        }

        internal override Task<PPPScoreCollection> GetAllScores(string userId, PPPMapPool mapPool)
        {
            return Task.FromResult(new PPPScoreCollection());
        }

        internal override async Task<List<PPPPlayer>> GetPlayers(double fetchIndexPage, PPPMapPool mapPool)
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
                Logging.ErrorPrint($"PPCalculatorScoreSaber GetPlayers Error: {ex.Message}");
                return new List<PPPPlayer>();
            }
        }

        internal override Task<PPPBeatMapInfo> GetBeatMapInfoAsync(PPPBeatMapInfo beatMapInfo, PPPMapPool mapPool)
        {
            try
            {
                if (!string.IsNullOrEmpty(beatMapInfo.CustomLevelHash))
                {
                    return Task.FromResult(scoreSaberLookUpFunction(beatMapInfo));
                }
                return Task.FromResult(new PPPBeatMapInfo(beatMapInfo, new PPPStarRating(0)));
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"PPCalculatorScoreSaber GetStarsForBeatmapAsync Error: {ex.Message}");
                return Task.FromResult(new PPPBeatMapInfo(beatMapInfo, new PPPStarRating(-1)));
            }
        }

        internal override PPPBeatMapInfo ApplyModifiersToBeatmapInfo(PPPBeatMapInfo beatMapInfo, PPPMapPool mapPool, GameplayModifiers gameplayModifiers, bool levelFailed = false, bool levelPaused = false)
        {
            if (beatMapInfo.OldDotsEnabled)
            {
                beatMapInfo.ModifiedStarRating = new PPPStarRating();
            }
            return beatMapInfo;
        }

        public override string CreateSeachString(string hash, BeatmapKey beatmapKey)
        {
            return $"{hash}_{ParsingUtil.ParseDifficultyNameToInt(beatmapKey.difficulty.ToString())}";
        }
        internal override Task InternalUpdateMapPoolDetails(PPPMapPool mapPool)
        {
            return Task.CompletedTask;
        }

        internal override bool IsScoreSetOnCurrentMapPool(PPPMapPool mapPool, PPPScoreSetData score)
        {
            return true;
        }

        public override Task UpdateAvailableMapPools()
        {
            PPPMapPool mapPool = new PPPMapPool(MapPoolType.Default, $"", PPCalculatorScoreSaber<ScoresaberAPI>.accumulationConstant, 0, CurveParser.ParseToCurve(new CurveInfo(CurveType.ScoreSaber)));
            if (!_dctMapPool.ContainsKey(mapPool.Id)) _dctMapPool.Add(mapPool.Id, mapPool);
            return Task.CompletedTask;
        }

        internal override List<PPPMapPoolShort> GetMapPools()
        {
            return _dctMapPool.Values.OrderBy(x => x.SortIndex).Select(x => (PPPMapPoolShort)x).ToList();
        }
    }
}
