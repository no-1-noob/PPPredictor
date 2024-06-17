using Newtonsoft.Json;
using PPPredictor.Data;
using PPPredictor.Interfaces;
using System;
using System.Collections.Generic;
using Zenject;

namespace PPPredictor.Utilities
{
    internal class GamePlayMgr : IInitializable, IDisposable
    {
#pragma warning disable CS0649
        [Inject] private readonly ScoreController scoreController;
        [Inject] private readonly GameplayCoreSceneSetupData setupData;
        [InjectOptional] private readonly PauseController pauseController;
        [Inject] private readonly BeatmapObjectManager beatmapObjectManager;
        [Inject] private readonly IPPPredictorMgr ppPredictorMgr;
#pragma warning restore CS0649

        private int maxPossibleScore = 0;
        private bool _levelFailed = false;
        private bool _levelPause = false;
        private int _noteCount = 0;
        private int _noteDone = 0;
        private bool _isSongStarted = false;
        private bool _isSongFinished = false;
        private GamePlayInfo gamePlayInfo;

        public event EventHandler OnPaused;
        public event EventHandler OnResumed;
        public event EventHandler OnLastNoteHit;
        public event EventHandler OnFirstNoteHit;
        public event EventHandler OnSongStarted;
        public event EventHandler OnSongFinished;
        public event EventHandler<GamePlayInfo> OnGameplayInfoChanged;

        public void Initialize()
        {
            try
            {
                OverlayDataSubscribe();
                if (setupData.practiceSettings == null)
                {
                    SetupCounter();
                }
                OnSongStarted?.Invoke(this, null);
            }
            catch (Exception ex)
            {
                Plugin.ErrorPrint($"CounterInit Error: {ex.Message}");
            }
        }

        public void Dispose()
        {
            OnSongFinished?.Invoke(this, null);
            scoreController.scoreDidChangeEvent -= ScoreController_scoreDidChangeEvent;
            BS_Utils.Utilities.BSEvents.energyReachedZero -= BSEvents_energyReachedZero;
            if (pauseController != null)
            {
                pauseController.didPauseEvent -= PauseController_didPauseEvent;
                pauseController.didResumeEvent -= PauseController_didResumeEvent;
            }
            beatmapObjectManager.noteWasCutEvent -= BeatmapObjectManager_noteWasCutEvent;
            beatmapObjectManager.noteWasMissedEvent -= BeatmapObjectManager_noteWasMissedEvent;
            OverlayDataUnsubscribe();
        }

        private async void SetupCounter()
        {
            try
            {
                this.ppPredictorMgr.ChangeGameplayModifiers(setupData.gameplayModifiers);
                try
                {
                    await ppPredictorMgr.UpdateCurrentBeatMapInfos(setupData.previewBeatmapLevel as CustomPreviewBeatmapLevel, setupData.difficultyBeatmap);
                }
                catch (Exception ex)
                {
                    Plugin.ErrorPrint($"PPPCounter change selected Map: UpdateCurrentBeatMapInfos failed {ex.Message}");
                }
                if (pauseController != null)
                {
                    pauseController.didPauseEvent += PauseController_didPauseEvent;
                    pauseController.didResumeEvent += PauseController_didResumeEvent;
                }
                beatmapObjectManager.noteWasCutEvent += BeatmapObjectManager_noteWasCutEvent;
                beatmapObjectManager.noteWasMissedEvent += BeatmapObjectManager_noteWasMissedEvent;

                var v = await setupData.difficultyBeatmap.GetBeatmapDataBasicInfoAsync();
                _noteCount = v.cuttableNotesCount;

                gamePlayInfo = new GamePlayInfo();
                gamePlayInfo.scoreboardCount = GetActiveScoreboardsCount();
                if (ShowScoreSaber()) gamePlayInfo.lsInfo.Add(new LeaderBoardGameplayInfo(Leaderboard.ScoreSaber, ppPredictorMgr, setupData.gameplayModifiers));
                if (ShowBeatLeader()) gamePlayInfo.lsInfo.Add(new LeaderBoardGameplayInfo(Leaderboard.BeatLeader, ppPredictorMgr, setupData.gameplayModifiers));
                if (ShowHitBloq()) gamePlayInfo.lsInfo.Add(new LeaderBoardGameplayInfo(Leaderboard.HitBloq, ppPredictorMgr, setupData.gameplayModifiers));

                maxPossibleScore = ScoreModel.ComputeMaxMultipliedScoreForBeatmap(setupData.transformedBeatmapData);
                scoreController.scoreDidChangeEvent += ScoreController_scoreDidChangeEvent;
                BS_Utils.Utilities.BSEvents.energyReachedZero += BSEvents_energyReachedZero;
                CalculatePercentages();
            }
            catch (Exception ex)
            {
                Plugin.ErrorPrint($"SetupCounter Error: {ex.Message}");
            }
        }

        internal GamePlayInfo GetStatus()
        {
            return gamePlayInfo;
        }

        #region events
        private void BeatmapObjectManager_noteWasMissedEvent(NoteController noteController)
        {
            CheckNotesLeft(noteController);
        }

        private void BeatmapObjectManager_noteWasCutEvent(NoteController noteController, in NoteCutInfo noteCutInfo)
        {
            CheckNotesLeft(noteController);
        }

        private void CheckNotesLeft(NoteController noteController)
        {
            if (!_isSongFinished
                && noteController.noteData.gameplayType != NoteData.GameplayType.Bomb
                && noteController.noteData.gameplayType != NoteData.GameplayType.BurstSliderElementFill
                && noteController.noteData.gameplayType != NoteData.GameplayType.BurstSliderElement)
            {
                if(!_isSongStarted)
                {
                    OnFirstNoteHit?.Invoke(this, null);
                }
                _noteDone++;
                _isSongStarted = true;
                if (_noteDone >= _noteCount && Plugin.ProfileInfo.IsCounterGainSilentModeEnabled)
                {
                    _isSongFinished = true;
                    OnLastNoteHit?.Invoke(this, null);
                }
            }
        }

        private void BSEvents_energyReachedZero()
        {
            _levelFailed = true;
        }
        private void ScoreController_scoreDidChangeEvent(int arg1, int arg2)
        {
            CalculatePercentages();
        }

        private void PauseController_didPauseEvent()
        {
            if (!_levelPause && _isSongStarted && !_isSongFinished)
            {
                _levelPause = true;
                CalculatePercentages();
            }
            if (Plugin.ProfileInfo.IsCounterGainSilentModeEnabled && !_isSongFinished)
            {
                OnPaused?.Invoke(this, null);
            }
        }

        private void PauseController_didResumeEvent()
        {
            if (Plugin.ProfileInfo.IsCounterGainSilentModeEnabled && !_isSongFinished)
            {
                OnResumed?.Invoke(this, null);
            }
        }
        #endregion
        private void CalculatePercentages()
        {
            try
            {
                double percentage = 0;
                switch (Plugin.ProfileInfo.CounterScoringType)
                {
                    case CounterScoringType.Global:
                        percentage = maxPossibleScore > 0 ? ((double)scoreController.multipliedScore / maxPossibleScore) * 100.0 : 0;
                        break;
                    case CounterScoringType.Local:
                        percentage = scoreController.immediateMaxPossibleMultipliedScore > 0 ? ((double)scoreController.multipliedScore / scoreController.immediateMaxPossibleMultipliedScore) * 100.0 : 0;
                        break;
                }
                CalculatePP(percentage);
            }
            catch (Exception ex)
            {
                Plugin.ErrorPrint($"CalculatePercentages Error: {ex.Message}");
            }
        }

        private void CalculatePP(double percentage)
        {
            foreach (LeaderBoardGameplayInfo gamePlayInfo in gamePlayInfo.lsInfo)
            {
                gamePlayInfo.percentage = percentage;
                gamePlayInfo.pp = ppPredictorMgr.GetPPAtPercentageForCalculator(gamePlayInfo.leaderboard, percentage, _levelFailed, _levelPause, _levelFailed ? gamePlayInfo.failedBeatMapInfo : gamePlayInfo.modifiedBeatMapInfo);
                gamePlayInfo.ppGain = Math.Round(ppPredictorMgr.GetPPGainForCalculator(gamePlayInfo.leaderboard, gamePlayInfo.pp), 2);
                if (gamePlayInfo.maxPP == -1) gamePlayInfo.maxPP = ppPredictorMgr.GetMaxPPForCalculator(gamePlayInfo.leaderboard);
            }
            OnGameplayInfoChanged?.Invoke(this, gamePlayInfo);
        }

        #region calculations
        //Stupid way to do it but works
        private int GetActiveScoreboardsCount()
        {
            int reVal = 0;
            if (ShowScoreSaber()) reVal++;
            if (ShowBeatLeader()) reVal++;
            if (ShowHitBloq()) reVal++;
            return reVal;
        }

        private bool ShowCounter(Leaderboard leaderboard)
        {
            return ppPredictorMgr.IsRanked(leaderboard) || !Plugin.ProfileInfo.CounterHideWhenUnranked;
        }

        private bool ShowScoreSaber()
        {
            return Plugin.ProfileInfo.IsScoreSaberEnabled && ShowCounter(Leaderboard.ScoreSaber);
        }

        private bool ShowBeatLeader()
        {
            return Plugin.ProfileInfo.IsBeatLeaderEnabled && ShowCounter(Leaderboard.BeatLeader);
        }

        private bool ShowHitBloq()
        {
            return Plugin.ProfileInfo.IsHitBloqEnabled && ShowCounter(Leaderboard.HitBloq);
        }
        #endregion

        #region overlay data
        private void OverlayDataSubscribe()
        {
            OnGameplayInfoChanged += GamePlayMgr_OnGameplayInfoChanged;
            OnPaused += GamePlayMgr_OnPaused;
            OnResumed += GamePlayMgr_OnResumed;
            OnLastNoteHit += GamePlayMgr_OnLastNoteHit;
            OnFirstNoteHit += GamePlayMgr_OnFirstNoteHit;
            OnSongStarted += GamePlayMgr_OnSongStarted;
            OnSongFinished += GamePlayMgr_OnSongFinished;
        }
        private void OverlayDataUnsubscribe()
        {
            OnGameplayInfoChanged -= GamePlayMgr_OnGameplayInfoChanged;
            OnPaused -= GamePlayMgr_OnPaused;
            OnResumed -= GamePlayMgr_OnResumed;
            OnLastNoteHit -= GamePlayMgr_OnLastNoteHit;
            OnFirstNoteHit -= GamePlayMgr_OnFirstNoteHit;
            OnSongStarted -= GamePlayMgr_OnSongStarted;
            OnSongFinished -= GamePlayMgr_OnSongFinished;
        }

        private void GamePlayMgr_OnSongFinished(object sender, EventArgs e)
        {
            SendOverlayEvent("OnSongFinished");
        }

        private void GamePlayMgr_OnSongStarted(object sender, EventArgs e)
        {
            SendOverlayEvent("OnSongStarted");
        }

        private void GamePlayMgr_OnFirstNoteHit(object sender, EventArgs e)
        {
            SendOverlayEvent("OnFirstNoteHit");
        }

        private void GamePlayMgr_OnLastNoteHit(object sender, EventArgs e)
        {
            SendOverlayEvent("OnLastNoteHit");
        }

        private void GamePlayMgr_OnResumed(object sender, EventArgs e)
        {
            SendOverlayEvent("OnResumed");
        }

        private void GamePlayMgr_OnPaused(object sender, EventArgs e)
        {
            SendOverlayEvent("OnPaused");
        }

        private void GamePlayMgr_OnGameplayInfoChanged(object sender, GamePlayInfo e)
        {
            List<SimplifiedData> lsSimplifiedData = new List<SimplifiedData>();
            foreach (var item in e.lsInfo)
            {
                lsSimplifiedData.Add(new SimplifiedData(item));
            }
            MessageContainer message = new MessageContainer() { messageType = "OnGameplayInfoChanged", payload= lsSimplifiedData };
            ppPredictorMgr.WebsocketMgr.OverlayServer.SendData(JsonConvert.SerializeObject(message));
        }

        private void SendOverlayEvent(string type)
        {
            MessageContainer message = new MessageContainer() { messageType = type};
            ppPredictorMgr.WebsocketMgr.OverlayServer.SendData(JsonConvert.SerializeObject(message));
        }
        #endregion
    }

    internal class GamePlayInfo
    {
        public int scoreboardCount = 0;
        public List<LeaderBoardGameplayInfo> lsInfo = new List<LeaderBoardGameplayInfo>();
    }

    internal class LeaderBoardGameplayInfo
    {
        public Leaderboard leaderboard;
        public double ppGain;
        public double pp;
        public double maxPP;
        public double targetPercentage;
        public double percentage;
        public PPPBeatMapInfo modifiedBeatMapInfo;
        public PPPBeatMapInfo failedBeatMapInfo;
        public string ppSuffix;
        public double? personalBest;
        public string iconPath;
        public bool isRanked;

        public LeaderBoardGameplayInfo(Leaderboard leaderboard, IPPPredictorMgr ppPredictorMgr, GameplayModifiers gameplayModifiers)
        {
            modifiedBeatMapInfo = ppPredictorMgr.GetModifiedBeatMapInfo(leaderboard, gameplayModifiers);
            failedBeatMapInfo = ppPredictorMgr.GetModifiedBeatMapInfo(leaderboard, gameplayModifiers);
            ppSuffix = ppPredictorMgr.GetPPSuffixForLeaderboard(leaderboard);
            this.leaderboard = leaderboard;
            this.ppGain = 0;
            this.pp = 0;
            this.percentage = 0;
            this.targetPercentage = ppPredictorMgr.GetPercentage();
            this.maxPP = ppPredictorMgr.GetMaxPPForCalculator(leaderboard);
            this.personalBest = ppPredictorMgr.GetPersonalBest(leaderboard);
            this.iconPath = ppPredictorMgr.GetMapPoolIcon(leaderboard);
            this.isRanked = ppPredictorMgr.IsRanked(leaderboard);
        }
    }

    internal class MessageContainer
    {
        public string messageType;
        public List<SimplifiedData> payload = new List<SimplifiedData>();
    }

    internal class SimplifiedData
    {
        public SimplifiedData(LeaderBoardGameplayInfo info)
        {
            leaderboardName = info.leaderboard.ToString();
            pp = info.pp;
            ppGain = info.ppGain;
            maxPP = info.maxPP;
            targetPercentage = info.targetPercentage;
            percentage = info.percentage;
            ppSuffix = info.ppSuffix;
            personalBest = info.personalBest;
            iconPath = info.iconPath;
            isRanked = info.isRanked;
        }

        public string leaderboardName;
        public double pp;
        public double ppGain;
        public double maxPP;
        public double targetPercentage;
        public double percentage;
        public string ppSuffix;
        public double? personalBest;
        public string iconPath;
        public bool isRanked;
    }
}
