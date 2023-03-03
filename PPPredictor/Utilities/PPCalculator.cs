﻿using PPPredictor.Data;
using SongCore.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using scoresaberapi;

namespace PPPredictor.Utilities
{
    public abstract class PPCalculator
    {
        internal PPPLeaderboardInfo _leaderboardInfo;
        protected int playerPerPages = 0; //Cause a null reference if not set ;)

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

        public async Task GetPlayerScores(string userId, int pageSize)
        {
            try
            {
                bool hasMoreData = true;
                int page = 1;
                List<ShortScore> lsNewScores = new List<ShortScore>();
                DateTimeOffset dtNewLastScoreSet = new DateTime(2000, 1, 1);
                while (hasMoreData)
                {
                    PPPScoreCollection playerscores = await GetRecentScores(userId, pageSize, page);
                    if (playerscores.Page * playerscores.ItemsPerPage >= playerscores.Total)
                    {
                        hasMoreData = false;
                    }
                    if (_leaderboardInfo.CurrentMapPool.LsScores == null) _leaderboardInfo.CurrentMapPool.LsScores = new List<ShortScore>();
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
                    await Task.Delay(250);
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
                Plugin.Log?.Error($"PPPredictor getPlayerScores Error: {ex.Message}");
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
                        return new PPGainResult(Math.Round(ppAfterPlay, 2, MidpointRounding.AwayFromZero), Zeroizer(Math.Round(ppAfterPlay - _leaderboardInfo.CurrentMapPool.CurrentPlayer.Pp, 2, MidpointRounding.AwayFromZero)), pp - previousPP);
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
                Plugin.Log?.Error($"PPPredictor {_leaderboardInfo?.LeaderboardName} GetPlayerScorePPGain Error: {ex.Message}");
                return new PPGainResult(_leaderboardInfo.CurrentMapPool.CurrentPlayer.Pp, pp, pp);
            }
            return new PPGainResult(_leaderboardInfo.CurrentMapPool.CurrentPlayer.Pp, pp, pp);
        }

        public async Task<RankGainResult> GetPlayerRankGain(double pp)
        {
            try
            {
                if (pp == _leaderboardInfo.CurrentMapPool.CurrentPlayer.Pp) return new RankGainResult(_leaderboardInfo.CurrentMapPool.CurrentPlayer.Rank, _leaderboardInfo.CurrentMapPool.CurrentPlayer.CountryRank, _leaderboardInfo.CurrentMapPool.CurrentPlayer); //Fucking bullshit
                //Refetch if the current rank has decrease outside of fetched range (first GetPlayerRankGain call after loading saved Session data, then update from web)
                double worstRankFetched = _leaderboardInfo.CurrentMapPool.LsPlayerRankings.Select(x => x.Rank).DefaultIfEmpty(Double.MaxValue).Max();
                if (_leaderboardInfo.CurrentMapPool.CurrentPlayer.Rank > worstRankFetched) _leaderboardInfo.CurrentMapPool.LsPlayerRankings = new List<PPPPlayer>();

                double bestRankFetched = _leaderboardInfo.CurrentMapPool.LsPlayerRankings.Select(x => x.Rank).DefaultIfEmpty(-1).Min();
                double fetchIndexPage = bestRankFetched > 0 ? Math.Floor((bestRankFetched - 1) / playerPerPages) + 1 : Math.Floor(_leaderboardInfo.CurrentMapPool.CurrentPlayer.Rank / playerPerPages) + 1;
                bool needMoreData = true;
                while (needMoreData)
                {
                    int indexOfBetterPlayer = _leaderboardInfo.CurrentMapPool.LsPlayerRankings.FindIndex(x => x.Pp > pp);
                    var ppBetterPlayer = indexOfBetterPlayer > -1 ? _leaderboardInfo.CurrentMapPool.LsPlayerRankings[indexOfBetterPlayer].Pp : -1d;
                    if (indexOfBetterPlayer != -1 || fetchIndexPage == 0 || (bestRankFetched == 1 && indexOfBetterPlayer == -1)) //bestRankFetched == 1: Special case for first place; congratz ;)
                    {
                        //Found a better player or already fetched until rank 1
                        needMoreData = false;
                        continue;
                    }
                    else
                    {
                        List<PPPPlayer> playerscores = await GetPlayers(fetchIndexPage);
                        _leaderboardInfo.CurrentMapPool.LsPlayerRankings.AddRange(playerscores);
                    }
                    fetchIndexPage--;
                    await Task.Delay(250);
                }
                double rankAfterPlay = _leaderboardInfo.CurrentMapPool.LsPlayerRankings.Where(x => Math.Round(x.Pp, 2, MidpointRounding.AwayFromZero) <= pp).Select(x => x.Rank).DefaultIfEmpty(-1).Min();
                double rankCountryAfterPlay = _leaderboardInfo.CurrentMapPool.LsPlayerRankings.Where(x => Math.Round(x.Pp, 2, MidpointRounding.AwayFromZero) <= pp && x.Country == _leaderboardInfo.CurrentMapPool.CurrentPlayer.Country).Select(x => x.CountryRank).DefaultIfEmpty(-1).Min();
                if(_leaderboardInfo.LeaderboardName == Leaderboard.NoLeaderboard.ToString())
                {
                    rankAfterPlay = rankCountryAfterPlay = 0; //Special case for when no leaderboard is active;
                }
                return new RankGainResult(rankAfterPlay, rankCountryAfterPlay, _leaderboardInfo.CurrentMapPool.CurrentPlayer);
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPPredictor GetPlayerRankGain Error: {ex.Message}");
                return new RankGainResult();
            }
        }

        internal double CalculatePPatPercentage(double star, double percentage, bool failed)
        {
            return _leaderboardInfo.CurrentMapPool.Curve.CalculatePPatPercentage(star, percentage, failed);
        }

        public double WeightPP(double rawPP, int index, float accumulationConstant)
        {
            return rawPP * Math.Pow(accumulationConstant, (index - 1));
        }

        public string CreateSeachString(string hash, string gameMode, int difficulty)
        {
            return $"{hash}_{gameMode}_{difficulty}".ToUpper();
        }

        public double Zeroizer(double pp)
        {
            if (pp < -0.01 || pp > 0.01) return pp;
            if ((pp > -0.01 && pp < 0) || (pp < 0.01 && pp > 0)) return 0;
            return pp;
        }

        protected abstract Task<PPPPlayer> GetPlayerInfo(long userId);
        //TODO: Test when user does not exist yet

        protected abstract Task<PPPScoreCollection> GetRecentScores(string userId, int pageSize, int page);

        protected abstract Task<List<PPPPlayer>> GetPlayers(double fetchIndexPage);


        public abstract Task<PPPBeatMapInfo> GetBeatMapInfoAsync(LevelSelectionNavigationController lvlSelectionNavigationCtrl, IDifficultyBeatmap beatmap);

        public abstract double ApplyModifierMultiplierToStars(PPPBeatMapInfo beatMapInfo, GameplayModifiers gameplayModifiers, bool levelFailed = false);

        public abstract string CreateSeachString(string hash, IDifficultyBeatmap beatmap);

        public abstract Task UpdateMapPoolDetails(PPPMapPool mapPool);
    }
}
