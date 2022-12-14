using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using PPPredictor.Data;
using scoresaberapi;
using SongCore.Utilities;
using SongDetailsCache;
using SongDetailsCache.Structs;

namespace PPPredictor.Utilities
{
    public class PPCalculatorScoreSaber : PPCalculator
    {
        private readonly double basePPMultiplier = 42.117208413;
        private readonly HttpClient httpClient = new HttpClient();
        private readonly scoresaberapi.scoresaberapi scoreSaberClient;
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
        private SongDetails SongDetails { get; }
        public PPCalculatorScoreSaber() : base()
        {
            scoreSaberClient = new scoresaberapi.scoresaberapi(httpClient);
            SongDetails = SongDetails.Init().Result;
        }

        protected override async Task<PPPPlayer> GetPlayerInfo(long userId)
        {
            try
            {
                var playerInfo = scoreSaberClient.BasicAsync(userId);
                var scoreSaberPlayer = await playerInfo;
                PPPPlayer player = new PPPPlayer(scoreSaberPlayer);
                return player;
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPCalculatorScoreSaber GetPlayerInfo Error: {ex.Message}");
                return new PPPPlayer(true);
            }
        }

        protected override async Task<PPPScoreCollection> GetRecentScores(string userId, int pageSize, int page)
        {
            try
            {
                PlayerScoreCollection scoreSaberCollection = await scoreSaberClient.Scores3Async(userId, pageSize, Sort.Recent, page, true);
                return new PPPScoreCollection(scoreSaberCollection);
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPCalculatorScoreSaber GetRecentScores Error: {ex.Message}");
                return new PPPScoreCollection();
            }
        }

        protected override async Task<List<PPPPlayer>> GetPlayers(double fetchIndexPage)
        {
            try
            {
                List<PPPPlayer> lsPlayer = new List<PPPPlayer>();
                PlayerCollection scoreSaberPlayerCollection = await scoreSaberClient.PlayersAsync(null, fetchIndexPage, null, true);
                foreach (var scoreSaberPlayer in scoreSaberPlayerCollection.Players)
                {
                    lsPlayer.Add(new PPPPlayer(scoreSaberPlayer));
                }
                return lsPlayer;
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPCalculatorScoreSaber GetPlayers Error: {ex.Message}");
                return new List<PPPPlayer>();
            }
        }

        private double CalculateMultiplierAtPercentage(double percentage)
        {
            try
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
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPCalculatorScoreSaber CalculateMultiplierAtPercentage Error: {ex.Message}");
                return -1;
            }
        }

        private double CalculateMultiplierAtPercentageWithLine((double x, double y) p1, (double x, double y) p2, double percentage)
        {
            try
            {
                double m = (p2.y - p1.y) / (p2.x - p1.x);
                double b = p1.y - (m * p1.x);
                return m * percentage + b;
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPCalculatorScoreSaber CalculateMultiplierAtPercentageWithLine Error: {ex.Message}");
                return -1;
            }
        }

        public override double CalculatePPatPercentage(double star, double percentage, bool levelFailed)
        {
            try
            {
                percentage /= 100.0;
                if (levelFailed) percentage /= 2.0; //Halve score if nofail triggered
                double multiplier = CalculateMultiplierAtPercentage(percentage);
                return multiplier * star * basePPMultiplier;
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPCalculatorScoreSaber CalculatePPatPercentage Error: {ex.Message}");
                return -1;
            }
        }

        public override Task<double> GetStarsForBeatmapAsync(LevelSelectionNavigationController lvlSelectionNavigationCtrl, IDifficultyBeatmap beatmap)
        {
            try
            {
                if (lvlSelectionNavigationCtrl.selectedBeatmapLevel is CustomBeatmapLevel selectedCustomBeatmapLevel)
                {
                    if (SongDetails.songs.FindByHash(Hashing.GetCustomLevelHash(selectedCustomBeatmapLevel), out Song song))
                    {
                        if (song.GetDifficulty(out SongDifficulty songDiff, (MapDifficulty)beatmap.difficulty))
                        {
                            return Task.FromResult((double)songDiff.stars);
                        }
                    }
                    return Task.FromResult(0d);
                }
                return Task.FromResult(0d);
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"PPCalculatorScoreSaber GetStarsForBeatmapAsync Error: {ex.Message}");
                return Task.FromResult(-1d);
            }
        }

        public override double ApplyModifierMultiplierToStars(double baseStars, GameplayModifiers gameplayModifiers, bool levelFailed)
        {
            return baseStars;
        }
    }
}
