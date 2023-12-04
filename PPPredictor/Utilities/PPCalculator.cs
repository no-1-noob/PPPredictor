using PPPredictor.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PPPredictor.Utilities
{
    abstract class PPCalculator
    {
        internal PPPLeaderboardInfo _leaderboardInfo;
        protected int playerPerPages = 0; //Cause a null reference if not set ;)
        protected bool hasGetAllScoresFunctionality = false;
        protected bool hasPPToRankFunctionality = false;
        protected int taskDelayValue = 250;
        internal bool hasOldDotRanking = true;
        private int pageFetchLimit = 5;
        public event EventHandler OnMapPoolRefreshed;

        public PPCalculator()
        {
        }

        public async Task<PPPPlayer> GetProfile(string userId)
        {
            if (long.TryParse(userId, out long longUserId))
            {
                var player = await GetPlayerInfo(longUserId); 
                return player;
            }
            return new PPPPlayer();
        }

        public async Task GetPlayerScores(string userId, int pageSize, int largePageSize)
        {
            try
            {
                bool hasNoScores = false;
                bool hasMoreData = true;
                int page = 1;
                List<ShortScore> lsNewScores = new List<ShortScore>();
                DateTimeOffset dtNewLastScoreSet = new DateTime(2000, 1, 1);
                while (hasMoreData)
                {
                    if (_leaderboardInfo.CurrentMapPool.LsScores == null) _leaderboardInfo.CurrentMapPool.LsScores = new List<ShortScore>();
                    PPPScoreCollection playerscores = null;
                    hasNoScores = _leaderboardInfo.CurrentMapPool.LsScores.Count == 0;
                    if (hasGetAllScoresFunctionality && hasNoScores)
                    {
                        playerscores = await GetAllScores(userId);
                        hasMoreData = false;
                    }
                    else {
                        playerscores = await GetRecentScores(userId, hasNoScores ? largePageSize: pageSize, page);
                        if (playerscores.Page * playerscores.ItemsPerPage >= playerscores.Total)
                        {
                            hasMoreData = false;
                        }
                    }
                    foreach (PPPScore scores in playerscores.LsPPPScore)
                    {
                        string searchString = CreateSeachString(scores.SongHash, scores.GameMode, (int)scores.Difficulty1);
                        ShortScore previousScore = _leaderboardInfo.CurrentMapPool.LsScores.Find(x => x.Searchstring == searchString);
                        ShortScore newScore = new ShortScore(searchString, scores.Pp);
                        if(newScore.Pp > 0) // Do not cache unranked scores
                        {
                            if (previousScore == null)
                            {
                                lsNewScores.Add(newScore);
                            }
                            else
                            {
                                if (_leaderboardInfo.CurrentMapPool.DtLastScoreSet >= scores.TimeSet)
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
                    await Task.Delay(taskDelayValue);
                }
                //Update after fetching all data. So when closing while fetching the incomplete data is not saved.
                foreach (ShortScore newScore in lsNewScores)
                {
                    ShortScore previousScore = _leaderboardInfo.CurrentMapPool.LsScores.Find(x => x.Searchstring == newScore.Searchstring);
                    if (previousScore == null)
                    {
                        _leaderboardInfo.CurrentMapPool.LsScores.Add(newScore);
                    }
                    else
                    {
                        previousScore.Pp = newScore.Pp;
                    }
                };
                _leaderboardInfo.CurrentMapPool.DtLastScoreSet = dtNewLastScoreSet > _leaderboardInfo.CurrentMapPool.DtLastScoreSet ? dtNewLastScoreSet : _leaderboardInfo.CurrentMapPool.DtLastScoreSet;
            }
            catch (Exception ex)
            {
                Plugin.ErrorPrint($"PPPredictor getPlayerScores Error: {ex.Message}");
            }
        }

        public PPGainResult GetPlayerScorePPGain(string mapSearchString, double pp)
        {
            try
            {
                if (_leaderboardInfo.CurrentMapPool.LsScores.Count > 0 && !string.IsNullOrEmpty(mapSearchString))
                {
                    if (pp > 0)
                    {
                        double ppAfterPlay = 0;
                        int index = 1;
                        bool newPPadded = false;
                        bool newPPSkiped = false;
                        double previousPP = 0;
                        _leaderboardInfo.CurrentMapPool.LsScores.Sort((score1, score2) => score2.Pp.CompareTo(score1.Pp));
                        foreach (ShortScore score in _leaderboardInfo.CurrentMapPool.LsScores)
                        {
                            double weightedPP = WeightPP(score.Pp, index, _leaderboardInfo.CurrentMapPool.AccumulationConstant);
                            double weightedNewPP = WeightPP(pp, index, _leaderboardInfo.CurrentMapPool.AccumulationConstant);
                            if (score.Searchstring == mapSearchString) //skip older (lower) score
                            {
                                previousPP = score.Pp;
                                if(pp <= previousPP)
                                {
                                    ppAfterPlay = _leaderboardInfo.CurrentMapPool.CurrentPlayer.Pp; //If old score is better return currentPlayer pp => Otherwise innacuraccies while adding could result in gain
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
                                weightedPP = WeightPP(score.Pp, index, _leaderboardInfo.CurrentMapPool.AccumulationConstant);
                            }
                            ppAfterPlay += weightedPP;
                            index++;
                        }
                        return new PPGainResult(Math.Round(ppAfterPlay, 2, MidpointRounding.AwayFromZero), Zeroizer(Math.Round(ppAfterPlay - _leaderboardInfo.CurrentMapPool.CurrentPlayer.Pp, 2, MidpointRounding.AwayFromZero), 0.02), pp - previousPP);
                    }
                    //Try to find old pp value if the map has been failed
                    ShortScore oldScore = _leaderboardInfo.CurrentMapPool.LsScores.Find(x => x.Searchstring == mapSearchString);
                    return new PPGainResult(_leaderboardInfo.CurrentMapPool.CurrentPlayer.Pp, pp, oldScore != null ? -oldScore.Pp : 0);
                }
                else if(_leaderboardInfo.CurrentMapPool.CurrentPlayer.Pp == 0) //If you have not set a score yet, total is = the new pp play
                {
                    return new PPGainResult(pp, pp, pp);
                }
            }
            catch (Exception ex)
            {
                Plugin.ErrorPrint($"PPPredictor {_leaderboardInfo?.LeaderboardName} GetPlayerScorePPGain Error: {ex.Message}");
                return new PPGainResult(_leaderboardInfo.CurrentMapPool.CurrentPlayer.Pp, pp, pp);
            }
            return new PPGainResult(_leaderboardInfo.CurrentMapPool.CurrentPlayer.Pp, pp, pp);
        }

        public async Task<RankGainResult> GetPlayerRankGain(double pp)
        {
            try
            {
                if (pp == _leaderboardInfo.CurrentMapPool.CurrentPlayer.Pp || pp < _leaderboardInfo.CurrentMapPool.CurrentPlayer.Pp) return new RankGainResult(_leaderboardInfo.CurrentMapPool.CurrentPlayer.Rank, _leaderboardInfo.CurrentMapPool.CurrentPlayer.CountryRank, _leaderboardInfo.CurrentMapPool.CurrentPlayer); //Fucking bullshit
                                                                                                                                                                                                                                                                                                                                //Refetch if the current rank has decrease outside of fetched range (first GetPlayerRankGain call after loading saved Session data, then update from web)
                double worstRankFetched = _leaderboardInfo.CurrentMapPool.LsPlayerRankings.Select(x => x.Rank).DefaultIfEmpty(Double.MaxValue).Max();
                if (!hasPPToRankFunctionality && _leaderboardInfo.CurrentMapPool.CurrentPlayer.Rank > worstRankFetched) _leaderboardInfo.CurrentMapPool.LsPlayerRankings = new List<PPPPlayer>();

                double bestRankFetched = _leaderboardInfo.CurrentMapPool.LsPlayerRankings.Select(x => x.Rank).DefaultIfEmpty(-1).Min();
                double fetchIndexPage = bestRankFetched > 0 ? Math.Floor((bestRankFetched - 1) / playerPerPages) + _leaderboardInfo.LeaderboardFirstPageIndex : Math.Floor(_leaderboardInfo.CurrentMapPool.CurrentPlayer.Rank / playerPerPages) + _leaderboardInfo.LeaderboardFirstPageIndex;
                bool needMoreData = true;
                int pageFetchCount = 0;
                while (needMoreData && pageFetchCount < pageFetchLimit)
                {
                    int indexOfBetterPlayer = _leaderboardInfo.CurrentMapPool.LsPlayerRankings.FindLastIndex(x => x.Pp > pp);
                    //Check if the rank before is actually the rank before, gaps can be created when using PPToRank
                    bool isNextRankActuallyNext = indexOfBetterPlayer > -1 && _leaderboardInfo.CurrentMapPool.LsPlayerRankings.Count - 1 > indexOfBetterPlayer &&
                                                        _leaderboardInfo.CurrentMapPool.LsPlayerRankings[indexOfBetterPlayer].Rank + 1 == _leaderboardInfo.CurrentMapPool.LsPlayerRankings[indexOfBetterPlayer + 1].Rank;
                    var ppBetterPlayer = indexOfBetterPlayer > -1 ? _leaderboardInfo.CurrentMapPool.LsPlayerRankings[indexOfBetterPlayer].Pp : -1d;
                    if ((hasPPToRankFunctionality && ((isNextRankActuallyNext && indexOfBetterPlayer != -1) || (bestRankFetched == 1 && indexOfBetterPlayer == -1)))
                        || (!hasPPToRankFunctionality && (indexOfBetterPlayer != -1 || fetchIndexPage == _leaderboardInfo.LeaderboardFirstPageIndex - 1 || (bestRankFetched == 1 && indexOfBetterPlayer == -1)))) //bestRankFetched == 1: Special case for first place; congratz ;)
                    {
                        //Found a better player or already fetched until rank 1
                        needMoreData = false;
                        continue;
                    }
                    else
                    {
                        bool isLastRankOnPage = false;
                        if (hasPPToRankFunctionality)
                        {
                            int newRank = await GetPPToRank(_leaderboardInfo.CurrentMapPool.Id, pp);
                            fetchIndexPage = ((newRank - 1) / playerPerPages);
                            isLastRankOnPage = newRank % playerPerPages == 0;
                        }
                        await FetchPlayerPageAndAddToList(fetchIndexPage);
                        indexOfBetterPlayer = _leaderboardInfo.CurrentMapPool.LsPlayerRankings.FindIndex(x => x.Pp > pp);
                        if (hasPPToRankFunctionality && indexOfBetterPlayer == -1 && fetchIndexPage > _leaderboardInfo.LeaderboardFirstPageIndex)
                        {
                            //Fetch one more page, when it is the first rank on the page, since i check for the index of the better player, otherwise endless loop
                            await FetchPlayerPageAndAddToList(fetchIndexPage - 1);
                        }
                        if (isLastRankOnPage)
                        {
                            await FetchPlayerPageAndAddToList(fetchIndexPage + 1);
                        }
                    }
                    fetchIndexPage--;
                    pageFetchCount++;
                    bestRankFetched = _leaderboardInfo.CurrentMapPool.LsPlayerRankings.Select(x => x.Rank).DefaultIfEmpty(-1).Min();
                    await Task.Delay(taskDelayValue);
                }
                double rankAfterPlay = _leaderboardInfo.CurrentMapPool.LsPlayerRankings.Where(x => Math.Round(x.Pp, 2, MidpointRounding.AwayFromZero) <= pp).Select(x => x.Rank).DefaultIfEmpty(-1).Min();
                double rankCountryAfterPlay = _leaderboardInfo.CurrentMapPool.LsPlayerRankings.Where(x => Math.Round(x.Pp, 2, MidpointRounding.AwayFromZero) <= pp && x.Country == _leaderboardInfo.CurrentMapPool.CurrentPlayer.Country).Select(x => x.CountryRank).DefaultIfEmpty(-1).Min();
                if (_leaderboardInfo.LeaderboardName == Leaderboard.NoLeaderboard.ToString())
                {
                    rankAfterPlay = rankCountryAfterPlay = 0; //Special case for when no leaderboard is active;
                }
                return new RankGainResult(rankAfterPlay, rankCountryAfterPlay, _leaderboardInfo.CurrentMapPool.CurrentPlayer, pageFetchCount >= pageFetchLimit);
            }
            catch (Exception ex)
            {
                Plugin.ErrorPrint($"PPPredictor GetPlayerRankGain Error: {ex.Message}");
                return new RankGainResult();
            }
        }

        private async Task FetchPlayerPageAndAddToList(double fetchIndexPage)
        {
            List<PPPPlayer> playerscores = await GetPlayers(fetchIndexPage);
            if (hasPPToRankFunctionality)
            {
                //Clear possible duplicates
                _leaderboardInfo.CurrentMapPool.LsPlayerRankings = _leaderboardInfo.CurrentMapPool.LsPlayerRankings.Where(x => !playerscores.Select(y => y.Rank).Contains(x.Rank)).ToList();
            }
            _leaderboardInfo.CurrentMapPool.LsPlayerRankings.AddRange(playerscores);
            _leaderboardInfo.CurrentMapPool.LsPlayerRankings.Sort((score1, score2) => score1.Rank.CompareTo(score2.Rank)); //sort ascending
        }

        internal double? GetPersonalBest(string mapSearchString)
        {
            ShortScore score = _leaderboardInfo.CurrentMapPool.LsScores.Find(x => x.Searchstring == mapSearchString);
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

        internal double CalculatePPatPercentage(PPPBeatMapInfo _currentBeatMapInfo, double percentage, bool failed, bool paused)
        {
            return _leaderboardInfo.CurrentMapPool.Curve.CalculatePPatPercentage(_currentBeatMapInfo, percentage, failed, paused, _leaderboardInfo.CurrentMapPool.LeaderboardContext);
        }

        internal double CalculateMaxPP(PPPBeatMapInfo _currentBeatMapInfo)
        {
            return _leaderboardInfo.CurrentMapPool.Curve.CalculateMaxPP(_currentBeatMapInfo, _leaderboardInfo.CurrentMapPool.LeaderboardContext);
        }

        public double WeightPP(double rawPP, int index, float accumulationConstant)
        {
            return rawPP * Math.Pow(accumulationConstant, (index - 1));
        }

        public string CreateSeachString(string hash, string gameMode, int difficulty)
        {
            return $"{hash}_{gameMode}_{difficulty}".ToUpper();
        }

        public double Zeroizer(double pp, double limit = 0.01)
        {
            if (pp < -limit || pp > limit) return pp;
            if ((pp > -limit && pp < 0) || (pp < limit && pp > 0)) return 0;
            return pp;
        }

        protected void SendMapPoolRefreshed()
        {
            OnMapPoolRefreshed?.Invoke(this, null);
        }

        protected abstract Task<PPPPlayer> GetPlayerInfo(long userId);
        //TODO: Test when user does not exist yet

        protected abstract Task<PPPScoreCollection> GetRecentScores(string userId, int pageSize, int page);

        protected abstract Task<PPPScoreCollection> GetAllScores(string userId);

        protected abstract Task<List<PPPPlayer>> GetPlayers(double fetchIndexPage);

        public abstract Task<PPPBeatMapInfo> GetBeatMapInfoAsync(PPPBeatMapInfo beatMapInfo);

        public abstract PPPBeatMapInfo ApplyModifiersToBeatmapInfo(PPPBeatMapInfo beatMapInfo, GameplayModifiers gameplayModifiers, bool levelFailed = false, bool levelPaused = false);

        public abstract string CreateSeachString(string hash, IDifficultyBeatmap beatmap);

        public abstract Task UpdateMapPoolDetails(PPPMapPool mapPool);
    }
}
