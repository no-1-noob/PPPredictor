using PPPredictor.Core.DataType;
using PPPredictor.Core.DataType.MapPool;
using PPPredictor.Data.DisplayInfos;
using PPPredictor.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PPPredictor.Utilities
{
    internal class DummPredictor : IPPPredictor
    {
        public float Percentage { get => 0; set { return; } }

        public string LeaderBoardName => string.Empty;

        public string LeaderBoardIcon => "PPPredictor.Resources.LeaderBoardLogos.ScoreSaber.png";

        public string MapPoolIcon => string.Empty;

        public byte[] MapPoolIconData { get => throw new NotImplementedException(); set { return; } }

        public List<object> MapPoolOptions => new List<object>();

        public object CurrentMapPool { get => new PPPMapPoolShort(); set { return; } }

        public string PPSuffix => string.Empty;

#pragma warning disable CS0067
        public event EventHandler<bool> OnDataLoading;
        public event EventHandler<DisplaySessionInfo> OnDisplaySessionInfo;
        public event EventHandler<DisplayPPInfo> OnDisplayPPInfo;
        public event EventHandler OnMapPoolRefreshed;
#pragma warning restore CS0067

        public double CalculateMaxPP()
        {
            return 0D;
        }

        public void CalculatePP()
        {
            return;
        }

        public double CalculatePPatPercentage(double percentage, PPPBeatMapInfo beatMapInfo, bool levelFailed = false, bool levelPaused = false)
        {
            return 0D;
        }

        public double CalculatePPGain(double pp)
        {
            return 0D;
        }

        public void ChangeGameplayModifiers(GameplaySetupViewController gameplaySetupViewController)
        {
            return;
        }

        public void DifficultyChanged(BeatmapLevel selectedBeatmapLevel, BeatmapKey beatmapKey)
        {
            return;
        }

        public Task<PPPMapPoolShort> FindPoolWithSyncURL(string syncUrl)
        {
            return Task.FromResult(new PPPMapPoolShort());
        }

        public Task GetMapPoolIconData()
        {
            return Task.CompletedTask;
        }

        public PPPBeatMapInfo GetModifiedBeatMapInfo(GameplayModifiers gameplayModifiers, bool levelFailed = false, bool levelPaused = false)
        {
            return new PPPBeatMapInfo();
        }

        public double? GetPersonalBest()
        {
            double? d = 0;
            return d;
        }

        public bool IsRanked()
        {
            return false;
        }

        public void RefreshCurrentData(int fetchLength, bool refreshStars = false, bool fetchOnePage = false)
        {
            return;
        }

        public void ResetDisplay(bool resetAll)
        {
            return;
        }

        public void ScoreSet(PPPScoreSetData data)
        {
            return;
        }

        public void SetActive(bool setActive)
        {
            return;
        }

        public Task UpdateCurrentAndCheckResetSession(bool doResetSession)
        {
            return Task.CompletedTask;
        }

        public Task UpdateCurrentBeatMapInfos(BeatmapLevel selectedBeatmapLevel, BeatmapKey beatmapKey)
        {
            return Task.CompletedTask;
        }
    }
}
