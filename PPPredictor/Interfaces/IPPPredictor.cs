using System.ComponentModel;
using System.Threading.Tasks;

namespace PPPredictor.Interfaces
{
    interface IPPPredictor
    {
        void ChangeGameplayModifiers(GameplaySetupViewController gameplaySetupViewController);
        void DetailContentChanged(LevelSelectionNavigationController lvlSelectionNavigationCtrl, StandardLevelDetailViewController.ContentType contentType);
        void DifficultyChanged(LevelSelectionNavigationController lvlSelectionNavigationCtrl, IDifficultyBeatmap beatmap);
        void SetPropertyChangedEventHandler(PropertyChangedEventHandler propertyChanged);
        Task UpdateCurrentAndCheckResetSession(bool doResetSession);
        void RefreshCurrentData(int fetchLength);
        void ResetDisplay(bool resetAll);
        double CalculatePPatPercentage(double percentage);
        double CalculatePPGain(double pp);
        bool IsRanked();
        void DisplayPP();
        void SetActive(bool setActive);
        #region Properties
        float Percentage { get; set; }
        string PPGainRaw { get; set; }
        string PPGainWeighted { get; set; }
        string PPGainDiffColor { get; set; }
        string SessionRank { get; set; }
        string SessionRankDiff { get; set; }
        string SessionRankDiffColor { get; set; }
        string SessionCountryRank { get; set; }
        string SessionCountryRankDiff { get; set; }
        string SessionCountryRankDiffColor { get; set; }
        string SessionPP { get; set; }
        string SessionPPDiff { get; set; }
        string SessionPPDiffColor { get; set; }
        string PredictedRank { get; set; }
        string PredictedRankDiff { get; set; }
        string PredictedRankDiffColor { get; set; }
        string PredictedCountryRank { get; set; }
        string PredictedCountryRankDiff { get; set; }
        string PredictedCountryRankDiffColor { get; set; }
        bool IsDataLoading { get; }
        bool IsNoDataLoading { get; }
        string LeaderBoardName { get; }
        bool IsUserFound { get; }
        bool IsNoUserFound { get; }


        #endregion
    }
}
