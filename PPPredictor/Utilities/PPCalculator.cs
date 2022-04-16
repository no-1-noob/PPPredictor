using scoresaberapi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IPALogger = IPA.Logging.Logger;

namespace PPPredictor.Utilities
{
    class PPCalculator
    {
        static readonly HttpClient httpClient = new HttpClient();
        static double basePPMultiplier = 42.117208413;
        private static double[,] arrPPCurve = new double[32, 2] {
            {1, 7},
            {0.999, 5.8},
            {0.9975, 4.7},
            {0.995, 3.76},
            {0.9925, 3.17},
            {0.99, 2.73},
            {0.9875, 2.38},
            {0.985, 2.1},
            {0.9825, 1.88},
            {0.98, 1.71},
            {0.9775, 1.57},
            {0.975, 1.45},
            {0.9725, 1.37},
            {0.97, 1.31},
            {0.965, 1.20},
            {0.96, 1.11},
            {0.955, 1.045},
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


        static public async Task<double> calculateBasePPForBeatmapAsync(IDifficultyBeatmap beatmap)
        {
            try
            {
                if (beatmap.level.levelID.StartsWith("custom_level_"))
                {
                    string hash = beatmap.level.levelID.Replace("custom_level_", "");
                    double basePP = Plugin.ProfileInfo.findBasePP(hash, beatmap.difficultyRank);
                    if (basePP < 0)
                    {
                        Plugin.Log?.Info("PP Not found in dict");
                        var scoreSaberClient = new scoresaberapi.scoresaberapi(httpClient);
                        Task<LeaderboardInfo> downloadingMapInfo = scoreSaberClient.Info2Async(hash, beatmap.difficultyRank, null);
                        var mapInfo = await downloadingMapInfo;
                        Task<ScoreCollection> downloadingLeaderboard = scoreSaberClient.Scores2Async(hash, beatmap.difficultyRank, null, null, null, null, true);
                        var scoreCollection = await downloadingLeaderboard;
                        if (scoreCollection.Scores.Count() > 0)
                        {
                            double topScorePercentage = scoreCollection.Scores.ElementAt(0).ModifiedScore / mapInfo.MaxScore;
                            double multiplierAtTopScore = calculateMultiplierAtPercentage(topScorePercentage);
                            double calculateStarRating = scoreCollection.Scores.ElementAt(0).Pp / multiplierAtTopScore / basePPMultiplier;
                            mapInfo.Stars = calculateStarRating;
                        }
                        basePP = mapInfo.Stars * basePPMultiplier;
                        Plugin.ProfileInfo.addDictBasePP(hash, beatmap.difficultyRank, basePP);
                    }
                    else
                    {
                        Plugin.Log?.Info("PP found in dict");
                    }
                    return basePP;
                }
                return 0;
            }
            catch (System.Exception)
            {
                return -1;
            }
        }

        static public double calculatePPatPercentage(double basePP, double percentage)
        {
            percentage = percentage / 100.0;
            double multiplier = calculateMultiplierAtPercentage(percentage);
            return multiplier * basePP;
        }

        static private double calculateMultiplierAtPercentage(double percentage)
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
                        return calculateMultiplierAtPercentageWithLine((arrPPCurve[i + 1, 0], arrPPCurve[i + 1, 1]), (arrPPCurve[i, 0], arrPPCurve[i, 1]), percentage);
                    }
                }
            }
            return 0;
        }

        static private double calculateMultiplierAtPercentageWithLine((double x, double y) p1, (double x, double y) p2, double percentage)
        {
            double m = (p2.y - p1.y) / (p2.x - p1.x);
            double b = p1.y - (m * p1.x);
            return m * percentage + b;
        }

        static public async Task<Player> getProfile(string userId)
        {
            long longUserId = 0;
            if(long.TryParse(userId, out longUserId))
            {
                Plugin.Log?.Info(longUserId.ToString());
                var scoreSaberClient = new scoresaberapi.scoresaberapi(httpClient);
                Task<Player> playerInfo = scoreSaberClient.BasicAsync(longUserId);
                var player = await playerInfo;
                return player;
            }
            return new Player();
        }

        static public async Task getPlayerScores(string userId, int pageSize)
        {
            try
            {
                var scoreSaberClient = new scoresaberapi.scoresaberapi(httpClient);
                bool hasMoreData = true;
                int page = 1;
                List<ShortScore> lsNewScores = new List<ShortScore>();
                while (hasMoreData)
                {
                    Plugin.Log?.Info($"getPlayerScores page {page}");
                    PlayerScoreCollection playerscores = await scoreSaberClient.Scores3Async(userId, pageSize, Sort.Recent, page, true);
                    if (playerscores.Metadata.Page * playerscores.Metadata.ItemsPerPage >= playerscores.Metadata.Total)
                    {
                        hasMoreData = false;
                    }
                    if (Plugin.ProfileInfo.LSScores == null) Plugin.ProfileInfo.LSScores = new List<ShortScore>();
                    foreach (PlayerScore scores in playerscores.PlayerScores)
                    {
                        string searchString = createSeachString(scores.Leaderboard.SongHash, (int)scores.Leaderboard.Difficulty.Difficulty1);
                        ShortScore previousScore = Plugin.ProfileInfo.LSScores.Find(x => x.Searchstring == searchString);
                        ShortScore newScore = new ShortScore(searchString, scores.Score.TimeSet, scores.Leaderboard.MaxScore, scores.Score.ModifiedScore, scores.Score.Pp);
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
                    Thread.Sleep(250);
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
                            previousScore.ModifiedScore = newScore.ModifiedScore;
                    }
                };
                Plugin.ProfileInfo.LSScores.Sort();
            }
            catch (System.Exception ex)
            {
                Plugin.Log?.Error($"Error in PPPredictor: getPlayerScores: {ex.Message}");
            }
            
        }

        static public double getPlayerScorePPGain(string mapSearchString, double pp)
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
                    if (score.Searchstring == mapSearchString) //skip older (lower) score
                    {
                        if (!newPPadded)
                        {
                            ppAfterPlay += weightPP(score.Pp, index);
                            newPPSkiped = true;
                            index++;
                        }
                        continue;
                    }
                    double weightedPP = weightPP(score.Pp, index);
                    if (!newPPadded && !newPPSkiped)
                    {
                        double weightedNewPP = weightPP(pp, index);
                        if (weightedNewPP >= weightedPP) //add new (potential) pp
                        {
                            ppAfterPlay += weightedNewPP;
                            newPPadded = true;
                            index++;
                            weightedPP = weightPP(score.Pp, index);
                        }
                    }
                    ppAfterPlay += weightedPP;
                    index++;
                }
                return ppAfterPlay - Plugin.ProfileInfo.CurrentPlayer.Pp;
            }
            return pp;
        }

        static public double weightPP(double rawPP, int index)
        {
            return rawPP * Math.Pow(0.965, (index - 1));
        }

        public static string createSeachString(string hash, int difficulty)
        {
            return $"{hash}_{difficulty}";
        }
    }
}
