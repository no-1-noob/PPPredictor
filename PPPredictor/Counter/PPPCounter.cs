using PPPredictor.Utilities;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

namespace PPPredictor.Counter
{
    public class PPPCounter : CountersPlus.Counters.Custom.BasicCustomCounter
    {
        [Inject] private readonly ScoreController scoreController;
        [Inject] private readonly GameplayCoreSceneSetupData setupData;
        [Inject] private readonly PPPredictorMgr ppPredictorMgr;
        private List<CounterInfoHolder> lsCounterInfoHolder;
        private int maxPossibleScore = 0;
        private bool _levelFailed = false;
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
                Plugin.Log?.Error($"CounterInit Error: {ex.Message}");
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
                    Plugin.Log.Error($"PPPCounter change selected Map: UpdateCurrentBeatMapInfos failed {ex.Message}");
                }
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
                if (Plugin.ProfileInfo.IsScoreSaberEnabled && ShowCounter(Leaderboard.ScoreSaber))
                {
                    lsCounterInfoHolder.Add(new CounterInfoHolder(Leaderboard.ScoreSaber, Settings, ppPredictorMgr, canvas, CanvasUtility, lineOffset, originalLineOffset, positionScale, setupData.gameplayModifiers));
                    lineOffset -= originalLineOffset * 2;
                }
                if (Plugin.ProfileInfo.IsBeatLeaderEnabled && ShowCounter(Leaderboard.BeatLeader))
                {
                    lsCounterInfoHolder.Add(new CounterInfoHolder(Leaderboard.BeatLeader, Settings, ppPredictorMgr, canvas, CanvasUtility, lineOffset, originalLineOffset, positionScale, setupData.gameplayModifiers));
                    lineOffset -= originalLineOffset * 2;
                }
                if (Plugin.ProfileInfo.IsHitBloqEnabled && ShowCounter(Leaderboard.HitBloq))
                {
                    lsCounterInfoHolder.Add(new CounterInfoHolder(Leaderboard.HitBloq, Settings, ppPredictorMgr, canvas, CanvasUtility, lineOffset, originalLineOffset, positionScale, setupData.gameplayModifiers));
                    lineOffset -= originalLineOffset * 2;
                }

                maxPossibleScore = ScoreModel.ComputeMaxMultipliedScoreForBeatmap(setupData.transformedBeatmapData);
                scoreController.scoreDidChangeEvent += ScoreController_scoreDidChangeEvent;
                BS_Utils.Utilities.BSEvents.energyReachedZero += BSEvents_energyReachedZero;
                CalculatePercentages();
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"SetupCounter Error: {ex.Message}");
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
                Plugin.Log?.Error($"CalculatePercentages Error: {ex.Message}");
            }
        }

        public override void CounterDestroy()
        {
            scoreController.scoreDidChangeEvent -= ScoreController_scoreDidChangeEvent;
            BS_Utils.Utilities.BSEvents.energyReachedZero -= BSEvents_energyReachedZero;
        }

        private void DisplayCounterText(double percentage)
        {
#if DEBUG
            debugPercentage.text = $"{Plugin.ProfileInfo.CounterScoringType} {percentage:F2}%";
#endif
            lsCounterInfoHolder.ForEach(item => item.UpdateCounterText(percentage, _levelFailed));
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
