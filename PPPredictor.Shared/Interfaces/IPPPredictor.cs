using PPPredictor.Core.DataType;
using PPPredictor.Core.DataType.BeatSaberEncapsulation;
using PPPredictor.Core.DataType.MapPool;
using PPPredictor.Data;
using PPPredictor.Data.DisplayInfos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PPPredictor.Shared.Interfaces
{
    public interface IPPPredictor
    {
        void ChangeGameplayModifiers(PPPredictor.Core.DataType.BeatSaberEncapsulation.GameplayModifiers gameplayModifiers);
        void DifficultyChanged(PPPBeatMapInfo beatMapInfo);
        Task UpdateCurrentAndCheckResetSession(bool doResetSession);
        void ScoreSet(PPPScoreSetData data);
        void RefreshCurrentData(int fetchLength, bool refreshStars = false, bool fetchOnePage = false);
        void ResetDisplay(bool resetAll);
        double CalculatePPatPercentage(double percentage, PPPBeatMapInfo beatMapInfo, bool levelFailed = false, bool levelPaused = false);
        double CalculateMaxPP();
        PPPBeatMapInfo GetModifiedBeatMapInfo(PPPredictor.Core.DataType.BeatSaberEncapsulation.GameplayModifiers gameplayModifiers, bool levelFailed = false, bool levelPaused = false);
        double CalculatePPGain(double pp);
        bool IsRanked();

        double? GetPersonalBest();
        void CalculatePP();
        void SetActive(bool setActive, bool hasPoolChanged = false);
        PPPMapPoolShort FindPoolWithSyncURL(string syncUrl);
        Task GetMapPoolIconData();
        Task UpdateCurrentBeatMapInfos(PPPBeatMapInfo beatMapInfo);
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
