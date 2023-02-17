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
        void RefreshCurrentData(int fetchLength);
        void ResetDisplay(bool resetAll);
        double CalculatePPatPercentage(double percentage, bool levelFailed = false);
        double CalculateMaxPP();
        double CalculatePPGain(double pp);
        bool IsRanked();
        void CalculatePP();
        void SetActive(bool setActive);
        event EventHandler<bool> OnDataLoading;
        event EventHandler<DisplaySessionInfo> OnDisplaySessionInfo;
        event EventHandler<DisplayPPInfo> OnDisplayPPInfo;
        event EventHandler OnMapPoolRefreshed;
        float Percentage { get; set; }
        string LeaderBoardName { get; }
        List<object> MapPoolOptions { get; }
        object CurrentMapPool { get; set; }
        string PPSuffix { get; }
    }
}
