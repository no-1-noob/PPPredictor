using HMUI;
using PPPredictor.Utilities;
using System;
using System.Linq;
using System.Reflection;
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
        private ImageView iconScoreSaber;
        private ImageView iconBeatLeader;
        private int maxPossibleScore = 0;
#if DEBUG
        private TMP_Text debugPercentage;
#endif
        private bool _showScoreSaber = false;
        private bool _showBeatLeader = false;
        private readonly int fontSize = 3;
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
                float iconTextOffset = Plugin.ProfileInfo.CounterUseIcons ? -.9f : 0f;
                _showScoreSaber = Plugin.pppViewController.ppPredictorMgr.IsRanked(Leaderboard.ScoreSaber) || !Plugin.ProfileInfo.CounterHideWhenUnranked;
                _showBeatLeader = Plugin.pppViewController.ppPredictorMgr.IsRanked(Leaderboard.BeatLeader) || !Plugin.ProfileInfo.CounterHideWhenUnranked;
                headerPpScoreSaber = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(-1f, +lineOffset, 0));
                headerPpBeatLeader = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(-1f, -lineOffset, 0));
                ppScoreSaber = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(0.9f + iconTextOffset, +lineOffset, 0));
                ppBeatLeader = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(0.9f + iconTextOffset, -lineOffset, 0));
                ppGainScoreSaber = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(1.2f + iconTextOffset, +lineOffset, 0));
                ppGainBeatLeader = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(1.2f + iconTextOffset, -lineOffset, 0));

                headerPpScoreSaber.alignment = headerPpBeatLeader.alignment = ppGainScoreSaber.alignment = ppGainBeatLeader.alignment = TextAlignmentOptions.BottomLeft;
                ppScoreSaber.alignment = ppBeatLeader.alignment = TextAlignmentOptions.BottomRight;
                headerPpScoreSaber.fontSize = headerPpBeatLeader.fontSize = ppScoreSaber.fontSize = ppBeatLeader.fontSize = ppGainScoreSaber.fontSize = ppGainBeatLeader.fontSize = fontSize;
#if DEBUG
                debugPercentage = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(-0.5f, 1, 0));
#endif
                var canvas = CanvasUtility.GetCanvasFromID(this.Settings.CanvasID);
                if (canvas != null && Plugin.ProfileInfo.CounterUseIcons)
                {
                    iconScoreSaber = CreateIcon(canvas, "PPPredictor.Resources.LeaderBoardLogos.ScoreSaber.png", new Vector3(-1f, +lineOffset, 0));
                    iconBeatLeader = CreateIcon(canvas, "PPPredictor.Resources.LeaderBoardLogos.BeatLeader.png", new Vector3(-1f, -lineOffset, 0));
                }

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

            if (_showScoreSaber && !Plugin.ProfileInfo.CounterUseIcons) headerPpScoreSaber.text = $"<color=\"{percentageThresholdColor}\">ScoreSaber</color>";
            if (_showBeatLeader && !Plugin.ProfileInfo.CounterUseIcons) headerPpBeatLeader.text = $"<color=\"{percentageThresholdColor}\">BeatLeader</color>";
            if (_showScoreSaber)
            {
                if(Plugin.ProfileInfo.CounterUseIcons) iconScoreSaber.enabled = true;
                double scoreSaberPP = Plugin.pppViewController.ppPredictorMgr.GetPPAtPercentageForCalculator(Leaderboard.ScoreSaber, percentage);
                double scoreSaberGain = Math.Round(Plugin.pppViewController.ppPredictorMgr.GetPPGainForCalculator(Leaderboard.ScoreSaber, scoreSaberPP), 2);
                ppScoreSaber.text = $"{scoreSaberPP:F2}pp";
                if (Plugin.ProfileInfo.CounterShowGain) ppGainScoreSaber.text = $"[<color=\"{DisplayHelper.GetDisplayColor(scoreSaberGain, false)}\">{scoreSaberGain:F2}</color>]";
            }
            if (_showBeatLeader)
            {
                if (Plugin.ProfileInfo.CounterUseIcons) iconBeatLeader.enabled = true;
                double beatLeaderPP = Plugin.pppViewController.ppPredictorMgr.GetPPAtPercentageForCalculator(Leaderboard.BeatLeader, percentage);
                double beatLeaderGain = Plugin.pppViewController.ppPredictorMgr.GetPPGainForCalculator(Leaderboard.BeatLeader, beatLeaderPP);
                ppBeatLeader.text = $"{beatLeaderPP:F2}pp";
                if (Plugin.ProfileInfo.CounterShowGain) ppGainBeatLeader.text = $"[<color=\"{DisplayHelper.GetDisplayColor(beatLeaderGain, false)}\">{beatLeaderGain:F2}</color>]";
            }
        }

        private ImageView CreateIcon(Canvas canvas, string imageIdent, Vector3 offset)
        {
            GameObject imageGameObject = new GameObject(imageIdent, typeof(RectTransform));
            ImageView newImage = imageGameObject.AddComponent<ImageView>();
            float posScaleFactor = 10;
            newImage.rectTransform.SetParent(canvas.transform, false);
            newImage.rectTransform.anchoredPosition = posScaleFactor * (CanvasUtility.GetAnchoredPositionFromConfig(Settings) + offset + new Vector3(0, lineOffset/1.25f, 0));
            newImage.rectTransform.sizeDelta = new Vector2(2.5f, 2.5f);
            newImage.enabled = false;
            var noGlowMat = new Material(Resources.FindObjectsOfTypeAll<Material>().Where(m => m.name == "UINoGlow").First());
            noGlowMat.name = "UINoGlowCustom";
            newImage.material = noGlowMat;
            //Load Image
            var assembly = Assembly.GetExecutingAssembly();
            System.IO.Stream stream = assembly.GetManifestResourceStream(imageIdent);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(data);
            texture.Apply();
            newImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            return newImage;
        }
    }
}
