using scoresaberapi;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IPALogger = IPA.Logging.Logger;

namespace PPPredictor.Utilities
{
    class PPCalculator
    {
        static readonly HttpClient httpClient = new HttpClient();
        static double basePPMultiplier = 42.117208413;
        private static double[,] arrPPCurve = new double[19, 2] {
            {1 ,  1.5},
            {0.99 , 1.39 },
            {0.98 , 1.29 },
            {0.97 , 1.2 },
            {0.96 , 1.115 },
            {0.95 , 1.046 },
            {0.945 , 1.015 },
            {0.925 , 0.905 },
            {0.9 , 0.78 },
            {0.86 , 0.6 },
            {0.8 , 0.42 },
            {0.75 , 0.3 },
            {0.7 , 0.22 },
            {0.65 , 0.15 },
            {0.6 , 0.105 },
            {0.55 , 0.06 },
            {0.5 , 0.03 },
            {0.45 , 0.015 },
            {0 , 0 } };


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

        static public async Task getScores(Player player)
        {
            /*var scoreSaberClient = new scoresaberapi.scoresaberapi(httpClient);
            bool hasMoreData = true;
            while (hasMoreData)
            {
                int itemsPerPage = 25;
                PlayerScoreCollection Playerscore = await scoreSaberClient.Scores3Async(player.Id, itemsPerPage, Sort.Recent, 0, true);
                if(Playerscore.PlayerScores.Count() < itemsPerPage)
                {
                    hasMoreData = false;
                }
                Playerscore.PlayerScores.
            }*/
        }
    }
}
