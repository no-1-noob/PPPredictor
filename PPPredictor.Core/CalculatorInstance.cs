using PPPredictor.Core.API;
using PPPredictor.Core.Calculator;
using PPPredictor.Core.DataType;
using PPPredictor.Core.DataType.BeatSaberEncapsulation;
using PPPredictor.Core.DataType.LeaderBoard;
using PPPredictor.Core.DataType.MapPool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static PPPredictor.Core.DataType.Enums;

namespace PPPredictor.Core
{
    public class CalculatorInstance
    {
        Dictionary<Leaderboard, PPCalculator> dctCalculator = new Dictionary<Leaderboard, PPCalculator> ();
        public event EventHandler<LoggingMessage> OnMessage;
        private readonly Settings settings;

        public CalculatorInstance(Settings settings)
        {
            this.settings = settings;
            Logging.OnMessage += (sender, message) => OnMessage?.Invoke(this, message);
        }

        public static async Task<CalculatorInstance> CreateAsync(Settings settings, Dictionary<string, LeaderboardData> dctLeaderboardData, Func<PPPBeatMapInfo, PPPBeatMapInfo> scoreSaberLookUpFunction)
        {
            var instance = new CalculatorInstance(settings);
            await instance.InitializeAsync(settings, dctLeaderboardData, scoreSaberLookUpFunction);
            return instance;
        }

        private async Task InitializeAsync(Settings settings, Dictionary<string, LeaderboardData> dctLeaderboardData, Func<PPPBeatMapInfo, PPPBeatMapInfo> scoreSaberLookUpFunction)
        {

            if (settings.IsScoreSaberEnabled)
            {
                if (scoreSaberLookUpFunction == null) throw new Exception("ScoreSaberLookUpFunction is missing");
                var v = new PPCalculatorScoreSaber<ScoresaberAPI>(dctLeaderboardData.TryGetValue(Leaderboard.ScoreSaber.ToString(), out var result) ? result.DctMapPool : null, settings, scoreSaberLookUpFunction);
                dctCalculator.Add(Leaderboard.ScoreSaber, v);
            }
            if(settings.IsBeatLeaderEnabled)
            {
                var v = new PPCalculatorBeatLeader<BeatleaderAPI>(dctLeaderboardData.TryGetValue(Leaderboard.BeatLeader.ToString(), out var result) ? result.DctMapPool : null, settings);
                dctCalculator.Add(Leaderboard.BeatLeader, v);
            }
            if (settings.IsHitbloqEnabled)
            {
                var v = new PPCalculatorHitBloq<HitbloqAPI>(dctLeaderboardData.TryGetValue(Leaderboard.HitBloq.ToString(), out var result) ? result.DctMapPool : null, settings);
                dctCalculator.Add(Leaderboard.HitBloq, v);
            }
            if (settings.IsAccSaberEnabled)
            {
                var v = new PPCalculatorAccSaber<AccSaberApi>(dctLeaderboardData.TryGetValue(Leaderboard.AccSaber.ToString(), out var result) ? result.DctMapPool : null, settings);
                dctCalculator.Add(Leaderboard.AccSaber, v);
            }
            if(dctCalculator.Count == 0)
            {
                var v = new PPCalculatorNoLeaderboard(null, settings);
                dctCalculator.Add(Leaderboard.AccSaber, v);
            }

            var updateAvailableMapPoolsTask = dctCalculator.Values.Select(item => item.UpdateAvailableMapPools());
            await Task.WhenAll(updateAvailableMapPoolsTask);
        }

        public void AssignSettings(Settings settings)
        {
            foreach (var item in dctCalculator.Values)
            {
                item.Settings = settings;
            }
        }

        private PPCalculator GetCalculator(Leaderboard leaderboard)
        {
            if(dctCalculator.TryGetValue(leaderboard, out PPCalculator calculator)){
                return calculator;
            }
            else
            {
                throw new Exception($"PPCalculator not found for {leaderboard}");
            }
        }

        public double CalculatePPatPercentage(Leaderboard leaderBoard, string mapPoolId, PPPBeatMapInfo _currentBeatMapInfo, double percentage, bool failed, bool paused)
        {
            (PPCalculator calculator, PPPMapPool mapPool) = GetCalculatorAndMapPool(leaderBoard, mapPoolId);
            return calculator.CalculatePPatPercentage(_currentBeatMapInfo, mapPool, percentage, failed, paused);
        }

        public PPPBeatMapInfo ApplyModifiersToBeatmapInfo(Leaderboard leaderBoard, string mapPoolId, PPPBeatMapInfo beatMapInfo, GameplayModifiers gameplayModifiers, bool levelFailed = false, bool levelPaused = false)
        {
            (PPCalculator calculator, PPPMapPool mapPool) = GetCalculatorAndMapPool(leaderBoard, mapPoolId);
            return calculator.ApplyModifiersToBeatmapInfo(beatMapInfo, mapPool, gameplayModifiers, levelFailed, levelPaused);
        }

        public double CalculateMaxPP(Leaderboard leaderBoard, string mapPoolId, PPPBeatMapInfo currentBeatMapInfo)
        {
            try
            {
                (PPCalculator calculator, PPPMapPool mapPool) = GetCalculatorAndMapPool(leaderBoard, mapPoolId);
                return calculator.CalculateMaxPP(currentBeatMapInfo, mapPool);
            }
            catch (Exception ex)
            {

                throw new Exception($"Error in CalculateMaxPP {ex.Message}");
            }
        }

        public async Task<RankGainResult> GetPlayerRankGain(Leaderboard leaderBoard, string mapPoolId, double pp)
        {
            (PPCalculator calculator, PPPMapPool mapPool) = GetCalculatorAndMapPool(leaderBoard, mapPoolId);
            var i = await SemaphoreFunction(calculator.Semaphore,
                () => Task.Run(() => calculator.GetPlayerRankGain(pp, mapPool))
            );
            return i;
        }

        public PPGainResult GetPlayerScorePPGain(Leaderboard leaderBoard, string mapPoolId, string mapSearchString, double pp)
        {
            (PPCalculator calculator, PPPMapPool mapPool) = GetCalculatorAndMapPool(leaderBoard, mapPoolId);
            return calculator.GetPlayerScorePPGain(mapSearchString, pp, mapPool);
        }

        public PPPMapPoolShort FindPoolWithSyncURL(Leaderboard leaderBoard, string syncUrl)
        {
            PPCalculator calculator = GetCalculator(leaderBoard);
            return calculator.FindPoolWithSyncURL(syncUrl);
        }

        public List<PPPMapPoolShort> GetMapPools(Leaderboard leaderBoard)
        {
            PPCalculator calculator = GetCalculator(leaderBoard);
            return calculator.GetMapPools();
        }
        


        public double? GetPersonalBest(Leaderboard leaderBoard, string mapPoolId, string mapSearchString)
        {
            (PPCalculator calculator, PPPMapPool mapPool) = GetCalculatorAndMapPool(leaderBoard, mapPoolId);
            return calculator.GetPersonalBest(mapSearchString, mapPool);
        }

        
        public async Task<(PPPPlayer, PPPPlayer)> UpdatePlayer(Leaderboard leaderBoard, string mapPoolId, bool doResetSession)
        {
            (PPCalculator calculator, PPPMapPool mapPool) = GetCalculatorAndMapPool(leaderBoard, mapPoolId);
            var i = await SemaphoreFunction(calculator.Semaphore,
                () => Task.Run(() => calculator.UpdatePlayer(mapPool, doResetSession))
            );
            return i;
        }

        public bool IsScoreSetOnCurrentMapPool(Leaderboard leaderBoard, string mapPoolId, PPPScoreSetData data)
        {
            (PPCalculator calculator, PPPMapPool mapPool) = GetCalculatorAndMapPool(leaderBoard, mapPoolId);
            return calculator.IsScoreSetOnCurrentMapPool(mapPool, data);
        }

        public async Task<PPPBeatMapInfo> GetBeatMapInfoAsync(Leaderboard leaderBoard, string mapPoolId, PPPBeatMapInfo beatMapInfo)
        {
            (PPCalculator calculator, PPPMapPool mapPool) = GetCalculatorAndMapPool(leaderBoard, mapPoolId);
            var i = await SemaphoreFunction(calculator.Semaphore,
                () => Task.Run(() => calculator.GetBeatMapInfoAsync(beatMapInfo, mapPool))
            );
            return i;
        }

        public async Task UpdateMapPoolDetails(Leaderboard leaderBoard, string mapPoolId)
        {
            (PPCalculator calculator, PPPMapPool mapPool) = GetCalculatorAndMapPool(leaderBoard, mapPoolId);
            await SemaphoreFunction(calculator.Semaphore,
                () => Task.Run(() => calculator.UpdateMapPoolDetails(mapPool))
            );
        }

        public async Task GetPlayerScores(Leaderboard leaderBoard, string mapPoolId, int pageSize, int largePageSize, bool fetchOnePage = false)
        {
            (PPCalculator calculator, PPPMapPool mapPool) = GetCalculatorAndMapPool(leaderBoard, mapPoolId);
            await SemaphoreFunction(calculator.Semaphore,
                () => Task.Run(() => calculator.GetPlayerScores(mapPool, pageSize, largePageSize, fetchOnePage))
            );
        }

        public Dictionary<string, LeaderboardData> GetSaveData()
        {
            Dictionary<string, LeaderboardData> dctLeaderboardData = new Dictionary<string, LeaderboardData>();
            foreach (var calculator in dctCalculator.Values)
            {
                LeaderboardData data = new LeaderboardData()
                {
                    DctMapPool = calculator._dctMapPool
                };
            dctLeaderboardData.Add(calculator._leaderboardInfo.LeaderboardName, data);
            }
            return dctLeaderboardData;
        }

        private (PPCalculator, PPPMapPool) GetCalculatorAndMapPool(Leaderboard leaderBoard, string mapPoolId)
        {
            var calculator = GetCalculator(leaderBoard);
            var mapPool = calculator.GetMapPoolById(mapPoolId);
            return (calculator, mapPool);
        }

        private async Task<T> SemaphoreFunction<T>(SemaphoreSlim sem, Func<Task<T>> function)
        {
            await sem.WaitAsync(); // Wait for access
            try
            {
                return await function();
            }
            finally
            {
                sem.Release(); // Ensure the semaphore is released
            }
        }

        private async Task SemaphoreFunction(SemaphoreSlim sem, Func<Task> function)
        {
            await sem.WaitAsync(); // Wait for access
            try
            {
                await function();
            }
            finally
            {
                sem.Release(); // Ensure the semaphore is released
            }
        }
    }
}
