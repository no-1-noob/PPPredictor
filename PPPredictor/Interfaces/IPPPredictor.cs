using PPPredictor.Data;
using PPPredictor.Data.DisplayInfos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace PPPredictor.Interfaces
{
    interface IPPPredictor
    {
        void ChangeGameplayModifiers(GameplaySetupViewController gameplaySetupViewController);
        void DetailContentChanged(LevelSelectionNavigationController lvlSelectionNavigationCtrl, StandardLevelDetailViewController.ContentType contentType);
        void DifficultyChanged(LevelSelectionNavigationController lvlSelectionNavigationCtrl, IDifficultyBeatmap beatmap);
        Task UpdateCurrentAndCheckResetSession(bool doResetSession);
        void RefreshCurrentData(int fetchLength, bool refreshStars = false);
        void ResetDisplay(bool resetAll);
        double CalculatePPatPercentage(double percentage, PPPBeatMapInfo beatMapInfo, bool levelFailed = false);
        double CalculateMaxPP();
        PPPBeatMapInfo GetModifiedBeatMapInfo(GameplayModifiers gameplayModifiers, bool levelFailed = false);
        double CalculatePPGain(double pp);
        bool IsRanked();

        string GetPersonalBest();
        void CalculatePP();
        void SetActive(bool setActive);
        PPPMapPool FindPoolWithSyncURL(string syncUrl);
        Task GetMapPoolIconData();
        Task UpdateCurrentBeatMapInfos(CustomBeatmapLevel selectedBeatmapLevel, IDifficultyBeatmap beatmap);
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
