using PPPredictor.Data;
using scoresaberapi;
using SongCore.Utilities;
using SongDetailsCache;
using SongDetailsCache.Structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PPPredictor.Utilities
{
    public class PPCalculator
    {
        static readonly HttpClient httpClient = new HttpClient();
        private readonly double basePPMultiplier = 42.117208413;
        private readonly double[,] arrPPCurve = new double[32, 2] {
            {1, 7},
            {0.999, 6.24},
            {0.9975, 5.31},
            {0.995, 4.14},
            {0.9925, 3.31},
            {0.99, 2.73},
            {0.9875, 2.31},
            {0.985, 2.0},
            {0.9825, 1.775},
            {0.98, 1.625},
            {0.9775, 1.515},
            {0.975, 1.43},
            {0.9725, 1.36},
            {0.97, 1.3},
            {0.965, 1.195},
            {0.96, 1.115},
            {0.955, 1.05},
            {0.95, 1},
            {0.94, 0.94},
            {0.93, 0.885},
            {0.92, 0.835},
            {0.91, 0.79},
            {0.9, 0.75},
            {0.875, 0.655},
            {0.85, 0.57},
            {0.825, 0.51},
            {0.8, 0.47},
            {0.75, 0.40},
            {0.7, 0.34},
            {0.65, 0.29},
            {0.6, 0.25},
            {0.0, 0.0}, };
        private readonly List<Player> lsPlayerRankings;
        private SongDetails SongDetails { get; }

        public PPCalculator()
        {
            SongDetails = SongDetails.Init().Result;
            lsPlayerRankings = new List<Player>();
        }

        public double CalculateBasePPForBeatmapAsync(LevelSelectionNavigationController lvlSelectionNavigationCtrl, IDifficultyBeatmap beatmap)
        {
            if (lvlSelectionNavigationCtrl.selectedBeatmapLevel is CustomBeatmapLevel selectedCustomBeatmapLevel)
            {
                if (SongDetails.songs.FindByHash(Hashing.GetCustomLevelHash(selectedCustomBeatmapLevel), out Song song))
                {
                    if (song.GetDifficulty(out SongDifficulty songDiff, (MapDifficulty)beatmap.difficulty))
                    {
                        return songDiff.stars * basePPMultiplier;
                    }
                }
                return 0;
            }
            return 0;
        }

        public double CalculatePPatPercentage(double basePP, double percentage)
        {
            percentage /= 100.0;
            double multiplier = CalculateMultiplierAtPercentage(percentage);
            return multiplier * basePP;
        }

        private double CalculateMultiplierAtPercentage(double percentage)
        {
            for (int i = 0; i < arrPPCurve.GetLength(0); i++)
            {
                if (arrPPCurve[i, 0] == percentage)
                {
                    return arrPPCurve[i, 1];
                }
                else
                {
                    if (arrPPCurve[i + 1, 0] < percentage)
                    {
                        return CalculateMultiplierAtPercentageWithLine((arrPPCurve[i + 1, 0], arrPPCurve[i + 1, 1]), (arrPPCurve[i, 0], arrPPCurve[i, 1]), percentage);
                    }
                }
            }
            return 0;
        }

        private double CalculateMultiplierAtPercentageWithLine((double x, double y) p1, (double x, double y) p2, double percentage)
        {
            double m = (p2.y - p1.y) / (p2.x - p1.x);
            double b = p1.y - (m * p1.x);
            return m * percentage + b;
        }

        public async Task<Player> GetProfile(string userId)
        {
            if (long.TryParse(userId, out long longUserId))
            {
                var scoreSaberClient = new scoresaberapi.scoresaberapi(httpClient);
                Task<Player> playerInfo = scoreSaberClient.BasicAsync(longUserId);
                var player = await playerInfo;
                return player;
            }
            return new Player();
        }

        public async Task GetPlayerScores(string userId, int pageSize)
        {
            try
            {
                var scoreSaberClient = new scoresaberapi.scoresaberapi(httpClient);
                bool hasMoreData = true;
                int page = 1;
                List<ShortScore> lsNewScores = new List<ShortScore>();
                while (hasMoreData)
                {
                    PlayerScoreCollection playerscores = await scoreSaberClient.Scores3Async(userId, pageSize, Sort.Recent, page, true);
                    if (playerscores.Metadata.Page * playerscores.Metadata.ItemsPerPage >= playerscores.Metadata.Total)
                    {
                        hasMoreData = false;
                    }
                    if (Plugin.ProfileInfo.LSScores == null) Plugin.ProfileInfo.LSScores = new List<ShortScore>();
                    foreach (PlayerScore scores in playerscores.PlayerScores)
                    {
                        string searchString = CreateSeachString(scores.Leaderboard.SongHash, (int)scores.Leaderboard.Difficulty.Difficulty1);
                        ShortScore previousScore = Plugin.ProfileInfo.LSScores.Find(x => x.Searchstring == searchString);
                        ShortScore newScore = new ShortScore(searchString, scores.Score.TimeSet, scores.Score.Pp);
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
            if (Plugin.ProfileInfo.LSScores.Count > 0 && !string.IsNullOrEmpty(mapSearchString) && pp > 0)
            {
                double ppAfterPlay = 0;
                int index = 1;
                bool newPPadded = false;
                bool newPPSkiped = false;
                Plugin.ProfileInfo.LSScores.Sort((score1, score2) => score2.Pp.CompareTo(score1.Pp));
                foreach (ShortScore score in Plugin.ProfileInfo.LSScores)
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
                return new PPGainResult(Math.Round(ppAfterPlay, 2, MidpointRounding.AwayFromZero), Math.Round(ppAfterPlay - Plugin.ProfileInfo.CurrentPlayer.Pp, 2, MidpointRounding.AwayFromZero));
            }
            return new PPGainResult(Plugin.ProfileInfo.CurrentPlayer.Pp, pp);
        }

        public async Task<RankGainResult> GetPlayerRankGain(double pp)
        {
            double bestRankFetched = lsPlayerRankings.Select(x => x.Rank).DefaultIfEmpty(-1).Min();
            double fetchIndexPage = bestRankFetched > 0 ? Math.Floor((bestRankFetched - 1) / 50) + 1 : Math.Floor(Plugin.ProfileInfo.CurrentPlayer.Rank / 50) + 1;
            var scoreSaberClient = new scoresaberapi.scoresaberapi(httpClient);
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
                    PlayerCollection playerscores = await scoreSaberClient.PlayersAsync(null, fetchIndexPage, null, true);
                    lsPlayerRankings.AddRange(playerscores.Players);
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
    }
}
