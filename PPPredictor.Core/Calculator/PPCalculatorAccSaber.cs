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
using static PPPredictor.Core.DataType.LeaderBoard.AccSaberDataTypes;

namespace PPPredictor.Core.Calculator
{
    class PPCalculatorAccSaber<ASAPI> : PPCalculator where ASAPI : IAccSaberAPI, new ()
    {
        private readonly ASAPI accsaberapi;
        private readonly string unsetPoolId = "-1";
        private Dictionary<string, List<ShortScore>> dctScores = new Dictionary<string, List<ShortScore>>();
        private Dictionary<string, double> dctScoresSum = new Dictionary<string, double>();

        public PPCalculatorAccSaber(Dictionary<string, PPPMapPool> dctMapPool, Settings settings) : base(dctMapPool, settings, Leaderboard.AccSaber)
        {
            accsaberapi = new ASAPI();
        }
        internal override Task<PPPBeatMapInfo> GetBeatMapInfoAsync(PPPBeatMapInfo beatMapInfo, PPPMapPool mapPool)
        {
            try
            {
                if (!string.IsNullOrEmpty(beatMapInfo.CustomLevelHash))
                {
                    string searchString = CreateSeachString(beatMapInfo.CustomLevelHash, beatMapInfo.BeatmapKey);
                    var cachedInfo = mapPool.LsLeaderboadInfo.FirstOrDefault(x => x.Searchstring == searchString);
                    if(cachedInfo != null)
                    {
                        return Task.FromResult(new PPPBeatMapInfo(beatMapInfo, new PPPStarRating(cachedInfo.StarRating.Stars)));
                    }
                    return Task.FromResult(new PPPBeatMapInfo(beatMapInfo, new PPPStarRating(0)));
                }
                return Task.FromResult(beatMapInfo);
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"PPCalculatorAccSaber GetBeatMapInfoAsync Error: {ex.Message}");
                return Task.FromResult(new PPPBeatMapInfo(beatMapInfo, new PPPStarRating(-1)));
            }
        }

        internal override async Task<PPPPlayer> GetPlayerInfo(long userId, PPPMapPool mapPool)
        {
            try
            {
                if (mapPool.Id == unsetPoolId) return new PPPPlayer(true);
                AccSaberPlayer player = await accsaberapi.GetAccSaberUserByPool(userId, mapPool.Id);
                return new PPPPlayer(player);
            }
            catch (Exception ex)
            {
                mapPool.IsPlayerFound = false;
                Logging.ErrorPrint($"PPCalculatorAccSaber GetPlayerInfo Error: {ex.Message}");
                return new PPPPlayer(true);
            }
        }

        internal override async Task<List<PPPPlayer>> GetPlayers(double fetchIndexPage, PPPMapPool mapPool)
        {
            if(!IsPlayerFound(mapPool)) return new List<PPPPlayer>();
            try
            {
                List<PPPPlayer> lsPlayer = new List<PPPPlayer>();
                if (mapPool.Id == unsetPoolId) return lsPlayer;
                List<AccSaberPlayer> lsAccPlayer = await accsaberapi.GetPlayerListForMapPool(fetchIndexPage, mapPool.Id);
                foreach (AccSaberPlayer accPlayer in lsAccPlayer)
                {
                    lsPlayer.Add(new PPPPlayer(accPlayer));
                }
                return lsPlayer;
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"PPCalculatorAccSaber GetPlayers Error: {ex.Message}");
                return new List<PPPPlayer>();
            }
        }

        internal override Task<PPPScoreCollection> GetRecentScores(string userId, int pageSize, int page, PPPMapPool mapPool)
        {
            //No recent scores functionality based on map pool available
            return Task.FromResult(new PPPScoreCollection());
        }

        internal override async Task<PPPScoreCollection> GetAllScores(string userId, PPPMapPool mapPool)
        {
            if (!IsPlayerFound(mapPool)) return new PPPScoreCollection(); 
            try
            {
                if (mapPool.Id == unsetPoolId) return new PPPScoreCollection();
                if (mapPool.MapPoolType != MapPoolType.Default)
                {

                    List<AccSaberScores> lsAccSaberScores = await accsaberapi.GetAllScoresByPool(userId, mapPool.Id);
                    return new PPPScoreCollection(lsAccSaberScores, 0);
                }
                else
                {
                    //Special case for overall leaderboard need the scores seperated
                    List<AccSaberScores> lsAccSaberScores = await accsaberapi.GetAllScores(userId);
                    //Clean old scores for overall function
                    mapPool.LsScores = new List<ShortScore>();
                    dctScores.Clear();
                    dctScoresSum.Clear();
                    foreach (var group in lsAccSaberScores.GroupBy(x => x.categoryDisplayName))
                    {
                        //I hate to parse it twice...
                        List<ShortScore> lsShortScore = new List<ShortScore>();
                        foreach (var score in group.OrderByDescending(x => x.weightedAp))
                        {
                            PPPScore pppScore = new PPPScore(score);
                            string searchString = CreateSeachString(pppScore.SongHash, pppScore.GameMode, (int)pppScore.Difficulty1);
                            lsShortScore.Add(new ShortScore(searchString, pppScore.Pp));
                        }
                        dctScores.Add(group.Key, lsShortScore);
                        dctScoresSum.Add(group.Key, group.Sum(x => x.weightedAp));
                    }
                    return new PPPScoreCollection(lsAccSaberScores, 0);
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"PPCalculatorAccSaber GetAllScores Error: {ex.Message}");
                return new PPPScoreCollection();
            }
        }

        internal override async Task InternalUpdateMapPoolDetails(PPPMapPool mapPool)
        {
            if (!IsPlayerFound(mapPool)) return;
            try
            {
                if (mapPool.Id == unsetPoolId) return;
                mapPool.LsMapPoolEntries.Clear();
                List<AccSaberRankedMap> rankedSongs = new List<AccSaberRankedMap>();
                if (mapPool.MapPoolType != MapPoolType.Default)
                {

                    rankedSongs = await this.accsaberapi.GetRankedMaps(mapPool.PlayListId);
                }
                else
                {
                    rankedSongs = await this.accsaberapi.GetAllRankedMaps();
                }
                foreach (AccSaberRankedMap song in rankedSongs)
                {
                    mapPool.LsLeaderboadInfo.Add(new ShortScore(CreateSeachString(song.songHash, "SoloStandard", (int)ParsingUtil.ParseDifficultyNameToInt(song.difficulty)), new PPPStarRating(song.complexity), DateTime.Now, song.categoryDisplayName));
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"PPCalculatorAccSaber UpdateMapPoolDetails Error: {ex.Message}");
            }
        }

        internal override PPPBeatMapInfo ApplyModifiersToBeatmapInfo(PPPBeatMapInfo beatMapInfo, PPPMapPool mapPool, GameplayModifiers gameplayModifiers, bool levelFailed = false, bool levelPaused = false)
        {
            beatMapInfo.ModifiedStarRating = new PPPStarRating(beatMapInfo.BaseStarRating.Stars);
            return beatMapInfo;
        }

        internal override bool IsScoreSetOnCurrentMapPool(PPPMapPool mapPool, PPPScoreSetData score)
        {
            return mapPool.LsLeaderboadInfo.Exists(x => x.Searchstring.Contains(score.hash.ToUpper()));
        }

        protected override double CalculateWeightMulitplier(int index, float accumulationConstant)
        {
            double y1 = 0.1;
            double x1 = 15;
            double k = 0.4;
            double x0 = -(Math.Log((1 - y1) / (y1 * Math.Exp(k * x1) - 1)) / k);
            return (1 + Math.Exp(-k * x0)) / (1 + Math.Exp(k * (index - 1 - x0)));
        }

        internal override PPGainResult GetPlayerScorePPGain(string mapSearchString, double pp, PPPMapPool mapPool)
        {
            if(mapPool.MapPoolType != MapPoolType.Default)
            {
                return GetPlayerScorePPGainInternal(mapPool.LsScores, mapSearchString, pp, mapPool.CurrentPlayer.Pp, mapPool);
            }
            else
            {
                var rankedMapInfo = mapPool.LsLeaderboadInfo.FirstOrDefault(x => x.Searchstring == mapSearchString);
                if (rankedMapInfo != null)
                {
                    PPGainResult ppGain = GetPlayerScorePPGainInternal(dctScores[rankedMapInfo.Category], mapSearchString, pp, dctScoresSum[rankedMapInfo.Category], mapPool);
                    double otherSum = dctScoresSum.Where(kvp => kvp.Key != rankedMapInfo.Category).Sum(kvp => kvp.Value);
                    return new PPGainResult(ppGain.PpTotal + otherSum, ppGain.PpGainWeighted, ppGain.PpGainRaw, _settings.PpGainCalculationType);
                }
                return new PPGainResult(mapPool.CurrentPlayer.Pp, pp, pp, _settings.PpGainCalculationType);
            }
        }

        public override string CreateSeachString(string hash, BeatmapKey beatmapKey)
        {
            return $"{hash}_SOLO{beatmapKey.serializedName}_{ParsingUtil.ParseDifficultyNameToInt(beatmapKey.difficulty.ToString())}".ToUpper();
        }

        public override async Task UpdateAvailableMapPools()
        {
            try
            {
                var defaultMapPool = new PPPMapPool(MapPoolType.Default, $"☞ Select a map pool ☜", 0, 0, CurveParser.ParseToCurve(new CurveInfo(CurveType.AccSaber)), 0);
                if (!_dctMapPool.ContainsKey(defaultMapPool.Id)) _dctMapPool.Add(defaultMapPool.Id, defaultMapPool);

                List<AccSaberMapPool> mapPool = await accsaberapi.GetAccSaberMapPools();
                //check if this map pool is already in list
                foreach (AccSaberMapPool newMapPool in mapPool)
                {
                    if (_dctMapPool.TryGetValue(newMapPool.categoryName, out PPPMapPool oldPool))
                    {
                        if (DateTime.UtcNow.AddDays(-1) < oldPool.DtUtcLastRefresh)
                        {
                            continue; //Do not get Playlist if it has been updated less than a day ago.
                        }
                    }
                    else
                    {
                        int sortindex = Array.IndexOf(new object[3] { "standard", "tech", "true" }, newMapPool.categoryName) + 1;
                        oldPool = new PPPMapPool(newMapPool.categoryName, newMapPool.categoryName, MapPoolType.Custom, newMapPool.categoryDisplayName, 0, sortindex, CurveParser.ParseToCurve(new CurveInfo(CurveType.AccSaber)), string.Empty);
                        if (!_dctMapPool.ContainsKey(oldPool.Id)) _dctMapPool.Add(oldPool.Id, oldPool);
                    }
                }
                if(!_dctMapPool.ContainsKey("overall"))
                {
                    var overallMapPool = new PPPMapPool("overall", "overall", MapPoolType.Default, "Overall", 0, 0, CurveParser.ParseToCurve(new CurveInfo(CurveType.AccSaber)), string.Empty);
                    if (!_dctMapPool.ContainsKey(overallMapPool.Id)) _dctMapPool.Add(overallMapPool.Id, overallMapPool);
                }
                SendMapPoolRefreshed();
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"PPCalculatorAccSaber UpdateAvailableMapPools Error: {ex.Message}");
            }
        }

        internal override List<PPPMapPoolShort> GetMapPools()
        {
            return _dctMapPool.Values.OrderBy(x => x.SortIndex).Select(x => (PPPMapPoolShort)x).ToList();
        }
    }
}
