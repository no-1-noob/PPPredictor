using PPPredictor.Core.API;
using PPPredictor.Core.Calculator;
using PPPredictor.Core.DataType;
using PPPredictor.Core.DataType.BeatSaberEncapsulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static PPPredictor.Core.DataType.Enums;

namespace PPPredictor.Core
{
    public class Instance
    {
        Dictionary<Leaderboard, PPCalculator> dctCalculator = new Dictionary<Leaderboard, PPCalculator> ();
        public Logging logging = new Logging();
        private readonly Settings settings;

        public Instance(Settings settings)
        {
            this.settings = settings;
        }

        public static async Task<Instance> CreateAsync(Settings settings)
        {
            var instance = new Instance(settings);
            await instance.InitializeAsync(settings);
            return instance;
        }

        private async Task InitializeAsync(Settings settings)
        {

            if (settings.IsScoreSaberEnabled)
            {
#warning todo;
            }
            if(settings.IsBeatLeaderEnabled)
            {
                var v = new PPCalculatorBeatLeader<BeatleaderAPI>();
                dctCalculator.Add(Leaderboard.BeatLeader, v);
            }
            if (settings.IsHitbloqEnabled)
            {
                var v = new PPCalculatorHitBloq<HitbloqAPI>();
                dctCalculator.Add(Leaderboard.HitBloq, v);
            }
            if (settings.IsAccSaberEnabled)
            {
                var v = new PPCalculatorAccSaber<AccSaberApi>();
                dctCalculator.Add(Leaderboard.AccSaber, v);
            }
            if(dctCalculator.Count == 0)
            {
                var v = new PPCalculatorNoLeaderboard();
                dctCalculator.Add(Leaderboard.AccSaber, v);
            }

            var updateAvailableMapPoolsTask = dctCalculator.Values.Select(item => item.UpdateAvailableMapPools());
            await Task.WhenAll(updateAvailableMapPoolsTask);
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

        public async Task<double> CalculatePPatPercentage(Leaderboard leaderBoard, string mapPoolId, PPPBeatMapInfo _currentBeatMapInfo, double percentage, bool failed, bool paused)
        {
            (PPCalculator calculator, PPPMapPool mapPool) = GetCalculatorAndMapPool(leaderBoard, mapPoolId);
            var i = await SemaphoreFunction<double>(calculator.Semaphore,
                () => Task.Run(() => calculator.CalculatePPatPercentage(_currentBeatMapInfo, mapPool, percentage, failed, paused))
            );
            return i;
        }

        public async Task<PPPBeatMapInfo> ApplyModifiersToBeatmapInfo(Leaderboard leaderBoard, string mapPoolId, PPPBeatMapInfo beatMapInfo, GameplayModifiers gameplayModifiers, bool levelFailed = false, bool levelPaused = false)
        {
            (PPCalculator calculator, PPPMapPool mapPool) = GetCalculatorAndMapPool(leaderBoard, mapPoolId);
            var i = await SemaphoreFunction(calculator.Semaphore,
                () => Task.Run(() => calculator.ApplyModifiersToBeatmapInfo(beatMapInfo, mapPool, gameplayModifiers, levelFailed, levelPaused))
            );
            return i;
        }

        public async Task<double> CalculateMaxPP(Leaderboard leaderBoard, string mapPoolId, PPPBeatMapInfo currentBeatMapInfo)
        {
            (PPCalculator calculator, PPPMapPool mapPool) = GetCalculatorAndMapPool(leaderBoard, mapPoolId);
            var i = await SemaphoreFunction(calculator.Semaphore,
                () => Task.Run(() => calculator.CalculateMaxPP(currentBeatMapInfo, mapPool))
            );
            return i;
        }

        public async Task<RankGainResult> GetPlayerRankGain(Leaderboard leaderBoard, string mapPoolId, double pp)
        {
            (PPCalculator calculator, PPPMapPool mapPool) = GetCalculatorAndMapPool(leaderBoard, mapPoolId);
            var i = await SemaphoreFunction(calculator.Semaphore,
                () => Task.Run(() => calculator.GetPlayerRankGain(pp, mapPool))
            );
            return i;
        }

        public async Task<PPGainResult> GetPlayerScorePPGain(Leaderboard leaderBoard, string mapPoolId, string mapSearchString, double pp)
        {
            (PPCalculator calculator, PPPMapPool mapPool) = GetCalculatorAndMapPool(leaderBoard, mapPoolId);
            var i = await SemaphoreFunction(calculator.Semaphore,
                () => Task.Run(() => calculator.GetPlayerScorePPGain(mapSearchString, pp, mapPool))
            );
            return i;
        }

        public async Task<PPPMapPoolShort> FindPoolWithSyncURL(Leaderboard leaderBoard, string syncUrl)
        {
            PPCalculator calculator = GetCalculator(leaderBoard);
            var i = await SemaphoreFunction(calculator.Semaphore,
                () => Task.Run(() => calculator.FindPoolWithSyncURL(syncUrl))
            );
            return i;
        }

        public List<PPPMapPoolShort> GetMapPools(Leaderboard leaderBoard)
        {
            PPCalculator calculator = GetCalculator(leaderBoard);
            return calculator.GetMapPools();
        }
        


        public async Task<double?> GetPersonalBest(Leaderboard leaderBoard, string mapPoolId, string mapSearchString)
        {
            (PPCalculator calculator, PPPMapPool mapPool) = GetCalculatorAndMapPool(leaderBoard, mapPoolId);
            var i = await SemaphoreFunction(calculator.Semaphore,
                () => Task.Run(() => calculator.GetPersonalBest(mapSearchString, mapPool))
            );
            return i;
        }

        public async Task<PPPPlayer> GetProfile(Leaderboard leaderBoard, string mapPoolId)
        {
            (PPCalculator calculator, PPPMapPool mapPool) = GetCalculatorAndMapPool(leaderBoard, mapPoolId);
            var i = await SemaphoreFunction(calculator.Semaphore,
                () => Task.Run(() => calculator.GetProfile(mapPool, settings.UserId))
            );
            return i;
        }

        public async Task<bool> HasOldDotRanking(Leaderboard leaderBoard, string mapPoolId)
        {
            (PPCalculator calculator, PPPMapPool mapPool) = GetCalculatorAndMapPool(leaderBoard, mapPoolId);
            var i = await SemaphoreFunction(calculator.Semaphore,
                () => Task.Run(() => calculator.hasOldDotRanking)
            );
            return i;
        }

        public async Task<bool> IsScoreSetOnCurrentMapPool(Leaderboard leaderBoard, string mapPoolId, PPPScoreSetData data)
        {
            (PPCalculator calculator, PPPMapPool mapPool) = GetCalculatorAndMapPool(leaderBoard, mapPoolId);
            var i = await SemaphoreFunction(calculator.Semaphore,
                () => Task.Run(() => calculator.IsScoreSetOnCurrentMapPool(mapPool, data))
            );
            return i;
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
                () => Task.Run(() => calculator.GetPlayerScores(mapPool, settings.UserId, pageSize, largePageSize, fetchOnePage))
            );
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
