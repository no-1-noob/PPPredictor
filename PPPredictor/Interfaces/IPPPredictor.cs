using PPPredictor.Core.DataType;
using PPPredictor.Core.DataType.BeatSaberEncapsulation;
using PPPredictor.Core.DataType.MapPool;
using PPPredictor.Data;
using PPPredictor.Data.DisplayInfos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PPPredictor.Interfaces
{
    interface IPPPredictor
    {
        Task ChangeGameplayModifiers(GameplaySetupViewController gameplaySetupViewController);
        void DifficultyChanged(BeatmapLevel selectedBeatmapLevel, BeatmapKey beatmapKey);
        Task UpdateCurrentAndCheckResetSession(bool doResetSession);
#warning ScoreSet needed?
        Task ScoreSet(PPPScoreSetData data);
        void RefreshCurrentData(int fetchLength, bool refreshStars = false, bool fetchOnePage = false);
        void ResetDisplay(bool resetAll);
        double CalculatePPatPercentage(double percentage, PPPBeatMapInfo beatMapInfo, bool levelFailed = false, bool levelPaused = false);
        double CalculateMaxPP();
        PPPBeatMapInfo GetModifiedBeatMapInfo(GameplayModifiers gameplayModifiers, bool levelFailed = false, bool levelPaused = false);
        double CalculatePPGain(double pp);
        bool IsRanked();

        double? GetPersonalBest();
        void CalculatePP();
        void SetActive(bool setActive);
        Task<PPPMapPoolShort> FindPoolWithSyncURL(string syncUrl);
        Task GetMapPoolIconData();
        Task UpdateCurrentBeatMapInfos(BeatmapLevel selectedBeatmapLevel, BeatmapKey beatmapKey);
        event EventHandler<bool> OnDataLoading;
        event EventHandler<DisplaySessionInfo> OnDisplaySessionInfo;
        event EventHandler<DisplayPPInfo> OnDisplayPPInfo;
        event EventHandler OnMapPoolRefreshed;
        float Percentage { get; set; }
        string LeaderBoardName { get; }
        string LeaderBoardIcon { get; }
        string MapPoolIcon { get; }
        byte[] MapPoolIconData { get; set; }
        List<object> MapPoolOptions { get; }
        object CurrentMapPool { get; set; }
        string PPSuffix { get; }
    }
}
