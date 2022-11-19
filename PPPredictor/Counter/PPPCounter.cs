using PPPredictor.Utilities;
using System;
using System.Collections.Generic;
using Zenject;

namespace PPPredictor.Counter
{
    public class PPPCounter : CountersPlus.Counters.Custom.BasicCustomCounter
    {
        [Inject] private readonly ScoreController scoreController;
        [Inject] private readonly GameplayCoreSceneSetupData setupData;
        private List<CounterInfoHolder> lsCounterInfoHolder;
        private int maxPossibleScore = 0;
#if DEBUG
        private TMP_Text debugPercentage;
#endif
        private readonly float lineOffset = 0.15f;

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
#if DEBUG
                debugPercentage = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(-0.5f, 1, 0));
#endif
                var canvas = CanvasUtility.GetCanvasFromID(this.Settings.CanvasID);
                lsCounterInfoHolder = new List<CounterInfoHolder>();
                if (Plugin.ProfileInfo.IsScoreSaberEnabled) lsCounterInfoHolder.Add(new CounterInfoHolder(Leaderboard.ScoreSaber, Settings, "PPPredictor.Resources.LeaderBoardLogos.ScoreSaber.png", canvas, CanvasUtility, +lineOffset));
                if (Plugin.ProfileInfo.IsScoreSaberEnabled) lsCounterInfoHolder.Add(new CounterInfoHolder(Leaderboard.BeatLeader, Settings, "PPPredictor.Resources.LeaderBoardLogos.BeatLeader.png", canvas, CanvasUtility, -lineOffset));

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
            lsCounterInfoHolder.ForEach(item => item.UpdateCounterText(percentage));
        }

        
    }
}
