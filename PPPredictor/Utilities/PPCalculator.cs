using PPPredictor.Data;
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
        private List<PPPPlayer> _lsPlayerRankings;
        internal PPPLeaderboardInfo _leaderboardInfo;

        public PPCalculator()
        {
            _lsPlayerRankings = new List<PPPPlayer>();
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
                _leaderboardInfo.LSScores.Sort();
                while (hasMoreData)
                {
                    PPPScoreCollection playerscores = await GetRecentScores(userId, pageSize, page);
                    if (playerscores.Page * playerscores.ItemsPerPage >= playerscores.Total)
                    {
                        hasMoreData = false;
                    }
                    if (_leaderboardInfo.LSScores == null) _leaderboardInfo.LSScores = new List<ShortScore>();
                    foreach (PPPScore scores in playerscores.LsPPPScore)
                    {
                        string searchString = CreateSeachString(scores.SongHash, (int)scores.Difficulty1);
                        ShortScore previousScore = _leaderboardInfo.LSScores.Find(x => x.Searchstring == searchString);
                        ShortScore newScore = new ShortScore(searchString, scores.TimeSet, scores.Pp);
                        if (previousScore == null)
                        {
                            lsNewScores.Add(newScore);
                        }
                        else
                        {
                            if (previousScore.TimeSet >= newScore.TimeSet)
                            {
                                hasMoreData = false;
                            }
                            else
                            {
                                lsNewScores.Add(newScore);
                            }
                        }
                    }
                    page++;
                    await Task.Delay(250);
                }
                //Update after fetching all data. So when closing while fetching the incomplete data is not saved.
                foreach (ShortScore newScore in lsNewScores)
                {
                    ShortScore previousScore = _leaderboardInfo.LSScores.Find(x => x.Searchstring == newScore.Searchstring);
                    if (previousScore == null)
                    {
                        _leaderboardInfo.LSScores.Add(newScore);
                    }
                    else
                    {
                        previousScore.TimeSet = newScore.TimeSet;
                        previousScore.Pp = newScore.Pp;
                    }
                };
                _leaderboardInfo.LSScores.Sort();
            }
            catch (System.Exception ex)
            {
                Plugin.Log?.Error($"Error in PPPredictor: getPlayerScores: {ex.Message}");
            }

        }

        public PPGainResult GetPlayerScorePPGain(string mapSearchString, double pp)
        {
            if (_leaderboardInfo.LSScores.Count > 0 && !string.IsNullOrEmpty(mapSearchString) && pp > 0)
            {
                double ppAfterPlay = 0;
                int index = 1;
                bool newPPadded = false;
                bool newPPSkiped = false;
                _leaderboardInfo.LSScores.Sort((score1, score2) => score2.Pp.CompareTo(score1.Pp));
                foreach (ShortScore score in _leaderboardInfo.LSScores)
                {
                    double weightedPP = WeightPP(score.Pp, index);
                    double weightedNewPP = WeightPP(pp, index);
                    if (score.Searchstring == mapSearchString) //skip older (lower) score
                    {
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
                        weightedPP = WeightPP(score.Pp, index);
                    }
                    ppAfterPlay += weightedPP;
                    index++;
                }
                return new PPGainResult(Math.Round(ppAfterPlay, 2, MidpointRounding.AwayFromZero), Math.Round(ppAfterPlay - _leaderboardInfo.CurrentPlayer.Pp, 2, MidpointRounding.AwayFromZero));
            }
            return new PPGainResult(_leaderboardInfo.CurrentPlayer.Pp, pp);
        }

        public async Task<RankGainResult> GetPlayerRankGain(double pp)
        {
            //Refetch if the current rank has decrease outside of fetched range (first GetPlayerRankGain call after loading saved Session data, then update from web)
            double worstRankFetched = _lsPlayerRankings.Select(x => x.Rank).DefaultIfEmpty(Double.MaxValue).Max();
            if (_leaderboardInfo.CurrentPlayer.Rank > worstRankFetched) _lsPlayerRankings = new List<PPPPlayer>();

            double bestRankFetched = _lsPlayerRankings.Select(x => x.Rank).DefaultIfEmpty(-1).Min();
            double fetchIndexPage = bestRankFetched > 0 ? Math.Floor((bestRankFetched - 1) / 50) + 1 : Math.Floor(_leaderboardInfo.CurrentPlayer.Rank / 50) + 1;
            bool needMoreData = true;
            while (needMoreData)
            {
                int indexOfBetterPlayer = _lsPlayerRankings.FindIndex(x => x.Pp > pp);
                if (indexOfBetterPlayer != -1 || fetchIndexPage == 1)
                {
                    //Found a better player or already fetched until rank 1
                    needMoreData = false;
                    continue;
                }
                else
                {
                    List<PPPPlayer> playerscores = await GetPlayers(fetchIndexPage);
                    _lsPlayerRankings.AddRange(playerscores);
                }
                fetchIndexPage--;
                await Task.Delay(250);
            }
            double rankAfterPlay = _lsPlayerRankings.Where(x => x.Pp <= pp).Select(x => x.Rank).DefaultIfEmpty(-1).Min();
            double rankCountryAfterPlay = _lsPlayerRankings.Where(x => x.Pp <= pp && x.Country == _leaderboardInfo.CurrentPlayer.Country).Select(x => x.CountryRank).DefaultIfEmpty(-1).Min();
            return new RankGainResult(rankAfterPlay, rankCountryAfterPlay, _leaderboardInfo.CurrentPlayer);
        }

        public double WeightPP(double rawPP, int index)
        {
            return rawPP * Math.Pow(0.965, (index - 1));
        }

        public string CreateSeachString(string hash, int difficulty)
        {
            return $"{hash}_{difficulty}";
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

        public abstract double CalculatePPatPercentage(double star, double percentage);

        public abstract Task<double> GetStarsForBeatmapAsync(LevelSelectionNavigationController lvlSelectionNavigationCtrl, IDifficultyBeatmap beatmap);

        public abstract double ApplyModifierMultiplierToStars(double baseStars, GameplayModifiers gameplayModifiers);

    }
}
