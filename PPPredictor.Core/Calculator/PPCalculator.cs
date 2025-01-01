using PPPredictor.Core.DataType;
using PPPredictor.Core.DataType.BeatSaberEncapsulation;
using PPPredictor.Core.DataType.LeaderBoard;
using PPPredictor.Core.DataType.MapPool;
using PPPredictor.Core.DataType.Score;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static PPPredictor.Core.DataType.Enums;

namespace PPPredictor.Core.Calculator
{
    public abstract class PPCalculator
    {
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        internal PPPLeaderboardInfo _leaderboardInfo;
        protected Settings _settings;
        internal Dictionary<string, PPPMapPool> _dctMapPool = new Dictionary<string, PPPMapPool>();

        public SemaphoreSlim Semaphore => semaphore;

        public Settings Settings { set => _settings = value; }

        public event EventHandler OnMapPoolRefreshed;

        public PPCalculator(Dictionary<string, PPPMapPool> dctMapPool, Settings settings, Leaderboard leaderboard)
        {
            if(dctMapPool != null)
            {
                _dctMapPool = dctMapPool;
            }
            _leaderboardInfo = new PPPLeaderboardInfo(leaderboard);
            _settings = settings;
        }

        internal PPPMapPool GetMapPoolById(string mapPoolId)
        {
            if(_dctMapPool.TryGetValue(mapPoolId, out var mapPool)){
                return mapPool;
            }
            else
            {
                throw new Exception($"MapPool {mapPoolId} for leaderboard {_leaderboardInfo.LeaderboardName} not found");
            }
        }

        internal async Task<(PPPPlayer, PPPPlayer)> UpdatePlayer(PPPMapPool mapPool, bool doResetSession)
        {
            PPPPlayer player = await GetProfile(mapPool);
            mapPool.CurrentPlayer = player;
            if (doResetSession || mapPool.SessionPlayer == null || NeedsResetSession())
            {
                _settings.LastSessionReset = DateTime.Now;
                mapPool.SessionPlayer = player;
            }
            return (mapPool.SessionPlayer, mapPool.CurrentPlayer);
        }

        private bool NeedsResetSession()
        {
            return
                _settings.ResetSessionHours > 0
                && ((DateTime.Now - _settings.LastSessionReset).TotalHours > _settings.ResetSessionHours
                || (DateTime.Now - _settings.LastSessionReset).TotalMinutes < 1); //Parallel reset of multiple scoreboards
        }

        internal async Task<PPPPlayer> GetProfile(PPPMapPool mapPool)
        {
            if (long.TryParse(GetUserId(mapPool), out long longUserId))
            {
                var player = await GetPlayerInfo(longUserId, mapPool); 
                return player;
            }
            return new PPPPlayer();
        }

        private string GetUserId(PPPMapPool mapPool)
        {
            if (!string.IsNullOrEmpty(mapPool.CustomLeaderboardUserId))
            {
                return mapPool.CustomLeaderboardUserId;
            }
            else
            {
                return _settings.UserId;
            }
        }

        internal async Task GetPlayerScores(PPPMapPool mapPool, int pageSize, int largePageSize, bool fetchOnePage = false)
        {
            try
            {
                string userId = GetUserId(mapPool);
                bool hasNoScores = false;
                bool hasMoreData = true;
                int page = 1;
                List<ShortScore> lsNewScores = new List<ShortScore>();
                DateTimeOffset dtNewLastScoreSet = new DateTime(2000, 1, 1);
                while (hasMoreData)
                {
                    if (mapPool.LsScores == null) mapPool.LsScores = new List<ShortScore>();
                    PPPScoreCollection playerscores = null;
                    hasNoScores = mapPool.LsScores.Count == 0;
                    if (_leaderboardInfo.HasGetAllScoresFunctionality && (!_leaderboardInfo.HasGetRecentScoresFunctionality || hasNoScores))
                    {
                        playerscores = await GetAllScores(userId, mapPool);
                        hasMoreData = false;
                    }
                    else {
                        playerscores = await GetRecentScores(userId, hasNoScores ? largePageSize: pageSize, page, mapPool);
                        if (playerscores.Page * playerscores.ItemsPerPage >= playerscores.Total || fetchOnePage)
                        {
                            hasMoreData = false;
                        }
                    }
                    foreach (PPPScore scores in playerscores.LsPPPScore)
                    {
                        string searchString = CreateSeachString(scores.SongHash, scores.GameMode, (int)scores.Difficulty1);
                        ShortScore previousScore = mapPool.LsScores.Find(x => x.Searchstring == searchString);
                        ShortScore newScore = new ShortScore(searchString, scores.Pp);
                        if(newScore.Pp > 0) // Do not cache unranked scores
                        {
                            if (previousScore == null)
                            {
                                lsNewScores.Add(newScore);
                            }
                            else
                            {
                                if (mapPool.DtLastScoreSet >= scores.TimeSet)
                                {
                                    hasMoreData = false;
                                }
                                else
                                {
                                    lsNewScores.Add(newScore);
                                }
                            }
                        }
                        dtNewLastScoreSet = scores.TimeSet > dtNewLastScoreSet ? scores.TimeSet : dtNewLastScoreSet;
                    }
                    page++;
                    await Task.Delay(_leaderboardInfo.TaskDelayValue);
                }
                //Update after fetching all data. So when closing while fetching the incomplete data is not saved.
                foreach (ShortScore newScore in lsNewScores)
                {
                    ShortScore previousScore = mapPool.LsScores.Find(x => x.Searchstring == newScore.Searchstring);
                    if (previousScore == null)
                    {
                        mapPool.LsScores.Add(newScore);
                    }
                    else
                    {
                        previousScore.Pp = newScore.Pp;
                    }
                };
                mapPool.LsScores = mapPool.LsScores; //Trigger sorting in setter
                mapPool.DtLastScoreSet = dtNewLastScoreSet > mapPool.DtLastScoreSet ? dtNewLastScoreSet : mapPool.DtLastScoreSet;
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"PPPredictor getPlayerScores Error: {ex.Message}");
            }
        }

        internal virtual PPGainResult GetPlayerScorePPGain(string mapSearchString, double pp, PPPMapPool mapPool)
        {
            return GetPlayerScorePPGainInternal(mapPool.LsScores, mapSearchString, pp, mapPool.CurrentPlayer.Pp, mapPool);
        }

        internal PPGainResult GetPlayerScorePPGainInternal(List<ShortScore> lsScores, string mapSearchString, double pp, double currentTotalPP, PPPMapPool mapPool)
        {
            try
            {
                if (lsScores.Count > 0 && !string.IsNullOrEmpty(mapSearchString))
                {
                    if (pp > 0)
                    {
                        double ppAfterPlay = 0;
                        int index = 1;
                        bool newPPadded = false;
                        bool newPPSkiped = false;
                        double previousPP = 0;
                        foreach (ShortScore score in lsScores)
                        {
                            double weightedPP = WeightPP(score.Pp, index, mapPool);
                            double weightedNewPP = WeightPP(pp, index, mapPool);
                            if (score.Searchstring == mapSearchString) //skip older (lower) score
                            {
                                previousPP = score.Pp;
                                if(pp <= previousPP)
                                {
                                    ppAfterPlay = currentTotalPP; //If old score is better return currentPlayer pp => Otherwise innacuraccies while adding could result in gain
                                    break;
                                }
                                if (!newPPadded)
                                {
                                    ppAfterPlay += Math.Max(weightedPP, weightedNewPP); //Special case for improvement of your top play
                                    newPPSkiped = true;
                                    index++;
                                }
                                continue;
                            }
                            if (!newPPadded && !newPPSkiped && weightedNewPP >= weightedPP) //add new (potential) pp
                            {
                                ppAfterPlay += weightedNewPP;
                                newPPadded = true;
                                index++;
                                weightedPP = WeightPP(score.Pp, index, mapPool);
                            }
                            ppAfterPlay += weightedPP;
                            index++;
                        }
                        return new PPGainResult(Math.Round(ppAfterPlay, 2, MidpointRounding.AwayFromZero), Zeroizer(Math.Round(ppAfterPlay - currentTotalPP, 2, MidpointRounding.AwayFromZero), 0.02), pp - previousPP, _settings.PpGainCalculationType);
                    }
                    //Try to find old pp value if the map has been failed
                    ShortScore oldScore = lsScores.Find(x => x.Searchstring == mapSearchString);
                    return new PPGainResult(currentTotalPP, pp, oldScore != null ? -oldScore.Pp : 0, _settings.PpGainCalculationType);
                }
                else if(currentTotalPP == 0) //If you have not set a score yet, total is = the new pp play
                {
                    return new PPGainResult(pp, pp, pp, _settings.PpGainCalculationType);
                }
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"PPPredictor {_leaderboardInfo?.LeaderboardName} GetPlayerScorePPGain Error: {ex.Message}");
                return new PPGainResult(currentTotalPP, pp, pp, _settings.PpGainCalculationType);
            }
            return new PPGainResult(currentTotalPP, pp, pp, _settings.PpGainCalculationType);
        }

        internal async Task<RankGainResult> GetPlayerRankGain(double pp, PPPMapPool mapPool)
        {
            try
            {
                if (pp == mapPool.CurrentPlayer.Pp || pp < mapPool.CurrentPlayer.Pp) return new RankGainResult(mapPool.CurrentPlayer.Rank, mapPool.CurrentPlayer.CountryRank, mapPool.CurrentPlayer); //Fucking bullshit
                                                                                                                                                                                                                                                                                                                                //Refetch if the current rank has decrease outside of fetched range (first GetPlayerRankGain call after loading saved Session data, then update from web)
                double worstRankFetched = mapPool.LsPlayerRankings.Select(x => x.Rank).DefaultIfEmpty(Double.MaxValue).Max();
                if (!_leaderboardInfo.HasPPToRankFunctionality && mapPool.CurrentPlayer.Rank > worstRankFetched) mapPool.LsPlayerRankings = new List<PPPPlayer>();

                double bestRankFetched = mapPool.LsPlayerRankings.Select(x => x.Rank).DefaultIfEmpty(-1).Min();
                double fetchIndexPage = bestRankFetched > 0 ? Math.Floor((bestRankFetched - 1) / _leaderboardInfo.PlayerPerPages) + _leaderboardInfo.LeaderboardFirstPageIndex : Math.Floor(mapPool.CurrentPlayer.Rank / _leaderboardInfo.PlayerPerPages) + _leaderboardInfo.LeaderboardFirstPageIndex;
                bool needMoreData = true;
                int pageFetchCount = 0;
                while (needMoreData && pageFetchCount < _leaderboardInfo.PageFetchLimit)
                {
                    int indexOfBetterPlayer = mapPool.LsPlayerRankings.FindLastIndex(x => x.Pp > pp);
                    //Check if the rank before is actually the rank before, gaps can be created when using PPToRank
                    bool isNextRankActuallyNext = indexOfBetterPlayer > -1 && mapPool.LsPlayerRankings.Count - 1 > indexOfBetterPlayer &&
                                                        mapPool.LsPlayerRankings[indexOfBetterPlayer].Rank + 1 == mapPool.LsPlayerRankings[indexOfBetterPlayer + 1].Rank;
                    var ppBetterPlayer = indexOfBetterPlayer > -1 ? mapPool.LsPlayerRankings[indexOfBetterPlayer].Pp : -1d;
                    if ((_leaderboardInfo.HasPPToRankFunctionality && ((isNextRankActuallyNext && indexOfBetterPlayer != -1) || (bestRankFetched == 1 && indexOfBetterPlayer == -1)))
                        || (!_leaderboardInfo.HasPPToRankFunctionality && (indexOfBetterPlayer != -1 || fetchIndexPage == _leaderboardInfo.LeaderboardFirstPageIndex - 1 || (bestRankFetched == 1 && indexOfBetterPlayer == -1)))) //bestRankFetched == 1: Special case for first place; congratz ;)
                    {
                        //Found a better player or already fetched until rank 1
                        needMoreData = false;
                        continue;
                    }
                    else
                    {
                        bool isLastRankOnPage = false;
                        if (_leaderboardInfo.HasPPToRankFunctionality)
                        {
                            int newRank = await GetPPToRank(mapPool.Id, pp);
                            fetchIndexPage = ((newRank - 1) / _leaderboardInfo.PlayerPerPages);
                            isLastRankOnPage = newRank % _leaderboardInfo.PlayerPerPages == 0;
                        }
                        await FetchPlayerPageAndAddToList(fetchIndexPage, mapPool);
                        indexOfBetterPlayer = mapPool.LsPlayerRankings.FindIndex(x => x.Pp > pp);
                        if (_leaderboardInfo.HasPPToRankFunctionality && indexOfBetterPlayer == -1 && fetchIndexPage > _leaderboardInfo.LeaderboardFirstPageIndex)
                        {
                            //Fetch one more page, when it is the first rank on the page, since i check for the index of the better player, otherwise endless loop
                            await FetchPlayerPageAndAddToList(fetchIndexPage - 1, mapPool);
                        }
                        if (isLastRankOnPage)
                        {
                            await FetchPlayerPageAndAddToList(fetchIndexPage + 1, mapPool);
                        }
                    }
                    fetchIndexPage--;
                    pageFetchCount++;
                    bestRankFetched = mapPool.LsPlayerRankings.Select(x => x.Rank).DefaultIfEmpty(-1).Min();
                    await Task.Delay(_leaderboardInfo.TaskDelayValue);
                }
                double rankAfterPlay = mapPool.LsPlayerRankings.Where(x => Math.Round(x.Pp, 2, MidpointRounding.AwayFromZero) <= pp).Select(x => x.Rank).DefaultIfEmpty(-1).Min();
                double rankCountryAfterPlay = mapPool.LsPlayerRankings.Where(x => Math.Round(x.Pp, 2, MidpointRounding.AwayFromZero) <= pp && x.Country == mapPool.CurrentPlayer.Country).Select(x => x.CountryRank).DefaultIfEmpty(-1).Min();
                if (_leaderboardInfo.LeaderboardName == Leaderboard.NoLeaderboard.ToString())
                {
                    rankAfterPlay = rankCountryAfterPlay = 0; //Special case for when no leaderboard is active;
                }
                return new RankGainResult(rankAfterPlay, rankCountryAfterPlay, mapPool.CurrentPlayer, pageFetchCount >= _leaderboardInfo.PageFetchLimit);
            }
            catch (Exception ex)
            {
                Logging.ErrorPrint($"PPPredictor GetPlayerRankGain Error: {ex.Message}");
                return new RankGainResult();
            }
        }

        private async Task FetchPlayerPageAndAddToList(double fetchIndexPage, PPPMapPool mapPool)
        {
            List<PPPPlayer> playerscores = await GetPlayers(fetchIndexPage, mapPool);
            if (_leaderboardInfo.HasPPToRankFunctionality)
            {
                //Clear possible duplicates
                mapPool.LsPlayerRankings = mapPool.LsPlayerRankings.Where(x => !playerscores.Select(y => y.Rank).Contains(x.Rank)).ToList();
            }
            mapPool.LsPlayerRankings.AddRange(playerscores);
            mapPool.LsPlayerRankings.Sort((score1, score2) => score1.Rank.CompareTo(score2.Rank)); //sort ascending
        }

        internal double? GetPersonalBest(string mapSearchString, PPPMapPool mapPool)
        {
            ShortScore score = mapPool.LsScores.Find(x => x.Searchstring == mapSearchString);
            if (score != null)
            {
                return score.Pp;
            }
            return null;
        }

        internal virtual Task<int> GetPPToRank(string mappoolid, double pp)
        {
            return Task.FromResult(0);
        }

        internal double CalculatePPatPercentage(PPPBeatMapInfo _currentBeatMapInfo, PPPMapPool mapPool, double percentage, bool failed, bool paused)
        {
            return mapPool.Curve.CalculatePPatPercentage(_currentBeatMapInfo, percentage, failed, paused, mapPool.LeaderboardContext);
        }

        internal double CalculateMaxPP(PPPBeatMapInfo _currentBeatMapInfo, PPPMapPool mapPool)
        {
            return mapPool.Curve.CalculateMaxPP(_currentBeatMapInfo, mapPool.LeaderboardContext);
        }

        internal double WeightPP(double rawPP, int index, PPPMapPool mapPool)
        {
            return rawPP * GetWeightMulitplier(index, mapPool);
        }

        private double GetWeightMulitplier(int index, PPPMapPool mapPool)
        {
            if(mapPool.DctWeightLookup.TryGetValue(index, out double value)){
                return value;
            }
            double mult = CalculateWeightMulitplier(index, mapPool.AccumulationConstant);
            mapPool.DctWeightLookup[index] = mult;
            return mult;
        }

        protected virtual double CalculateWeightMulitplier(int index, float accumulationConstant)
        {
            return Math.Pow(accumulationConstant, (index - 1));
        }

        public static string CreateSeachString(string hash, string gameMode, int difficulty)
        {
            return $"{hash}_{gameMode}_{difficulty}".ToUpper();
        }

        public static double Zeroizer(double pp, double limit = 0.01)
        {
            if (pp < -limit || pp > limit) return pp;
            if ((pp > -limit && pp < 0) || (pp < limit && pp > 0)) return 0;
            return pp;
        }

        protected void SendMapPoolRefreshed()
        {
            OnMapPoolRefreshed?.Invoke(this, null);
        }

        internal bool IsPlayerFound(PPPMapPool mapPool)
        {
            if (!mapPool.IsPlayerFound)
            {
                return false;
            }
            return true;
        }

        internal async Task UpdateMapPoolDetails(PPPMapPool mapPool){
            if (mapPool.DtUtcLastRefresh < DateTime.UtcNow.AddDays(-1))
            {
                await InternalUpdateMapPoolDetails(mapPool);
                mapPool.DtUtcLastRefresh = DateTime.UtcNow;
            }
        }

        internal abstract List<PPPMapPoolShort> GetMapPools();

        internal abstract Task<PPPPlayer> GetPlayerInfo(long userId, PPPMapPool mapPool);

        internal abstract Task<PPPScoreCollection> GetRecentScores(string userId, int pageSize, int page, PPPMapPool mapPool);

        internal abstract Task<PPPScoreCollection> GetAllScores(string userId, PPPMapPool mapPool);

        internal abstract Task<List<PPPPlayer>> GetPlayers(double fetchIndexPage, PPPMapPool mapPool);

        internal abstract Task<PPPBeatMapInfo> GetBeatMapInfoAsync(PPPBeatMapInfo beatMapInfo, PPPMapPool mapPool);

        internal abstract PPPBeatMapInfo ApplyModifiersToBeatmapInfo(PPPBeatMapInfo beatMapInfo, PPPMapPool mapPool, GameplayModifiers gameplayModifiers, bool levelFailed = false, bool levelPaused = false);

        public abstract string CreateSeachString(string hash, BeatmapKey beatmapKey);

        internal abstract Task InternalUpdateMapPoolDetails(PPPMapPool mapPool);

        public abstract Task UpdateAvailableMapPools();
        internal abstract bool IsScoreSetOnCurrentMapPool(PPPMapPool mapPool, PPPScoreSetData score);

        internal PPPMapPoolShort FindPoolWithSyncURL(string syncUrl)
        {
            return _dctMapPool.Values.FirstOrDefault(x => x.SyncUrl == syncUrl);
        }
    }
}
