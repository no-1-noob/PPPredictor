//using BeatSaberPlaylistsLib.Types;
using PPPredictor.Data.DisplayInfos;
using PPPredictor.Data;
using PPPredictor.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static PPPredictor.Core.DataType.Enums;
using PPPredictor.Core.DataType;
using BeatSaberPlaylistsLib.Types;

namespace PPPredictor.Interfaces
{
    interface IPPPredictorMgr
    {
        bool IsLeftArrowActive { get; }
        bool IsRightArrowActive { get; }
        bool IsMapPoolDropDownActive { get; }
        bool IsLeaderboardNavigationActive { get; }
        IPPPredictor CurrentPPPredictor { get; }

        WebSocketMgr WebsocketMgr { get; }

        event EventHandler<bool> ViewActivated;
        event EventHandler<bool> OnDataLoading;
        event EventHandler<DisplaySessionInfo> OnDisplaySessionInfo;
        event EventHandler<DisplayPPInfo> OnDisplayPPInfo;
        event EventHandler OnMapPoolRefreshed;

        void ResetPredictors();

        void CyclePredictors(int offset);

        void RestartOverlayServer();

        void ChangeGameplayModifiers(GameplaySetupViewController gameplaySetupViewController);

        void DifficultyChanged(BeatmapLevel selectedBeatmapLevel, BeatmapKey beatmapKey);

        void UpdateCurrentAndCheckResetSession(bool v);

        void ScoreSet(string leaderboardName, PPPScoreSetData data);

        void RefreshCurrentData(int v, bool refreshStars = false);

        void SetPercentage(float percentage);

        double GetPercentage();

        void ResetDisplay(bool resetAll);

        double GetPPAtPercentageForCalculator(Leaderboard leaderBoardName, double percentage, bool levelFailed, bool levelPaused, PPPBeatMapInfo beatMapInfo);

        double? GetPersonalBest(Leaderboard leaderBoardName);

        string GetPPSuffixForLeaderboard(Leaderboard leaderBoardName);

        double GetMaxPPForCalculator(Leaderboard leaderBoardName);

        PPPBeatMapInfo GetModifiedBeatMapInfo(Leaderboard leaderBoardName, GameplayModifiers gameplayModifiers);

        bool IsRanked(Leaderboard leaderBoardName);

        double GetPPGainForCalculator(Leaderboard leaderBoardName, double pp);

        string GetLeaderboardIcon(Leaderboard leaderBoardName);
        string GetMapPoolIcon(Leaderboard leaderBoardName);

        Task<byte[]> GetLeaderboardIconData(Leaderboard leaderBoardName);

        void ActivateView(bool activate);

        List<object> GetMapPoolsFromLeaderboard(Leaderboard leaderBoardName);

        Task UpdateCurrentBeatMapInfos(BeatmapLevel selectedBeatmapLevel, BeatmapKey beatmap);

        Task FindPoolWithSyncURL(IPlaylist playlist);
    }
}
