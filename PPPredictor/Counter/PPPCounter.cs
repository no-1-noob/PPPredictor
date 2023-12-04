using PPPredictor.Interfaces;
using PPPredictor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

namespace PPPredictor.Counter
{
    class PPPCounter : CountersPlus.Counters.Custom.BasicCustomCounter
    {
#pragma warning disable CS0649
        [Inject] private readonly ScoreController scoreController;
        [Inject] private readonly GameplayCoreSceneSetupData setupData;
        [Inject] private readonly PauseController pauseController;
        [Inject] private readonly BeatmapObjectManager beatmapObjectManager;
        [Inject] private readonly IPPPredictorMgr ppPredictorMgr;
#pragma warning restore CS0649
        private List<CounterInfoHolder> lsCounterInfoHolder;
        private int maxPossibleScore = 0;
        private bool _levelFailed = false;
        private bool _levelPause = false;
        private bool _iconMoved = false;
        private int _noteCount = 0;
        private int _noteDone = 0;
        private bool _isSongStarted = false;
        private bool _isSongFinished = false;
#if DEBUG
        private TMP_Text debugPercentage;
#endif
        private readonly float originalLineOffset = 0.15f;

        public override void CounterInit()
        {
            try
            {
                if (setupData.practiceSettings == null)
                {
                    SetupCounter();
                }
            }
            catch (Exception ex)
            {
                Plugin.ErrorPrint($"CounterInit Error: {ex.Message}");
            }
            
        }

        private async void SetupCounter()
        {
            try
            {
                try
                {
                    if (setupData.previewBeatmapLevel is CustomBeatmapLevel)
                    {
                        await ppPredictorMgr.UpdateCurrentBeatMapInfos(setupData.previewBeatmapLevel as CustomBeatmapLevel, setupData.difficultyBeatmap);
                    }
                }
                catch (Exception ex)
                {
                    Plugin.ErrorPrint($"PPPCounter change selected Map: UpdateCurrentBeatMapInfos failed {ex.Message}");
                }

                pauseController.didPauseEvent += PauseController_didPauseEvent;
                pauseController.didResumeEvent += PauseController_didResumeEvent;
                beatmapObjectManager.noteWasCutEvent += BeatmapObjectManager_noteWasCutEvent;
                beatmapObjectManager.noteWasMissedEvent += BeatmapObjectManager_noteWasMissedEvent;

                var v = await setupData.difficultyBeatmap.GetBeatmapDataBasicInfoAsync();
                _noteCount = v.cuttableNotesCount;
#if DEBUG
                //Center Helper for development
                /*float offsetPlus = 0.1f;
                for (int i = 0; i < 5; i++)
                {
                    TMP_Text testPlus = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(offsetPlus * -(i + 1), ((i % 2) + 1) * offsetPlus, 0));
                    testPlus.alignment = TextAlignmentOptions.Center;
                    testPlus.text = "+";
                }
                for (int i = 0; i < 5; i++)
                {
                    TMP_Text testPlus = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(offsetPlus * (i + 1), ((i % 2) + 1) * offsetPlus, 0));
                    testPlus.alignment = TextAlignmentOptions.Center;
                    testPlus.text = "+";
                }*/
                //debugPercentage = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(0, 0, 0));
                //debugPercentage.alignment = TextAlignmentOptions.Center;
#endif
                var canvas = CanvasUtility.GetCanvasFromID(this.Settings.CanvasID);
                float positionScale = CanvasUtility.GetCanvasSettingsFromCanvas(canvas).PositionScale;
                lsCounterInfoHolder = new List<CounterInfoHolder>();
                int scoreboardCount = GetActiveScoreboardsCount();
                float lineOffset = (originalLineOffset * (scoreboardCount / 2)) + (originalLineOffset * (scoreboardCount % 2));
                int id = 0;
                if (ShowScoreSaber())
                {
                    lsCounterInfoHolder.Add(new CounterInfoHolder(id, Leaderboard.ScoreSaber, Settings, ppPredictorMgr, canvas, CanvasUtility, lineOffset, originalLineOffset, positionScale, setupData.gameplayModifiers));
                    lineOffset -= originalLineOffset * 2;
                    id++;
                }
                if (ShowBeatLeader())
                {
                    lsCounterInfoHolder.Add(new CounterInfoHolder(id, Leaderboard.BeatLeader, Settings, ppPredictorMgr, canvas, CanvasUtility, lineOffset, originalLineOffset, positionScale, setupData.gameplayModifiers));
                    lineOffset -= originalLineOffset * 2;
                    id++;
                }
                if (ShowHitBloq())
                {
                    lsCounterInfoHolder.Add(new CounterInfoHolder(id, Leaderboard.HitBloq, Settings, ppPredictorMgr, canvas, CanvasUtility, lineOffset, originalLineOffset, positionScale, setupData.gameplayModifiers));
                    lineOffset -= originalLineOffset * 2;
                    id++;
                }

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

        private void BeatmapObjectManager_noteWasMissedEvent(NoteController noteController)
        {
            CheckNotesLeft(noteController);
        }

        private void BeatmapObjectManager_noteWasCutEvent(NoteController noteController, in NoteCutInfo noteCutInfo)
        {
                CheckNotesLeft(noteController);   
        }

        private void PauseController_didResumeEvent()
        {
            if (Plugin.ProfileInfo.IsCounterGainSilentModeEnabled && !_isSongFinished)
            {
                lsCounterInfoHolder.ForEach(item => _ = item.MoveTextWithAnimation(AnimateableCounterText.PPGAIN, 100f, new Vector3(0, 0, 0), false, true, true, true, item.IsPersonalBestAnimationRunning));
            }
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
                lsCounterInfoHolder.ForEach(item => _ = item.MoveTextWithAnimation(AnimateableCounterText.PPGAIN, 100f, new Vector3(0, .3f, 0), true, true, true, true, item.IsPersonalBestAnimationRunning));
            }
        }

        private void CheckNotesLeft(NoteController noteController)
        {
            if(!_isSongFinished
                && noteController.noteData.gameplayType != NoteData.GameplayType.Bomb
                && noteController.noteData.gameplayType != NoteData.GameplayType.BurstSliderElementFill
                && noteController.noteData.gameplayType != NoteData.GameplayType.BurstSliderElement)
            {
                lsCounterInfoHolder.ForEach(item =>
                {
                    if (!item.IsPersonalBestAnimationDone())
                    {
                        _ = item.StartPersonalBestAnimation(0);
                    }
                });
                _noteDone++;
                _isSongStarted = true;
                if (_noteDone >= _noteCount && Plugin.ProfileInfo.IsCounterGainSilentModeEnabled)
                {
                    _isSongFinished = true;
                    lsCounterInfoHolder.ForEach(item => _ = item.MoveTextWithAnimation(AnimateableCounterText.PPGAIN, 100f, new Vector3(0, .3f, 0), true, true, true, true, item.IsPersonalBestAnimationRunning));
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
                DisplayCounterText(percentage);
            }
            catch (Exception ex)
            {
                Plugin.ErrorPrint($"CalculatePercentages Error: {ex.Message}");
            }
        }

        public override void CounterDestroy()
        {
            scoreController.scoreDidChangeEvent -= ScoreController_scoreDidChangeEvent;
            BS_Utils.Utilities.BSEvents.energyReachedZero -= BSEvents_energyReachedZero;
            pauseController.didPauseEvent -= PauseController_didPauseEvent;
            pauseController.didResumeEvent -= PauseController_didResumeEvent;
            beatmapObjectManager.noteWasCutEvent -= BeatmapObjectManager_noteWasCutEvent;
            beatmapObjectManager.noteWasMissedEvent -= BeatmapObjectManager_noteWasMissedEvent;
        }

        private void DisplayCounterText(double percentage)
        {
            lsCounterInfoHolder.ForEach(item => item.UpdateCounterText(percentage, _levelFailed, _levelPause));
            if(!_iconMoved && !lsCounterInfoHolder.Where(x => x.MaxPP == -1).Any())
            {
                _iconMoved = true;
                double maxMaxPP = lsCounterInfoHolder.Select(x => x.MaxPP).Max();
                lsCounterInfoHolder.ForEach(item => item.MoveIconForLongMaxPP(Math.Truncate(maxMaxPP).ToString().Length));
            }
        }

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
    }
}
