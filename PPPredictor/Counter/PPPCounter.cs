using PPPredictor.Utilities;
using System;
using TMPro;
using UnityEngine;
using Zenject;

namespace PPPredictor.Counter
{
    public class PPPCounter : CountersPlus.Counters.Custom.BasicCustomCounter
    {
        [Inject] private readonly ScoreController scoreController;
        [Inject] private readonly GameplayCoreSceneSetupData setupData;
        private TMP_Text ppScoreSaber;
        private TMP_Text ppBeatLeader;
        private TMP_Text ppGainScoreSaber;
        private TMP_Text ppGainBeatLeader;
        private TMP_Text headerPpBeatLeader;
        private TMP_Text headerPpScoreSaber;
        private int maxPossibleScore = 0;
#if DEBUG
        private TMP_Text debugPercentage;
#endif
        private bool _showScoreSaber = false;
        private bool _showBeatLeader = false;
        private readonly int fontSize = 3;
        private readonly float lineOffset = 0.15f;
        //TODO: logo

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

        private void SetupCounter()
        {
            try
            {
                _showScoreSaber = Plugin.pppViewController.ppPredictorMgr.IsRanked(Leaderboard.ScoreSaber) || !Plugin.ProfileInfo.CounterHideWhenUnranked;
                _showBeatLeader = Plugin.pppViewController.ppPredictorMgr.IsRanked(Leaderboard.BeatLeader) || !Plugin.ProfileInfo.CounterHideWhenUnranked;
                headerPpScoreSaber = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(-1f, +lineOffset, 0));
                headerPpBeatLeader = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(-1f, -lineOffset, 0));
                ppScoreSaber = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(0.9f, +lineOffset, 0));
                ppBeatLeader = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(0.9f, -lineOffset, 0));
                ppGainScoreSaber = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(1.2f, +lineOffset, 0));
                ppGainBeatLeader = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(1.2f, -lineOffset, 0));

                headerPpScoreSaber.alignment = headerPpBeatLeader.alignment = ppGainScoreSaber.alignment = ppGainBeatLeader.alignment = TextAlignmentOptions.BottomLeft;
                ppScoreSaber.alignment = ppBeatLeader.alignment = TextAlignmentOptions.BottomRight;
                headerPpScoreSaber.fontSize = headerPpBeatLeader.fontSize = ppScoreSaber.fontSize = ppBeatLeader.fontSize = ppGainScoreSaber.fontSize = ppGainBeatLeader.fontSize = fontSize;
#if DEBUG
                debugPercentage = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(-0.5f, 1, 0));
#endif
                maxPossibleScore = ScoreModel.ComputeMaxMultipliedScoreForBeatmap(setupData.transformedBeatmapData);
                scoreController.scoreDidChangeEvent += ScoreController_scoreDidChangeEvent;
                CalculatePercentages();
            }
            catch (Exception ex)
            {
                Plugin.Log?.Error($"SetupCounter Error: {ex.Message}");
            }
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
        }

        private void DisplayCounterText(double percentage)
        {
#if DEBUG
            debugPercentage.text = $"{Plugin.ProfileInfo.CounterScoringType} {percentage:F2}%";
#endif
            string percentageThresholdColor = DisplayHelper.GetDisplayColor(0, false);
            if (percentage > Plugin.pppViewController.ppPredictorMgr.GetPercentage() && Plugin.ProfileInfo.CounterHighlightTargetPercentage)
            {
                percentageThresholdColor = DisplayHelper.GetDisplayColor(1, false);
            }

            if (_showScoreSaber) headerPpScoreSaber.text = $"<color=\"{percentageThresholdColor}\">ScoreSaber: </color>";
            if (_showBeatLeader) headerPpBeatLeader.text = $"<color=\"{percentageThresholdColor}\">BeatLeader: </color>";
            if (_showScoreSaber)
            {
                double scoreSaberPP = Plugin.pppViewController.ppPredictorMgr.GetPPAtPercentageForCalculator(Leaderboard.ScoreSaber, percentage);
                double scoreSaberGain = Math.Round(Plugin.pppViewController.ppPredictorMgr.GetPPGainForCalculator(Leaderboard.ScoreSaber, scoreSaberPP), 2);
                ppScoreSaber.text = $"{scoreSaberPP:F2}pp";
                if (Plugin.ProfileInfo.CounterShowGain) ppGainScoreSaber.text = $"[<color=\"{DisplayHelper.GetDisplayColor(scoreSaberGain, false)}\">{scoreSaberGain:F2}</color>]";
            }
            if (_showBeatLeader)
            {
                double beatLeaderPP = Plugin.pppViewController.ppPredictorMgr.GetPPAtPercentageForCalculator(Leaderboard.BeatLeader, percentage);
                double beatLeaderGain = Plugin.pppViewController.ppPredictorMgr.GetPPGainForCalculator(Leaderboard.BeatLeader, beatLeaderPP);
                ppBeatLeader.text = $"{beatLeaderPP:F2}pp";
                if (Plugin.ProfileInfo.CounterShowGain) ppGainBeatLeader.text = $"[<color=\"{DisplayHelper.GetDisplayColor(beatLeaderGain, false)}\">{beatLeaderGain:F2}</color>]";
            }
        }
    }
}
