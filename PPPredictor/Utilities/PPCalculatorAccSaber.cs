using PPPredictor.Data;
using PPPredictor.Data.Curve;
using PPPredictor.Interfaces;
using SongCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PPPredictor.Data.LeaderBoardDataTypes.AccSaberDataTypes;

namespace PPPredictor.Utilities
{
    class PPCalculatorAccSaber<ASAPI> : PPCalculator where ASAPI : IAccSaberAPI, new ()
    {
        private readonly ASAPI accsaberapi;
        private readonly string unsetPoolId = "-1";
        private Dictionary<string, List<ShortScore>> dctScores = new Dictionary<string, List<ShortScore>>();
        private Dictionary<string, double> dctScoresSum = new Dictionary<string, double>();

        private string getPlayerScorePPGainLastHash = string.Empty;
        private string getPlayerScorePPGainLastCategory = string.Empty;

        public PPCalculatorAccSaber()
        {
            hasGetAllScoresFunctionality = true;
            hasGetRecentScoresFunctionality = false;
            accsaberapi = new ASAPI();
            UpdateAvailableMapPools();
        }
        public override Task<PPPBeatMapInfo> GetBeatMapInfoAsync(PPPBeatMapInfo beatMapInfo)
        {
            try
            {
                if (beatMapInfo.SelectedCustomBeatmapLevel != null)
                {
                    string songHash = Hashing.GetCustomLevelHash(beatMapInfo.SelectedCustomBeatmapLevel);
                    string searchString = CreateSeachString(songHash, "SOLO" + beatMapInfo.Beatmap.parentDifficultyBeatmapSet.beatmapCharacteristic.serializedName, beatMapInfo.Beatmap.difficultyRank);
                    var cachedInfo = _leaderboardInfo.CurrentMapPool.LsLeaderboadInfo.FirstOrDefault(x => x.Searchstring == searchString);
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
                Plugin.ErrorPrint($"PPCalculatorAccSaber GetBeatMapInfoAsync Error: {ex.Message}");
                return Task.FromResult(new PPPBeatMapInfo(beatMapInfo, new PPPStarRating(-1)));
            }
        }

        protected override async Task<PPPPlayer> GetPlayerInfo(long userId)
        {
            try
            {
                if (_leaderboardInfo.CurrentMapPool.Id == unsetPoolId) return new PPPPlayer(true);
                AccSaberPlayer player = await accsaberapi.GetAccSaberUserByPool(userId, _leaderboardInfo.CurrentMapPool.Id);
                return new PPPPlayer(player);
            }
            catch (Exception ex)
            {
                _leaderboardInfo.CurrentMapPool.IsPlayerFound = false;
                Plugin.ErrorPrint($"PPCalculatorAccSaber GetPlayerInfo Error: {ex.Message}");
                return new PPPPlayer(true);
            }
        }

        protected override async Task<List<PPPPlayer>> GetPlayers(double fetchIndexPage)
        {
            if(!IsPlayerFound()) return new List<PPPPlayer>();
            try
            {
                List<PPPPlayer> lsPlayer = new List<PPPPlayer>();
                if (_leaderboardInfo.CurrentMapPool.Id == unsetPoolId) return lsPlayer;
                List<AccSaberPlayer> lsAccPlayer = await accsaberapi.GetPlayerListForMapPool(fetchIndexPage, _leaderboardInfo.CurrentMapPool.Id);
                foreach (AccSaberPlayer accPlayer in lsAccPlayer)
                {
                    lsPlayer.Add(new PPPPlayer(accPlayer));
                }
                return lsPlayer;
            }
            catch (Exception ex)
            {
                Plugin.ErrorPrint($"PPCalculatorAccSaber GetPlayers Error: {ex.Message}");
                return new List<PPPPlayer>();
            }
        }

        protected override Task<PPPScoreCollection> GetRecentScores(string userId, int pageSize, int page)
        {
            //No recent scores functionality based on map pool available
            return Task.FromResult(new PPPScoreCollection());
        }

        protected override async Task<PPPScoreCollection> GetAllScores(string userId)
        {
            if (!IsPlayerFound()) return new PPPScoreCollection(); 
            try
            {
                if (_leaderboardInfo.CurrentMapPool.Id == unsetPoolId) return new PPPScoreCollection();
                if (_leaderboardInfo.CurrentMapPool.MapPoolType != MapPoolType.Default)
                {

                    List<AccSaberScores> lsAccSaberScores = await accsaberapi.GetAllScoresByPool(userId, _leaderboardInfo.CurrentMapPool.Id);
                    return new PPPScoreCollection(lsAccSaberScores, 0);
                }
                else
                {
                    //Special case for overall leaderboard need the scores seperated
                    List<AccSaberScores> lsAccSaberScores = await accsaberapi.GetAllScores(userId);
                    //Clean old scores for overall function
                    _leaderboardInfo.CurrentMapPool.LsScores = new List<ShortScore>();
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
                Plugin.ErrorPrint($"PPCalculatorAccSaber GetAllScores Error: {ex.Message}");
                return new PPPScoreCollection();
            }
        }

        public override async Task UpdateMapPoolDetails(PPPMapPool mapPool)
        {
            if (!IsPlayerFound()) return;
            try
            {
                if (_leaderboardInfo.CurrentMapPool.Id == unsetPoolId) return;
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
                    mapPool.LsLeaderboadInfo.Add(new ShortScore(CreateSeachString(song.songHash, "SoloStandard", (int)ParsingUtil.ParseDifficultyNameToInt(song.difficulty)), new PPPStarRating(song.complexity), DateTime.Now));
                }
            }
            catch (Exception ex)
            {
                Plugin.ErrorPrint($"PPCalculatorAccSaber UpdateMapPoolDetails Error: {ex.Message}");
            }
        }

        public override PPPBeatMapInfo ApplyModifiersToBeatmapInfo(PPPBeatMapInfo beatMapInfo, GameplayModifiers gameplayModifiers, bool levelFailed = false, bool levelPaused = false)
        {
            beatMapInfo.ModifiedStarRating = new PPPStarRating(beatMapInfo.BaseStarRating.Stars);
            return beatMapInfo;
        }

        public async void UpdateAvailableMapPools()
        {
            try
            {
                List<AccSaberMapPool> mapPool = await accsaberapi.GetAccSaberMapPools();
                //check if this map pool is already in list
                foreach (AccSaberMapPool newMapPool in mapPool)
                {
                    PPPMapPool oldPool = _leaderboardInfo.LsMapPools.Find(x => x.Id == newMapPool.categoryName);
                    if (oldPool != null && DateTime.UtcNow.AddDays(-1) < oldPool.DtUtcLastRefresh)
                    {
                        continue; //Do not get Playlist if it has been updated less than a day ago.
                    }
                    if (oldPool == null)
                    {
                        int sortindex = Array.IndexOf(new object[3] { "standard", "tech", "true" }, newMapPool.categoryName) + 1;
                        oldPool = new PPPMapPool(newMapPool.categoryName, newMapPool.categoryName, MapPoolType.Custom, newMapPool.categoryDisplayName, 0, sortindex, CurveParser.ParseToCurve(new CurveInfo(CurveType.AccSaber)), string.Empty);
                        _leaderboardInfo.LsMapPools.Add(oldPool);
                    }
                }
                if(!_leaderboardInfo.LsMapPools.Exists(x => x.Id == "overall"))
                    _leaderboardInfo.LsMapPools.Add(new PPPMapPool("overall", "overall", MapPoolType.Default, "Overall", 0, 0, CurveParser.ParseToCurve(new CurveInfo(CurveType.AccSaber)), string.Empty));
                _leaderboardInfo.LsMapPools = _leaderboardInfo.LsMapPools.OrderBy(x => x.SortIndex).ToList();
                SendMapPoolRefreshed();
            }
            catch (Exception ex)
            {
                Plugin.ErrorPrint($"PPCalculatorAccSaber UpdateAvailableMapPools Error: {ex.Message}");
            }
        }

        public override bool IsScoreSetOnCurrentMapPool(PPPWebSocketData score) 
        {
            return _leaderboardInfo.CurrentMapPool.LsLeaderboadInfo.Exists(x => x.Searchstring.Contains(score.hash.ToUpper()));
        }

        public override string CreateSeachString(string hash, IDifficultyBeatmap beatmap)
        {
            return $"{hash}_{beatmap.difficultyRank}";
        }

        protected override double CalculateWeightMulitplier(int index, float accumulationConstant)
        {
            double y1 = 0.1;
            double x1 = 15;
            double k = 0.4;
            double x0 = -(Math.Log((1 - y1) / (y1 * Math.Exp(k * x1) - 1)) / k);
            return (1 + Math.Exp(-k * x0)) / (1 + Math.Exp(k * (index - 1 - x0)));
        }

        public override PPGainResult GetPlayerScorePPGain(string mapSearchString, double pp)
        {
            if(_leaderboardInfo.CurrentMapPool.MapPoolType != MapPoolType.Default)
            {
                return GetPlayerScorePPGainInternal(_leaderboardInfo.CurrentMapPool.LsScores, mapSearchString, pp, _leaderboardInfo.CurrentMapPool.CurrentPlayer.Pp);
            }
            else
            {
                var rankedMapInfo = _leaderboardInfo.CurrentMapPool.LsLeaderboadInfo.FirstOrDefault(x => x.Searchstring == mapSearchString);
                if (rankedMapInfo != null)
                {
                    string categoryDisplayName = getPlayerScorePPGainLastCategory;
                    if(getPlayerScorePPGainLastHash != mapSearchString)
                    {
                        foreach (var group in dctScores)
                        {
                            if (group.Value.FindIndex(x => x.Searchstring == mapSearchString) > -1)
                            {
                                categoryDisplayName = group.Key;
                                getPlayerScorePPGainLastCategory = group.Key;
                                getPlayerScorePPGainLastHash = mapSearchString;
                                break;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(categoryDisplayName))
                    {
                        PPGainResult ppGain = GetPlayerScorePPGainInternal(dctScores[categoryDisplayName], mapSearchString, pp, dctScoresSum[categoryDisplayName]);
                        double otherSum = dctScoresSum.Where(kvp => kvp.Key != categoryDisplayName).Sum(kvp => kvp.Value);
                        return new PPGainResult(ppGain.PpTotal + otherSum, ppGain.PpGainWeighted, ppGain.PpGainRaw);
                    }
                }
                return new PPGainResult(_leaderboardInfo.CurrentMapPool.CurrentPlayer.Pp, pp, pp);
            }
        }
    }
}
