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
        private readonly List<PPPPlayer> lsPlayerRankings;

        public PPCalculator()
        {
            lsPlayerRankings = new List<PPPPlayer>();
        }

        public async Task<PPPPlayer> GetProfile(string userId)
        {
            if (long.TryParse(userId, out long longUserId))
            {
                var player = await getPlayerInfo(longUserId); 
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
                while (hasMoreData)
                {
                    PPPScoreCollection playerscores = await getRecentScores(userId, pageSize, page);
                    if (playerscores.Page * playerscores.ItemsPerPage >= playerscores.Total)
                    {
                        hasMoreData = false;
                    }
                    if (Plugin.ProfileInfo.LSScores == null) Plugin.ProfileInfo.LSScores = new List<ShortScore>();
                    foreach (PPPScore scores in playerscores.LsPPPScore)
                    {
                        string searchString = CreateSeachString(scores.SongHash, (int)scores.Difficulty1);
                        ShortScore previousScore = Plugin.ProfileInfo.LSScores.Find(x => x.Searchstring == searchString);
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
                    ShortScore previousScore = Plugin.ProfileInfo.LSScores.Find(x => x.Searchstring == newScore.Searchstring);
                    if (previousScore == null)
                    {
                        Plugin.ProfileInfo.LSScores.Add(newScore);
                    }
                    else
                    {
                        previousScore.TimeSet = newScore.TimeSet;
                        previousScore.Pp = newScore.Pp;
                    }
                };
                Plugin.ProfileInfo.LSScores.Sort();
            }
            catch (System.Exception ex)
            {
                Plugin.Log?.Error($"Error in PPPredictor: getPlayerScores: {ex.Message}");
            }

        }

        public PPGainResult GetPlayerScorePPGain(string mapSearchString, double pp)
        {
            Plugin.Log?.Error($"{mapSearchString} - {pp}");
            if (Plugin.ProfileInfo.LSScores.Count > 0 && !string.IsNullOrEmpty(mapSearchString) && pp > 0)
            {
                Plugin.Log?.Error($"lsscores found {Plugin.ProfileInfo.LSScores.Count}");
                double ppAfterPlay = 0;
                int index = 1;
                bool newPPadded = false;
                bool newPPSkiped = false;
                Plugin.ProfileInfo.LSScores.Sort((score1, score2) => score2.Pp.CompareTo(score1.Pp));
                foreach (ShortScore score in Plugin.ProfileInfo.LSScores)
                {
                    Plugin.Log?.Error($"searchstring {score.Searchstring} - {score.Pp}");
                    double weightedPP = WeightPP(score.Pp, index);
                    double weightedNewPP = WeightPP(pp, index);
                    if (score.Searchstring == mapSearchString) //skip older (lower) score
                    {
                        if (!newPPadded)
                        {
                            ppAfterPlay += Math.Max(weightedPP, weightedNewPP); //Special case for improvement of your top play
                            Plugin.Log?.Error($"skip ppAfterPlay {ppAfterPlay} - {weightedPP} - {weightedNewPP}");
                            newPPSkiped = true;
                            index++;
                        }
                        Plugin.Log?.Error($"skip");
                        continue;
                        //Nominated rausschmissen ...................................................
                    }
                    if (!newPPadded && !newPPSkiped && weightedNewPP >= weightedPP) //add new (potential) pp
                    {
                        Plugin.Log?.Error($"add new (potential) pp {weightedNewPP}");
                        ppAfterPlay += weightedNewPP;
                        newPPadded = true;
                        index++;
                        weightedPP = WeightPP(score.Pp, index);
                    }
                    ppAfterPlay += weightedPP;
                    Plugin.Log?.Error($"ppAfter Play {ppAfterPlay} - {weightedPP}");
                    index++;
                }
                return new PPGainResult(Math.Round(ppAfterPlay, 2, MidpointRounding.AwayFromZero), Math.Round(ppAfterPlay - Plugin.ProfileInfo.CurrentPlayer.Pp, 2, MidpointRounding.AwayFromZero));
            }
            return new PPGainResult(Plugin.ProfileInfo.CurrentPlayer.Pp, pp);
        }

        public async Task<RankGainResult> GetPlayerRankGain(double pp)
        {
            double bestRankFetched = lsPlayerRankings.Select(x => x.Rank).DefaultIfEmpty(-1).Min();
            double fetchIndexPage = bestRankFetched > 0 ? Math.Floor((bestRankFetched - 1) / 50) + 1 : Math.Floor(Plugin.ProfileInfo.CurrentPlayer.Rank / 50) + 1;
            bool needMoreData = true;
            while (needMoreData)
            {
                int indexOfBetterPlayer = lsPlayerRankings.FindIndex(x => x.Pp > pp);
                if (indexOfBetterPlayer != -1 || fetchIndexPage == 1)
                {
                    //Found a better player or already fetched until rank 1
                    needMoreData = false;
                    continue;
                }
                else
                {
                    List<PPPPlayer> playerscores = await getPlayers(fetchIndexPage);
                    lsPlayerRankings.AddRange(playerscores);
                }
                fetchIndexPage--;
                await Task.Delay(250);
            }
            double rankAfterPlay = lsPlayerRankings.Where(x => x.Pp <= pp).Select(x => x.Rank).Min();

            double rankCountryAfterPlay = lsPlayerRankings.Where(x => x.Pp <= pp && x.Country == Plugin.ProfileInfo.CurrentPlayer.Country).Select(x => x.CountryRank).Min();
            return new RankGainResult(rankAfterPlay, rankCountryAfterPlay);
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

        protected abstract Task<PPPPlayer> getPlayerInfo(long userId);

        protected abstract Task<PPPScoreCollection> getRecentScores(string userId, int pageSize, int page);

        protected abstract Task<List<PPPPlayer>> getPlayers(double fetchIndexPage);

        public abstract double CalculatePPatPercentage(double star, double percentage);

        public abstract Task<double> GetStarsForBeatmapAsync(LevelSelectionNavigationController lvlSelectionNavigationCtrl, IDifficultyBeatmap beatmap);
    }
}
